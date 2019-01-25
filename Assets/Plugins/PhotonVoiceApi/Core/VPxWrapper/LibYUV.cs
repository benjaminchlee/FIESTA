#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define DLL_IMPORT_INTERNAL
#endif

using System;
using System.Runtime.InteropServices;

namespace LibYUV
{
    public class LibYUV
    {
        public enum FilterMode
        {
            kFilterNone = 0,      // Point sample; Fastest.
            kFilterLinear = 1,    // Filter horizontally only.
            kFilterBilinear = 2,  // Faster than box, but lower quality scaling down.
            kFilterBox = 3        // Highest quality.
        }

        public enum RotationMode
        {
            kRotate0 = 0,      // No rotation.
            kRotate90 = 90,    // Rotate 90 degrees clockwise.
            kRotate180 = 180,  // Rotate 180 degrees.
            kRotate270 = 270,  // Rotate 270 degrees clockwise.
        }

#if DLL_IMPORT_INTERNAL
        const string lib_name = "__Internal";
#else
        const string lib_name = "libyuv";
#endif

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ARGBToI420(IntPtr src_frame, int src_stride_frame, IntPtr dst_y, int dst_stride_y, IntPtr dst_u, int dst_stride_u, IntPtr dst_v, int dst_stride_v, int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int BGRAToI420(IntPtr src_frame, int src_stride_frame, IntPtr dst_y, int dst_stride_y, IntPtr dst_u, int dst_stride_u, IntPtr dst_v, int dst_stride_v, int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int ABGRToI420(IntPtr src_frame, int src_stride_frame, IntPtr dst_y, int dst_stride_y, IntPtr dst_u, int dst_stride_u, IntPtr dst_v, int dst_stride_v, int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int RGBAToI420(IntPtr src_frame, int src_stride_frame, IntPtr dst_y, int dst_stride_y, IntPtr dst_u, int dst_stride_u, IntPtr dst_v, int dst_stride_v, int width, int height);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420ToARGB(IntPtr src_y, int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, IntPtr dst, int dst_stride, int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420ToBGRA(IntPtr src_y, int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, IntPtr dst, int dst_stride, int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420ToABGR(IntPtr src_y, int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, IntPtr dst, int dst_stride, int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420ToRGBA(IntPtr src_y, int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, IntPtr dst, int dst_stride, int width, int height);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int Android420ToI420(IntPtr src_y,int src_stride_y, IntPtr src_u, int src_stride_u, IntPtr src_v, int src_stride_v, int pixel_stride_uv, IntPtr dst_y, int dst_stride_y, IntPtr dst_u, int dst_stride_u, IntPtr dst_v, int dst_stride_v,int width,int height);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420Scale(
            IntPtr src_y, int src_stride_y,
            IntPtr src_u, int src_stride_u,
            IntPtr src_v, int src_stride_v,
            int src_width, int src_height,
            IntPtr dst_y, int dst_stride_y,
            IntPtr dst_u, int dst_stride_u,
            IntPtr dst_v, int dst_stride_v,
            int dst_width, int dst_height,
            FilterMode filtering);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420Rotate(
            IntPtr src_y, int src_stride_y,
            IntPtr src_u, int src_stride_u,
            IntPtr src_v, int src_stride_v,
            IntPtr dst_y, int dst_stride_y,
            IntPtr dst_u, int dst_stride_u,
            IntPtr dst_v, int dst_stride_v,
            int src_width, int src_height,
            RotationMode mode);

        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420Copy(
            IntPtr src_y, int src_stride_y,
            IntPtr src_u, int src_stride_u,
            IntPtr src_v, int src_stride_v,
            IntPtr dst_y, int dst_stride_y,
            IntPtr dst_u, int dst_stride_u,
            IntPtr dst_v, int dst_stride_v,
            int width, int height);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int I420Mirror(
                    IntPtr src_y, int src_stride_y,
                    IntPtr src_u, int src_stride_u,
                    IntPtr src_v, int src_stride_v,
                    IntPtr dst_y, int dst_stride_y,
                    IntPtr dst_u, int dst_stride_u,
                    IntPtr dst_v, int dst_stride_v,
                    int width, int height);
    }
}
