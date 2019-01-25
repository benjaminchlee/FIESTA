using FFmpegWrapper.AVCodec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFmpegWrapper.SWScale
{
    public struct SWScaleFlags
    {
        public const int SWS_FAST_BILINEAR = 1;
        public const int SWS_BILINEAR = 2;
        public const int SWS_BICUBIC = 4;
        public const int SWS_X = 8;
        public const int SWS_POINT = 0x10;
        public const int SWS_AREA = 0x20;
        public const int SWS_BICUBLIN = 0x40;
        public const int SWS_GAUSS = 0x80;
        public const int SWS_SINC = 0x100;
        public const int SWS_LANCZOS = 0x200;
        public const int SWS_SPLINE = 0x400;

    }
    public class SWScaleMethods
    {
        [DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr sws_getContext(int srcW, int srcH, AVPixelFormat srcFormat, int dstW, int dstH, AVPixelFormat dstFormat, int flags, IntPtr srcFilter, IntPtr dstFilter, IntPtr param);

        [DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
        public static extern int sws_scale(IntPtr swsContext, IntPtr[] srcSlice, IntPtr srcStride, int srcSliceY, int srcSliceH, IntPtr[] dst, IntPtr dstStride);
        [DllImport("swscale-4", CallingConvention = CallingConvention.Cdecl)]
        public static extern void sws_freeContext(IntPtr swsContext);
        

    }
}
