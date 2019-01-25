using System.Collections;
using System;
using System.Runtime.InteropServices;
using VPx.Encoder;
using VPx.Codec;
using VPx.Image;
using System.Collections.Generic;
using VPx.Decoder;

namespace ExitGames.Client.Photon.Voice
{
    public class VPxCodec
    {
        delegate int EncoderFormatConvertFunc(IntPtr src_frame, int src_stride_frame, IntPtr dst_y, int dst_stride_y, IntPtr dst_u, int dst_stride_u, IntPtr dst_v, int dst_stride_v, int width, int height);

        public static int EncoderFormatBypass(IntPtr src_y, int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, IntPtr dst, int dst_stride, int width, int height)
        {
            return 0;
        }

        public static int Android420ToI420(IntPtr src_y, int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, IntPtr dst, int dst_stride, int width, int height)
        {
            return 0;
        }

        static Dictionary<ImageFormat, EncoderFormatConvertFunc> toEncoderFormat = new Dictionary<ImageFormat, EncoderFormatConvertFunc>()
            {
                { ImageFormat.I420, EncoderFormatBypass},
                { ImageFormat.YV12, EncoderFormatBypass},
                { ImageFormat.Android420, Android420ToI420 },
                { ImageFormat.RGBA, LibYUV.LibYUV.RGBAToI420},
                { ImageFormat.ABGR, LibYUV.LibYUV.ABGRToI420},
                { ImageFormat.BGRA, LibYUV.LibYUV.BGRAToI420},
                { ImageFormat.ARGB, LibYUV.LibYUV.ARGBToI420},
            };

        static Dictionary<ImageFormat, EncoderFormatConvertFunc> fromEncoderFormat = new Dictionary<ImageFormat, EncoderFormatConvertFunc>()
            {
                { ImageFormat.I420, EncoderFormatBypass}, // TODO:  I420ToYV12
                { ImageFormat.YV12, EncoderFormatBypass},
                { ImageFormat.Android420, Android420ToI420 }, // or unsupported?
                { ImageFormat.RGBA, LibYUV.LibYUV.I420ToRGBA},
                { ImageFormat.ABGR, LibYUV.LibYUV.I420ToABGR},
                { ImageFormat.BGRA, LibYUV.LibYUV.I420ToBGRA},
                { ImageFormat.ARGB, LibYUV.LibYUV.I420ToARGB},
            };

        static VPxCodec()
        {
            var len = Enum.GetValues(typeof(ImageFormat)).Length;
            if (len != toEncoderFormat.Count)
            {
                throw new Exception("Wrong toEncoderFormat elements count");
            }
            if (len != fromEncoderFormat.Count)
            {
                throw new Exception("Wrong fromEncoderFormat elements count");
            }
        }

        // When creating vpx_image_t, use single format for all rgba formats
        // vpx_img_fmt_t misses some ImageFormat' rgba variations.
        static vpx_img_fmt_t formatToVPxImgFmt(ImageFormat f)
        {
            switch (f)
            {
                case ImageFormat.I420: return vpx_img_fmt_t.VPX_IMG_FMT_I420;
                case ImageFormat.YV12: return vpx_img_fmt_t.VPX_IMG_FMT_YV12;
                default: return vpx_img_fmt_t.VPX_IMG_FMT_ARGB;
            }
        }

        public class Encoder : IEncoderNativeImageDirect
        {
            vpx_codec_ctx_t ctx = new vpx_codec_ctx_t();
            // video -> scale -> format conversion
            IntPtr[] framePtr = new IntPtr[5];
            vpx_image_t[] frame = new vpx_image_t[5];
            Rotation imageRotation = Rotation.Rotate0;
            Flip imageFlip = Flip.None;
            EncoderFormatConvertFunc formatConvertFunc;
            int pts;
            int frame_count;
            bool disposed;

            // scale source image before encoding
            int encoderWidth;
            int encoderHeight;
            int bitrate;

            public Encoder(VoiceInfo info)
            {
                Open(info);
            }

            static void die_codec(vpx_codec_ctx_t ctx, string s)
            {
                IntPtr detail = VPx.VPx.vpx_codec_error_detail(ref ctx);
                IntPtr err = VPx.VPx.vpx_codec_error(ref ctx);
                s = "VEn: " + s + ". " + Marshal.PtrToStringAnsi(err);
                if (detail != IntPtr.Zero)
                    s += " " + Marshal.PtrToStringAnsi(detail);
                throw new Exception(s);
            }

            vpx_codec_enc_cfg cfg = new vpx_codec_enc_cfg();

            bool Open(VoiceInfo info)
            {
                encoderWidth = info.Width;
                encoderHeight = info.Height;
                bitrate = info.Bitrate;
                return true;
            }

            vpx_codec_cx_pkt_t pkt = new vpx_codec_cx_pkt_t();

            public IEnumerable<ArraySegment<byte>> GetOutput()
            {
                lock (buffQueue)
                {
                    foreach (var x in buffQueue)
                    {
                        yield return x;
                    }
                    buffQueue.Clear();
                }                
            }

            Queue<ArraySegment<byte>> buffQueue = new Queue<ArraySegment<byte>>();

            void createImage(int i, vpx_img_fmt_t f, int w, int h)
            {
                VPx.VPx.vpx_img_free(framePtr[i]);
                framePtr[i] = VPx.VPx.vpx_img_alloc(IntPtr.Zero, f, (uint)w, (uint)h, 1);
                if (framePtr[i] == IntPtr.Zero)
                {
                    die_codec(ctx, "VEn: Failed to allocate image.");
                }

                frame[i] = (vpx_image_t)Marshal.PtrToStructure(framePtr[i], typeof(vpx_image_t));
            }
            
            public IEnumerable<ArraySegment<byte>> EncodeAndGetOutput(IntPtr[] buf, int srcWidth, int srcHeight, int[] stride, ImageFormat srcFormat, Rotation rotation, Flip flip)
            {
                lock (this)
                {
                    if (disposed)
                    {
                        yield break;
                    }

                    bool flipDone; //same var used in setup and processing (after reinitialization)
                                   // flip may be done by one of the methods in pipeline
                                   // horizontal: Mirror functionality can also be achieved with the I420Scale and ARGBScale functions by passing negative width and / or height.
                                   // verical: Inverting can be achieved with almost any libyuv function by passing a negative source height.

                    var srcVPxImgFmt = formatToVPxImgFmt(srcFormat);
                    uint pre_rot_w, pre_rot_h;
                    if (frame[0].w != srcWidth || frame[0].h != srcHeight || frame[0].fmt != srcVPxImgFmt || imageRotation != rotation || imageFlip != flip)
                    {
                        free();

                        var iface = VPx.VPx.vpx_codec_vp8_cx();
                        if (VPx.VPx.vpx_codec_enc_config_default(iface, ref cfg, 0) != 0)
                        {
                            die_codec(ctx, "vpx_codec_enc_config_default");
                        }
                        cfg.g_error_resilient = vpx_codec_er_flags_t.VPX_ERROR_RESILIENT_DEFAULT;
                        if (bitrate != 0)
                        {
                            cfg.rc_target_bitrate = (uint)bitrate / 1024;
                        }
                        if (encoderWidth != 0)
                        {
                            cfg.g_w = (uint)encoderWidth;
                            cfg.g_h = (uint)(encoderHeight == -1 ? encoderWidth * srcHeight / srcWidth : encoderHeight);
                        }
                        else
                        {
                            cfg.g_w = (uint)srcWidth;
                            cfg.g_h = (uint)srcHeight;
                        }
                        pre_rot_w = cfg.g_w;
                        pre_rot_h = cfg.g_h;
                        if (rotation == Rotation.Rotate90 || rotation == Rotation.Rotate270)
                        {
                            var tmp = cfg.g_w; cfg.g_w = cfg.g_h; cfg.g_h = tmp;
                        }
                        if (VPx.VPx.vpx_codec_enc_init(ref ctx, iface, ref cfg, 0) != 0)
                        {
                            die_codec(ctx, "vpx_codec_enc_init");
                        }

                        formatConvertFunc = toEncoderFormat[srcFormat];

                        // Conversion is not required if source format is one of 2 supported.
                        vpx_img_fmt_t format = vpx_img_fmt_t.VPX_IMG_FMT_I420;
                        if (srcFormat == ImageFormat.YV12)
                        {
                            format = vpx_img_fmt_t.VPX_IMG_FMT_YV12;
                        }

                        createImage(0, srcVPxImgFmt, srcWidth, srcHeight);

                        flipDone = flip == Flip.None;
                        if (formatConvertFunc != EncoderFormatBypass)
                        {
                            createImage(1, format, srcWidth, srcHeight);
                            if (flip == Flip.Vertical)
                            {
                                flipDone = true;
                            }
                        }

                        if (srcWidth != pre_rot_w || srcHeight != pre_rot_h)
                        {
                            createImage(2, format, (int)pre_rot_w, (int)pre_rot_h);
                            flipDone = true;
                        }

                        imageRotation = rotation;
                        imageFlip = flip;
                        if (rotation != Rotation.Rotate0)
                        {
                            createImage(3, format, (int)cfg.g_w, (int)cfg.g_h);
                            if (flip == Flip.Vertical)
                            {
                                flipDone = true;
                            }
                        }

                        if (!flipDone)
                        {
                            createImage(4, format, (int)cfg.g_w, (int)cfg.g_h);
                        }
                    }

                    flipDone = flip == Flip.None;
                    // - height for vertical flip
                    var flipH = flip == Flip.Vertical ? -1 : 1;
                    // - width for horizontal flip
                    var flipW = flip == Flip.Horizontal ? -1 : 1;

                    for (int i = 0; i < buf.Length; i++)
                    {
                        frame[0].planes[i] = buf[i];
                        frame[0].stride[i] = stride[i];
                    }
                    int I = 0; // current frame in processing pipe
                    if (formatConvertFunc != EncoderFormatBypass)
                    {
                        if (formatConvertFunc == Android420ToI420)
                        {
                            var p = frame[0].planes;
                            var s = frame[0].stride;
                            LibYUV.LibYUV.Android420ToI420(p[0], s[0], p[1], s[1], p[2], s[2], 2,
                                frame[1].planes[0], frame[1].stride[0],
                                frame[1].planes[1], frame[1].stride[1],
                                frame[1].planes[2], frame[1].stride[2],
                                (int)frame[1].w, (int)frame[1].h * flipH);
                        }
                        else
                        {
                            formatConvertFunc(
                                frame[0].planes[0], (int)frame[0].w * 4,
                                frame[1].planes[0], frame[1].stride[0],
                                frame[1].planes[1], frame[1].stride[1],
                                frame[1].planes[2], frame[1].stride[2],
                                (int)frame[1].w, (int)frame[1].h * flipH
                                );
                        }
                        if (flip == Flip.Vertical)
                        {
                            flipDone = true;
                            flipW = flipH = 1;
                        }
                        I = 1;
                    }

                    pre_rot_w = cfg.g_w;
                    pre_rot_h = cfg.g_h;
                    if (rotation == Rotation.Rotate90 || rotation == Rotation.Rotate270)
                    {
                        var tmp = pre_rot_w; pre_rot_w = pre_rot_h; pre_rot_h = tmp;
                    }
                    // TODO: YV12 scale
                    if (srcWidth != pre_rot_w || srcHeight != pre_rot_h)
                    {
                        LibYUV.LibYUV.I420Scale(
                            frame[I].planes[0], frame[I].stride[0],
                            frame[I].planes[1], frame[I].stride[1],
                            frame[I].planes[2], frame[I].stride[2],
                            (int)frame[1].w * flipW, (int)frame[1].h * flipH,
                            frame[2].planes[0], frame[2].stride[0],
                            frame[2].planes[1], frame[2].stride[1],
                            frame[2].planes[2], frame[2].stride[2],
                            (int)frame[2].w, (int)frame[2].h,
                            LibYUV.LibYUV.FilterMode.kFilterNone
                            );
                        flipDone = true;
                        flipW = flipH = 1;
                        I = 2;
                    }

                    if (rotation != Rotation.Rotate0)
                    {
                        LibYUV.LibYUV.I420Rotate(
                            frame[I].planes[0], frame[I].stride[0],
                            frame[I].planes[1], frame[I].stride[1],
                            frame[I].planes[2], frame[I].stride[2],
                            frame[3].planes[0], frame[3].stride[0],
                            frame[3].planes[1], frame[3].stride[1],
                            frame[3].planes[2], frame[3].stride[2],
                            (int)frame[I].w, (int)frame[I].h * flipH,
                            (LibYUV.LibYUV.RotationMode)rotation
                            );
                        if (flip == Flip.Vertical)
                        {
                            flipDone = true;
                            flipW = flipH = 1;
                        }
                        I = 3;
                    }

                    if (!flipDone)
                    {
                        if (flip == Flip.Vertical)
                        {
                            LibYUV.LibYUV.I420Copy(
                                frame[I].planes[0], frame[I].stride[0],
                                frame[I].planes[1], frame[I].stride[1],
                                frame[I].planes[2], frame[I].stride[2],
                                frame[4].planes[0], frame[4].stride[0],
                                frame[4].planes[1], frame[4].stride[1],
                                frame[4].planes[2], frame[4].stride[2],
                                (int)frame[I].w, -(int)frame[I].h
                                );
                        }
                        else
                        {
                            LibYUV.LibYUV.I420Mirror(
                                frame[I].planes[0], frame[I].stride[0],
                                frame[I].planes[1], frame[I].stride[1],
                                frame[I].planes[2], frame[I].stride[2],
                                frame[4].planes[0], frame[4].stride[0],
                                frame[4].planes[1], frame[4].stride[1],
                                frame[4].planes[2], frame[4].stride[2],
                                (int)frame[I].w, (int)frame[I].h
                                );
                        }
                        I = 4;
                    }

                    int flags = 0;
                    if (frame_count % 15 == 0)
                    {
                        flags |= VPx.Encoder.EncoderConst.VPX_EFLAG_FORCE_KF;
                    }

                    // debug
                    //var p0 = new byte[frame[I].stride[0] * frame[I].h];
                    //var p1 = new byte[frame[I].stride[1] * frame[I].h];
                    //var p2 = new byte[frame[I].stride[2] * frame[I].h];
                    //Marshal.Copy(frame[I].planes[0], p0, 0, p0.Length);
                    //Marshal.Copy(frame[I].planes[1], p1, 0, p1.Length);
                    //Marshal.Copy(frame[I].planes[2], p2, 0, p2.Length);

                    if (0 != VPx.VPx.vpx_codec_encode(ref ctx, ref frame[I], pts, 30000000, flags, EncoderConst.VPX_DL_REALTIME))
                    {
                        die_codec(ctx, "Failed to encode frame");
                    }
                    pts++;

                    IntPtr iter;
                    IntPtr pktPtr;
                    while ((pktPtr = VPx.VPx.vpx_codec_get_cx_data(ref ctx, out iter)) != IntPtr.Zero)
                    {
                        pkt = (vpx_codec_cx_pkt_t)Marshal.PtrToStructure(pktPtr, typeof(vpx_codec_cx_pkt_t));
                        if (pkt.kind == vpx_codec_cx_pkt_kind.VPX_CODEC_CX_FRAME_PKT)
                        {
                            var size = (int)pkt.data.frame.sz;
                            if (payload.Length < size)
                            {
                                payload = new byte[size];
                            }
                            Marshal.Copy(pkt.data.frame.buf, payload, 0, size);
                            lock (buffQueue)
                            {
                                if (buffQueue.Count < 10)
                                {
                                    yield return new ArraySegment<byte>(payload, 0, size);
                                }
                            }
                        }
                    }
                    frame_count++;
                }
            }

            byte[] payload = new byte[0];
            public void Dispose()
            {
                lock (this)
                {
                    disposed = true;
                    free();
                }
            }
            void free()
            {
                VPx.VPx.vpx_codec_destroy(ref ctx);                
                for (int i = 0;i < framePtr.Length;i++)
                {
                    VPx.VPx.vpx_img_free(framePtr[i]);
                    framePtr[i] = IntPtr.Zero;
                }
            }
        }


        public class Decoder : IDecoderQueuedOutputImageNative
        {
            vpx_codec_ctx_t ctx = new vpx_codec_ctx_t();
            bool ready;

            public Decoder()
            {
                this.OutputImageFormat = ImageFormat.ARGB;
            }
            
            public ImageFormat OutputImageFormat { get; set; }
            public Flip OutputImageFlip
            {
                get { return flip; }
                set
                {
                    if (value == Flip.Horizontal)
                    {
                        throw new NotImplementedException("Horizontal flip not implemented.");
                    }
                    flip = value;
                }
            }
            private Flip flip = Flip.None;
            public Func<int, int, IntPtr> OutputImageBufferGetter { get; set; }
            public OnImageOutputNative OnOutputImage { get; set; }

            static void die(string s)
            {
                s = "VDe: " + s;
                throw new Exception(s);
            }

            static void die_codec(vpx_codec_ctx_t ctx, string s)
            {
                IntPtr detail = VPx.VPx.vpx_codec_error_detail(ref ctx);
                IntPtr err = VPx.VPx.vpx_codec_error(ref ctx);
                s = "VDe: " + s + ". " + Marshal.PtrToStringAnsi(err);
                if (detail != IntPtr.Zero)
                    s += " " + Marshal.PtrToStringAnsi(detail);
                throw new Exception(s);
            }

            static void error_codec(vpx_codec_ctx_t ctx, string s)
            {
                IntPtr detail = VPx.VPx.vpx_codec_error_detail(ref ctx);
                IntPtr err = VPx.VPx.vpx_codec_error(ref ctx);
                s = "VDe: " + s + ". " + Marshal.PtrToStringAnsi(err);
                if (detail != IntPtr.Zero)
                    s += " " + Marshal.PtrToStringAnsi(detail);
                //Console.Write(s);
            }
            public void Open(Voice.VoiceInfo info)
            {
                vpx_codec_dec_cfg cfg = new vpx_codec_dec_cfg();
                cfg.w = 100;
                cfg.h = 100;
                var iface = VPx.VPx.vpx_codec_vp8_dx();
                int flags = 0;
                var res = VPx.VPx.vpx_codec_dec_init(ref ctx, iface, ref cfg, flags);
                if (res != 0)
                {
                    die_codec(ctx, "vpx_codec_dec_init");
                }
                ready = true;
            }

            private IntPtr GetOutPtr(uint w, uint h)
            {
                if (outFramePtr == IntPtr.Zero || outFrame.w != w || outFrame.h != h)
                {
                    VPx.VPx.vpx_img_free(outFramePtr);
                    outFramePtr = VPx.VPx.vpx_img_alloc(IntPtr.Zero, formatToVPxImgFmt(this.OutputImageFormat), w, h, 1);
                    outFrame = (vpx_image_t)Marshal.PtrToStructure(outFramePtr, typeof(vpx_image_t));
                }
                return outFrame.planes[0];
            }

            IntPtr outFramePtr;
            vpx_image_t outFrame;

            IntPtr bufPtr;
            int bufPtrLen;
            public void Decode(byte[] buf)
            {
                lock (this)
                {
                    if (!ready)
                        return;
                    if (buf == null)
                        return;

                    if (bufPtr == IntPtr.Zero || bufPtrLen < buf.Length)
                    {
                        Marshal.FreeHGlobal(bufPtr);
                        bufPtr = Marshal.AllocHGlobal(buf.Length);
                        bufPtrLen = buf.Length;
                    }
                    Marshal.Copy(buf, 0, bufPtr, buf.Length);

                    if (0 != VPx.VPx.vpx_codec_decode(ref ctx, bufPtr, buf.Length, IntPtr.Zero, 0))
                    {
                        error_codec(ctx, "Failed to decode frame");
                    }

                    IntPtr iter;
                    IntPtr imgPtr;
                    while ((imgPtr = VPx.VPx.vpx_codec_get_frame(ref ctx, out iter)) != IntPtr.Zero)
                    {
                        vpx_image_t img = (vpx_image_t)Marshal.PtrToStructure(imgPtr, typeof(vpx_image_t));
                        var w = img.d_w;
                        var h = img.d_h;

                        var outPtr = IntPtr.Zero;

                        if (OutputImageBufferGetter != null)
                        {
                            outPtr = OutputImageBufferGetter((int)w, (int)h);
                            if (outPtr == IntPtr.Zero) // user's buffer not ready, skip image
                            {
                                continue;
                            }
                        }

                        var formatConvert = fromEncoderFormat[OutputImageFormat];
                        if (formatConvert != VPxCodec.EncoderFormatBypass)
                        {
                            // no buffer provided by user
                            if (outPtr == IntPtr.Zero)
                            {
                                outPtr = GetOutPtr(w, h);
                            }
                            var flipH = this.OutputImageFlip == Flip.Vertical ? -1 : 1;
                            formatConvert(
                                img.planes[0], img.stride[0],
                                img.planes[1], img.stride[1],
                                img.planes[2], img.stride[2],
                                outPtr, (int)w * 4,
                                (int)w, (int)h * flipH);

                            if (this.OnOutputImage != null)
                            {
                                OnOutputImage(outPtr, (int)w, (int)h, (int)w * 4);
                            }
                        }
                        else
                        {
                            // no buffer provided by user
                            if (outPtr == IntPtr.Zero)
                            {
                                if (this.OnOutputImage != null)
                                {
                                    OnOutputImage(img.planes[0], (int)w, (int)h, (int)w * 4);
                                }
                            }
                            else
                            {
                                throw new NotImplementedException("Output to provided buffer w/o format conversion is not supported.");
                            }

                            if (this.flip != Flip.None)
                            {
                                throw new NotImplementedException("Flip w/o format conversion is not supported.");
                            }
                        }
                    }
                }
            }

            public void Dispose()
            {
                lock (this)
                {
                    ready = false;
                    Marshal.FreeHGlobal(bufPtr);
                    bufPtr = IntPtr.Zero;
                    VPx.VPx.vpx_codec_destroy(ref ctx);
                    VPx.VPx.vpx_img_free(outFramePtr);
                    outFramePtr = IntPtr.Zero;
                }
            }
        }
    }
}