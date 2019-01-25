using System;
using System.Runtime.InteropServices;

namespace VPx.Codec
{
    public enum vpx_codec_err_t
    {
        /*!\brief Operation completed without error */
        VPX_CODEC_OK,

        /*!\brief Unspecified error */
        VPX_CODEC_ERROR,

        /*!\brief Memory operation failed */
        VPX_CODEC_MEM_ERROR,

        /*!\brief ABI version mismatch */
        VPX_CODEC_ABI_MISMATCH,

        /*!\brief Algorithm does not have required capability */
        VPX_CODEC_INCAPABLE,

        /*!\brief The given bitstream is not supported.
         *
         * The bitstream was unable to be parsed at the highest level. The decoder
         * is unable to proceed. This error \ref SHOULD be treated as fatal to the
         * stream. */
        VPX_CODEC_UNSUP_BITSTREAM,

        /*!\brief Encoded bitstream uses an unsupported feature
         *
         * The decoder does not implement a feature required by the encoder. This
         * return code should only be used for features that prevent future
         * pictures from being properly decoded. This error \ref MAY be treated as
         * fatal to the stream or \ref MAY be treated as fatal to the current GOP.
         */
        VPX_CODEC_UNSUP_FEATURE,

        /*!\brief The coded data for this stream is corrupt or incomplete
         *
         * There was a problem decoding the current frame.  This return code
         * should only be used for failures that prevent future pictures from
         * being properly decoded. This error \ref MAY be treated as fatal to the
         * stream or \ref MAY be treated as fatal to the current GOP. If decoding
         * is continued for the current GOP, artifacts may be present.
         */
        VPX_CODEC_CORRUPT_FRAME,

        /*!\brief An application-supplied parameter is not valid.
         *
         */
        VPX_CODEC_INVALID_PARAM,

        /*!\brief An iterator reached the end of list.
         *
         */
        VPX_CODEC_LIST_END
    };    

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct vpx_codec_ctx_t
    {
        //[MarshalAs(UnmanagedType.LPStr)]
        //TODO: string
        public IntPtr name;        /**< Printable interface name */
        public IntPtr iface;       /**< Interface pointers */
        public vpx_codec_err_t err;         /**< Last returned error */
        //TODO: string
        public IntPtr err_detail;  /**< Detailed info, if available */
        public int init_flags;  /**< Flags passed at init time */
        public IntPtr config;      /**< Configuration pointer aliasing union */
        public IntPtr priv;        /**< Algorithm private storage */
    };        
    
}
