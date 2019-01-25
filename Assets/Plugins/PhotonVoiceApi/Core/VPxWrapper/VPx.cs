#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define DLL_IMPORT_INTERNAL
#endif

using System;
using System.Runtime.InteropServices;
using VPx.Codec;
using VPx.Decoder;
using VPx.Encoder;
using VPx.Image;

namespace VPx
{
    public class VPxConst
    {
        public const int VPX_CODEC_ABI_VERSION = 3 + ImageConst.VPX_IMAGE_ABI_VERSION;
    }
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct VpxInterface
    {
        string name;
        UInt32 fourcc;
        IntPtr codec_interface;
    };
    public class VPx
    {
#if DLL_IMPORT_INTERNAL
        const string lib_name = "__Internal";
#else
        const string lib_name = "vpx";
#endif

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern vpx_codec_err_t vpx_codec_enc_init_ver(ref vpx_codec_ctx_t ctx, IntPtr iface, ref vpx_codec_enc_cfg cfg, int flags, int ver);
        public static vpx_codec_err_t vpx_codec_enc_init(ref vpx_codec_ctx_t ctx, IntPtr iface, ref vpx_codec_enc_cfg cfg, int flags)
        {
            return vpx_codec_enc_init_ver(ref ctx, iface, ref cfg, flags, EncoderConst.VPX_ENCODER_ABI_VERSION);
        }

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern vpx_codec_err_t vpx_codec_enc_config_default(IntPtr iface, ref vpx_codec_enc_cfg cfg, UInt32 reserved);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_codec_vp8_cx();

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_codec_get_cx_data(ref vpx_codec_ctx_t ctx, out IntPtr iter);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern vpx_codec_err_t vpx_codec_encode(ref vpx_codec_ctx_t ctx, ref vpx_image_t img, Int64 pts, uint duration, int flags, uint deadline);


        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern vpx_codec_err_t vpx_codec_dec_init_ver(ref vpx_codec_ctx_t ctx, IntPtr iface, ref vpx_codec_dec_cfg cfg, int flags, int ver);
        public static vpx_codec_err_t vpx_codec_dec_init(ref vpx_codec_ctx_t ctx, IntPtr iface, ref vpx_codec_dec_cfg cfg, int flags)
        {
            return vpx_codec_dec_init_ver(ref ctx, iface, ref cfg, flags, DecoderConst.VPX_DECODER_ABI_VERSION);
        }

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_codec_vp8_dx();

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern vpx_codec_err_t vpx_codec_decode(ref vpx_codec_ctx_t ctx, IntPtr data, int data_sz, IntPtr user_priv, long deadline);


        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_codec_get_frame(ref vpx_codec_ctx_t ctx, out IntPtr iter);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_img_alloc(IntPtr img, vpx_img_fmt_t fmt, uint d_w, uint d_h, uint align);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void vpx_img_free(IntPtr img);


        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_codec_error(ref vpx_codec_ctx_t ctx);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr vpx_codec_error_detail(ref vpx_codec_ctx_t ctx);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern vpx_codec_err_t vpx_codec_destroy(ref vpx_codec_ctx_t ctx);

    }
}
