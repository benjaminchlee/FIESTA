using System;
using System.Runtime.InteropServices;

namespace VPx.Image
{    
    public struct ImageConst
    {
        public const int VPX_IMAGE_ABI_VERSION = 4;
        public const int VPX_IMG_FMT_PLANAR = 0x100;  /**< Image is a planar format. */
        public const int VPX_IMG_FMT_UV_FLIP = 0x200;  /**< V plane precedes U in memory. */
        public const int VPX_IMG_FMT_HAS_ALPHA = 0x400;  /**< Image has an alpha channel. */
        public const int VPX_IMG_FMT_HIGHBITDEPTH = 0x800;  /**< Image uses 16bit framebuffer. */
    }
    public enum vpx_img_fmt_t
    {      
        VPX_IMG_FMT_NONE,
        VPX_IMG_FMT_RGB24,   /**< 24 bit per pixel packed RGB */
        VPX_IMG_FMT_RGB32,   /**< 32 bit per pixel packed 0RGB */
        VPX_IMG_FMT_RGB565,  /**< 16 bit per pixel, 565 */
        VPX_IMG_FMT_RGB555,  /**< 16 bit per pixel, 555 */
        VPX_IMG_FMT_UYVY,    /**< UYVY packed YUV */
        VPX_IMG_FMT_YUY2,    /**< YUYV packed YUV */
        VPX_IMG_FMT_YVYU,    /**< YVYU packed YUV */
        VPX_IMG_FMT_BGR24,   /**< 24 bit per pixel packed BGR */
        VPX_IMG_FMT_RGB32_LE, /**< 32 bit packed BGR0 */
        VPX_IMG_FMT_ARGB,     /**< 32 bit packed ARGB, alpha=255 */
        VPX_IMG_FMT_ARGB_LE,  /**< 32 bit packed BGRA, alpha=255 */
        VPX_IMG_FMT_RGB565_LE,  /**< 16 bit per pixel, gggbbbbb rrrrrggg */
        VPX_IMG_FMT_RGB555_LE,  /**< 16 bit per pixel, gggbbbbb 0rrrrrgg */
        VPX_IMG_FMT_YV12 = ImageConst.VPX_IMG_FMT_PLANAR | ImageConst.VPX_IMG_FMT_UV_FLIP | 1, /**< planar YVU */
        VPX_IMG_FMT_I420 = ImageConst.VPX_IMG_FMT_PLANAR | 2,
        VPX_IMG_FMT_VPXYV12 = ImageConst.VPX_IMG_FMT_PLANAR | ImageConst.VPX_IMG_FMT_UV_FLIP | 3, /** < planar 4:2:0 format with vpx color space */
        VPX_IMG_FMT_VPXI420 = ImageConst.VPX_IMG_FMT_PLANAR | 4,
        VPX_IMG_FMT_I422 = ImageConst.VPX_IMG_FMT_PLANAR | 5,
        VPX_IMG_FMT_I444 = ImageConst.VPX_IMG_FMT_PLANAR | 6,
        VPX_IMG_FMT_I440 = ImageConst.VPX_IMG_FMT_PLANAR | 7,
        VPX_IMG_FMT_444A = ImageConst.VPX_IMG_FMT_PLANAR | ImageConst.VPX_IMG_FMT_HAS_ALPHA | 6,
        VPX_IMG_FMT_I42016 = VPX_IMG_FMT_I420 | ImageConst.VPX_IMG_FMT_HIGHBITDEPTH,
        VPX_IMG_FMT_I42216 = VPX_IMG_FMT_I422 | ImageConst.VPX_IMG_FMT_HIGHBITDEPTH,
        VPX_IMG_FMT_I44416 = VPX_IMG_FMT_I444 | ImageConst.VPX_IMG_FMT_HIGHBITDEPTH,
        VPX_IMG_FMT_I44016 = VPX_IMG_FMT_I440 | ImageConst.VPX_IMG_FMT_HIGHBITDEPTH
    }

    public enum vpx_color_space_t
    {
        // ...
    }
    public enum vpx_color_range_t
    {
        // ...
    }

    // [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    // public class vpx_image_t
    // When PtrToStructure called for classes, mono throws System.ExecutionEngineException: Attempting to JIT compile method '(wrapper unknown) VPx.Image.vpx_image_t:PtrToStructure (intptr,object)' while running with --aot-only.
    public struct vpx_image_t
    {
        public vpx_img_fmt_t fmt; /**< Image Format */
        public vpx_color_space_t cs; /**< Color Space */
        public vpx_color_range_t range; /**< Color Range */

        /* Image storage dimensions */
        public uint w;           /**< Stored image width */
        public uint h;           /**< Stored image height */
        public uint bit_depth;   /**< Stored image bit-depth */

        /* Image display dimensions */
        public uint d_w;   /**< Displayed image width */
        public uint d_h;   /**< Displayed image height */

        /* Image intended rendering dimensions */
        public uint r_w;   /**< Intended rendering image width */
        public uint r_h;   /**< Intended rendering image height */

        /* Chroma subsampling info */
        public uint x_chroma_shift;   /**< subsampling order, X */
        public uint y_chroma_shift;   /**< subsampling order, Y */

        /* Image data pointers. */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]        
        public IntPtr[] planes;  /**< pointer to the top left pixel for each plane */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] stride;  /**< stride between rows for each plane */
        public int bps; /**< bits per sample (for packed formats) */

        /* The following member may be set by the application to associate data
         * with this image.
         */
        public IntPtr user_priv; /**< may be set by the application to associate data
                         *   with this image. */

        /* The following members should be treated as private. */
        public IntPtr img_data;       /**< private */
        public int img_data_owner; /**< private */
        public int self_allocd;    /**< private */

        public IntPtr fb_priv; /**< Frame buffer data associated with the image. */
    }
}
