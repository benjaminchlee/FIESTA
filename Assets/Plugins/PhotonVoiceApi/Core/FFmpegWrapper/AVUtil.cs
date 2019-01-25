#define FF_API_FRAME_QP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFmpegWrapper.AVUtil
{
    public struct AVUtilConst
    {
        public const Int64 AV_NOPTS_VALUE = Int64.MinValue; //((int64_t)UINT64_C(0x8000000000000000))

        public const int AV_TIME_BASE = 1000000;

        public const int AV_CH_FRONT_LEFT = 0x00000001;
        public const int AV_CH_FRONT_RIGHT = 0x00000002;
        public const int AV_CH_FRONT_CENTER = 0x00000004;
        public const int AV_CH_LOW_FREQUENCY = 0x00000008;
        public const int AV_CH_BACK_LEFT = 0x00000010;
        public const int AV_CH_BACK_RIGHT = 0x00000020;
        public const int AV_CH_FRONT_LEFT_OF_CENTER = 0x00000040;
        public const int AV_CH_FRONT_RIGHT_OF_CENTER = 0x00000080;
        public const int AV_CH_BACK_CENTER = 0x00000100;
        public const int AV_CH_SIDE_LEFT = 0x00000200;
        public const int AV_CH_SIDE_RIGHT = 0x00000400;
        public const int AV_CH_TOP_CENTER = 0x00000800;
        public const int AV_CH_TOP_FRONT_LEFT = 0x00001000;
        public const int AV_CH_TOP_FRONT_CENTER = 0x00002000;
        public const int AV_CH_TOP_FRONT_RIGHT = 0x00004000;
        public const int AV_CH_TOP_BACK_LEFT = 0x00008000;
        public const int AV_CH_TOP_BACK_CENTER = 0x00010000;
        public const int AV_CH_TOP_BACK_RIGHT = 0x00020000;
        public const int AV_CH_STEREO_LEFT = 0x20000000;
        public const int AV_CH_STEREO_RIGHT = 0x40000000;
        public const ulong AV_CH_WIDE_LEFT = 0x0000000080000000;
        public const ulong AV_CH_WIDE_RIGHT = 0x0000000100000000;
        public const ulong AV_CH_SURROUND_DIRECT_LEFT = 0x0000000200000000;
        public const ulong AV_CH_SURROUND_DIRECT_RIGHT = 0x0000000400000000;
        public const ulong AV_CH_LOW_FREQUENCY_2 = 0x0000000800000000;

        public const int AV_CH_LAYOUT_MONO = (AV_CH_FRONT_CENTER);
        public const int AV_CH_LAYOUT_STEREO = (AV_CH_FRONT_LEFT | AV_CH_FRONT_RIGHT);
        public const int AV_CH_LAYOUT_2POINT1 = (AV_CH_LAYOUT_STEREO | AV_CH_LOW_FREQUENCY);
        public const int AV_CH_LAYOUT_2_1 = (AV_CH_LAYOUT_STEREO | AV_CH_BACK_CENTER);
        public const int AV_CH_LAYOUT_SURROUND = (AV_CH_LAYOUT_STEREO | AV_CH_FRONT_CENTER);
        public const int AV_CH_LAYOUT_3POINT1 = (AV_CH_LAYOUT_SURROUND | AV_CH_LOW_FREQUENCY);
        public const int AV_CH_LAYOUT_4POINT0 = (AV_CH_LAYOUT_SURROUND | AV_CH_BACK_CENTER);
        public const int AV_CH_LAYOUT_4POINT1 = (AV_CH_LAYOUT_4POINT0 | AV_CH_LOW_FREQUENCY);
        public const int AV_CH_LAYOUT_2_2 = (AV_CH_LAYOUT_STEREO | AV_CH_SIDE_LEFT | AV_CH_SIDE_RIGHT);
        public const int AV_CH_LAYOUT_QUAD = (AV_CH_LAYOUT_STEREO | AV_CH_BACK_LEFT | AV_CH_BACK_RIGHT);
        public const int AV_CH_LAYOUT_5POINT0 = (AV_CH_LAYOUT_SURROUND | AV_CH_SIDE_LEFT | AV_CH_SIDE_RIGHT);
        public const int AV_CH_LAYOUT_5POINT1 = (AV_CH_LAYOUT_5POINT0 | AV_CH_LOW_FREQUENCY);
        public const int AV_CH_LAYOUT_5POINT0_BACK = (AV_CH_LAYOUT_SURROUND | AV_CH_BACK_LEFT | AV_CH_BACK_RIGHT);
        public const int AV_CH_LAYOUT_5POINT1_BACK = (AV_CH_LAYOUT_5POINT0_BACK | AV_CH_LOW_FREQUENCY);
        public const int AV_CH_LAYOUT_6POINT0 = (AV_CH_LAYOUT_5POINT0 | AV_CH_BACK_CENTER);
        public const int AV_CH_LAYOUT_6POINT0_FRONT = (AV_CH_LAYOUT_2_2 | AV_CH_FRONT_LEFT_OF_CENTER | AV_CH_FRONT_RIGHT_OF_CENTER);
        public const int AV_CH_LAYOUT_HEXAGONAL = (AV_CH_LAYOUT_5POINT0_BACK | AV_CH_BACK_CENTER);
        public const int AV_CH_LAYOUT_6POINT1 = (AV_CH_LAYOUT_5POINT1 | AV_CH_BACK_CENTER);
        public const int AV_CH_LAYOUT_6POINT1_BACK = (AV_CH_LAYOUT_5POINT1_BACK | AV_CH_BACK_CENTER);
        public const int AV_CH_LAYOUT_6POINT1_FRONT = (AV_CH_LAYOUT_6POINT0_FRONT | AV_CH_LOW_FREQUENCY);
        public const int AV_CH_LAYOUT_7POINT0 = (AV_CH_LAYOUT_5POINT0 | AV_CH_BACK_LEFT | AV_CH_BACK_RIGHT);
        public const int AV_CH_LAYOUT_7POINT0_FRONT = (AV_CH_LAYOUT_5POINT0 | AV_CH_FRONT_LEFT_OF_CENTER | AV_CH_FRONT_RIGHT_OF_CENTER);
        public const int AV_CH_LAYOUT_7POINT1 = (AV_CH_LAYOUT_5POINT1 | AV_CH_BACK_LEFT | AV_CH_BACK_RIGHT);
        public const int AV_CH_LAYOUT_7POINT1_WIDE = (AV_CH_LAYOUT_5POINT1 | AV_CH_FRONT_LEFT_OF_CENTER | AV_CH_FRONT_RIGHT_OF_CENTER);
        public const int AV_CH_LAYOUT_7POINT1_WIDE_BACK = (AV_CH_LAYOUT_5POINT1_BACK | AV_CH_FRONT_LEFT_OF_CENTER | AV_CH_FRONT_RIGHT_OF_CENTER);
        public const int AV_CH_LAYOUT_OCTAGONAL = (AV_CH_LAYOUT_5POINT0 | AV_CH_BACK_LEFT | AV_CH_BACK_CENTER | AV_CH_BACK_RIGHT);
        public const ulong AV_CH_LAYOUT_HEXADECAGONAL = (AV_CH_LAYOUT_OCTAGONAL | AV_CH_WIDE_LEFT | AV_CH_WIDE_RIGHT | AV_CH_TOP_BACK_LEFT | AV_CH_TOP_BACK_RIGHT | AV_CH_TOP_BACK_CENTER | AV_CH_TOP_FRONT_CENTER | AV_CH_TOP_FRONT_LEFT | AV_CH_TOP_FRONT_RIGHT);
        public const int AV_CH_LAYOUT_STEREO_DOWNMIX = (AV_CH_STEREO_LEFT | AV_CH_STEREO_RIGHT);

        // log level
        public const int AV_LOG_QUIET = -8;
        public const int AV_LOG_PANIC = 0;
        public const int AV_LOG_FATAL = 8;
        public const int AV_LOG_ERROR = 16;
        public const int AV_LOG_WARNING = 24;
        public const int AV_LOG_INFO = 32;
        public const int AV_LOG_VERBOSE = 40;
        public const int AV_LOG_DEBUG = 48;
        public const int AV_LOG_TRACE = 56;

        // log flags
        public const int AV_LOG_SKIP_REPEATED = 1;
        public const int AV_LOG_PRINT_LEVEL = 2;
    }
    public enum AVMediaType
    {
        AVMEDIA_TYPE_UNKNOWN = -1,  ///< Usually treated as AVMEDIA_TYPE_DATA
        AVMEDIA_TYPE_VIDEO,
        AVMEDIA_TYPE_AUDIO,
        AVMEDIA_TYPE_DATA,          ///< Opaque data information usually continuous
        AVMEDIA_TYPE_SUBTITLE,
        AVMEDIA_TYPE_ATTACHMENT,    ///< Opaque data information usually sparse
        AVMEDIA_TYPE_NB
    };

    public enum AVPictureType
    {
        AV_PICTURE_TYPE_NONE = 0, ///< Undefined
        AV_PICTURE_TYPE_I,     ///< Intra
        AV_PICTURE_TYPE_P,     ///< Predicted
        AV_PICTURE_TYPE_B,     ///< Bi-dir predicted
        AV_PICTURE_TYPE_S,     ///< S(GMC)-VOP MPEG-4
        AV_PICTURE_TYPE_SI,    ///< Switching Intra
        AV_PICTURE_TYPE_SP,    ///< Switching Predicted
        AV_PICTURE_TYPE_BI,    ///< BI type
    }

    public enum AVFrameSideDataType
    {
        AV_FRAME_DATA_PANSCAN,
        AV_FRAME_DATA_A53_CC,
        AV_FRAME_DATA_STEREO3D,
        AV_FRAME_DATA_MATRIXENCODING,
        AV_FRAME_DATA_DOWNMIX_INFO,
        AV_FRAME_DATA_REPLAYGAIN,
        AV_FRAME_DATA_DISPLAYMATRIX,
        AV_FRAME_DATA_AFD,
        AV_FRAME_DATA_MOTION_VECTORS,
        AV_FRAME_DATA_SKIP_SAMPLES,
        AV_FRAME_DATA_AUDIO_SERVICE_TYPE,
        AV_FRAME_DATA_MASTERING_DISPLAY_METADATA,
        AV_FRAME_DATA_GOP_TIMECODE
    };

    public enum AVSampleFormat
    {
        AV_SAMPLE_FMT_NONE = -1,
        AV_SAMPLE_FMT_U8,          ///< unsigned 8 bits
        AV_SAMPLE_FMT_S16,         ///< signed 16 bits
        AV_SAMPLE_FMT_S32,         ///< signed 32 bits
        AV_SAMPLE_FMT_FLT,         ///< float
        AV_SAMPLE_FMT_DBL,         ///< double

        AV_SAMPLE_FMT_U8P,         ///< unsigned 8 bits, planar
        AV_SAMPLE_FMT_S16P,        ///< signed 16 bits, planar
        AV_SAMPLE_FMT_S32P,        ///< signed 32 bits, planar
        AV_SAMPLE_FMT_FLTP,        ///< float, planar
        AV_SAMPLE_FMT_DBLP,        ///< double, planar

        AV_SAMPLE_FMT_NB           ///< Number of sample formats. DO NOT USE if linking dynamically
    };

    public enum AVColorPrimaries
    {
        AVCOL_PRI_RESERVED0 = 0,
        AVCOL_PRI_BT709 = 1,  ///< also ITU-R BT1361 / IEC 61966-2-4 / SMPTE RP177 Annex B
        AVCOL_PRI_UNSPECIFIED = 2,
        AVCOL_PRI_RESERVED = 3,
        AVCOL_PRI_BT470M = 4,  ///< also FCC Title 47 Code of Federal Regulations 73.682 (a)(20)

        AVCOL_PRI_BT470BG = 5,  ///< also ITU-R BT601-6 625 / ITU-R BT1358 625 / ITU-R BT1700 625 PAL & SECAM
        AVCOL_PRI_SMPTE170M = 6,  ///< also ITU-R BT601-6 525 / ITU-R BT1358 525 / ITU-R BT1700 NTSC
        AVCOL_PRI_SMPTE240M = 7,  ///< functionally identical to above
        AVCOL_PRI_FILM = 8,  ///< colour filters using Illuminant C
        AVCOL_PRI_BT2020 = 9,  ///< ITU-R BT2020
        AVCOL_PRI_SMPTEST428_1 = 10, ///< SMPTE ST 428-1 (CIE 1931 XYZ)
        AVCOL_PRI_NB,               ///< Not part of ABI
    };
    public enum AVColorTransferCharacteristic
    {
        AVCOL_TRC_RESERVED0 = 0,
        AVCOL_TRC_BT709 = 1,  ///< also ITU-R BT1361
        AVCOL_TRC_UNSPECIFIED = 2,
        AVCOL_TRC_RESERVED = 3,
        AVCOL_TRC_GAMMA22 = 4,  ///< also ITU-R BT470M / ITU-R BT1700 625 PAL & SECAM
        AVCOL_TRC_GAMMA28 = 5,  ///< also ITU-R BT470BG
        AVCOL_TRC_SMPTE170M = 6,  ///< also ITU-R BT601-6 525 or 625 / ITU-R BT1358 525 or 625 / ITU-R BT1700 NTSC
        AVCOL_TRC_SMPTE240M = 7,
        AVCOL_TRC_LINEAR = 8,  ///< "Linear transfer characteristics"
        AVCOL_TRC_LOG = 9,  ///< "Logarithmic transfer characteristic (100:1 range)"
        AVCOL_TRC_LOG_SQRT = 10, ///< "Logarithmic transfer characteristic (100 * Sqrt(10) : 1 range)"
        AVCOL_TRC_IEC61966_2_4 = 11, ///< IEC 61966-2-4
        AVCOL_TRC_BT1361_ECG = 12, ///< ITU-R BT1361 Extended Colour Gamut
        AVCOL_TRC_IEC61966_2_1 = 13, ///< IEC 61966-2-1 (sRGB or sYCC)
        AVCOL_TRC_BT2020_10 = 14, ///< ITU-R BT2020 for 10-bit system
        AVCOL_TRC_BT2020_12 = 15, ///< ITU-R BT2020 for 12-bit system
        AVCOL_TRC_SMPTEST2084 = 16, ///< SMPTE ST 2084 for 10-, 12-, 14- and 16-bit systems
        AVCOL_TRC_SMPTEST428_1 = 17, ///< SMPTE ST 428-1
        AVCOL_TRC_ARIB_STD_B67 = 18, ///< ARIB STD-B67, known as "Hybrid log-gamma"
        AVCOL_TRC_NB,                ///< Not part of ABI
    };
    public enum AVColorSpace
    {
        AVCOL_SPC_RGB = 0,  ///< order of coefficients is actually GBR, also IEC 61966-2-1 (sRGB)
        AVCOL_SPC_BT709 = 1,  ///< also ITU-R BT1361 / IEC 61966-2-4 xvYCC709 / SMPTE RP177 Annex B
        AVCOL_SPC_UNSPECIFIED = 2,
        AVCOL_SPC_RESERVED = 3,
        AVCOL_SPC_FCC = 4,  ///< FCC Title 47 Code of Federal Regulations 73.682 (a)(20)
        AVCOL_SPC_BT470BG = 5,  ///< also ITU-R BT601-6 625 / ITU-R BT1358 625 / ITU-R BT1700 625 PAL & SECAM / IEC 61966-2-4 xvYCC601
        AVCOL_SPC_SMPTE170M = 6,  ///< also ITU-R BT601-6 525 / ITU-R BT1358 525 / ITU-R BT1700 NTSC
        AVCOL_SPC_SMPTE240M = 7,  ///< functionally identical to above
        AVCOL_SPC_YCOCG = 8,  ///< Used by Dirac / VC-2 and H.264 FRext, see ITU-T SG16
        AVCOL_SPC_BT2020_NCL = 9,  ///< ITU-R BT2020 non-constant luminance system
        AVCOL_SPC_BT2020_CL = 10, ///< ITU-R BT2020 constant luminance system
        AVCOL_SPC_NB,               ///< Not part of ABI
    };

    public enum AVColorRange
    {
        AVCOL_RANGE_UNSPECIFIED = 0,
        AVCOL_RANGE_MPEG = 1, ///< the normal 219*2^(n-8) "MPEG" YUV ranges
        AVCOL_RANGE_JPEG = 2, ///< the normal     2^n-1   "JPEG" YUV ranges
        AVCOL_RANGE_NB,              ///< Not part of ABI
    };

    public enum AVChromaLocation
    {
        AVCHROMA_LOC_UNSPECIFIED = 0,
        AVCHROMA_LOC_LEFT = 1, ///< MPEG-2/4 4:2:0, H.264 default for 4:2:0
        AVCHROMA_LOC_CENTER = 2, ///< MPEG-1 4:2:0, JPEG 4:2:0, H.263 4:2:0
        AVCHROMA_LOC_TOPLEFT = 3, ///< ITU-R 601, SMPTE 274M 296M S314M(DV 4:1:1), mpeg2 4:2:2
        AVCHROMA_LOC_TOP = 4,
        AVCHROMA_LOC_BOTTOMLEFT = 5,
        AVCHROMA_LOC_BOTTOM = 6,
        AVCHROMA_LOC_NB,              ///< Not part of ABI
    };

    public struct AVRational
    {
        public int num; ///< numerator
        public int den; ///< denominator
        public AVRational(int num, int den) 
        {
            this.num = num;
            this.den = den;
        }
        public override string ToString()
        {
            return this.num + "/" + this.den;
        }
    }

    public struct AVFrameSideData
    {
        public AVFrameSideDataType type;
        public IntPtr data;
        public int size;
        public IntPtr metadata; // AVDictionary* metadata;
        public IntPtr buf; // AVBufferRef* buf;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AVFrame
    {
        public const int AV_NUM_DATA_POINTERS = 8;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AV_NUM_DATA_POINTERS)]
        public IntPtr[] data; //uint8_t* data[AV_NUM_DATA_POINTERS];

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AV_NUM_DATA_POINTERS)]
        public int[] linesize;

        IntPtr extended_data; //uint8_t** extended_data;
        public int width, height;

        public int nb_samples;

        public int format;

        public int key_frame;

        public AVPictureType pict_type;

        public AVRational sample_aspect_ratio;

        public Int64 pts;

        public Int64 pkt_pts;

        public Int64 pkt_dts;

        public int coded_picture_number;

        public int display_picture_number;

        public int quality;

        public IntPtr opaque;

        public int repeat_pict;

        public int interlaced_frame;

        public int top_field_first;

        public int palette_has_changed;

        public Int64 reordered_opaque;

        public int sample_rate;

        public UInt64 channel_layout;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AV_NUM_DATA_POINTERS)]
        public IntPtr[] buf;// AVBufferRef* buf[AV_NUM_DATA_POINTERS];

        public IntPtr extended_buf; //AVBufferRef** extended_buf;

        public int nb_extended_buf;

        public IntPtr side_data;// AVFrameSideData** side_data;

        public int nb_side_data;

        const int AV_FRAME_FLAG_CORRUPT = (1 << 0);

        public int flags;

        public AVColorRange color_range;

        public AVColorPrimaries color_primaries;

        public AVColorTransferCharacteristic color_trc;

        public AVColorSpace colorspace;

        public AVChromaLocation chroma_location;

        public Int64 best_effort_timestamp;

        public Int64 pkt_pos;

        public Int64 pkt_duration;

        public IntPtr metadata;// AVDictionary* metadata;

        public int decode_error_flags;

        const int FF_DECODE_ERROR_INVALID_BITSTREAM = 1;
        const int FF_DECODE_ERROR_MISSING_REFERENCE = 2;

        public int channels;

        public int pkt_size;

#if FF_API_FRAME_QP
        // attribute_deprecated
        public IntPtr qscale_table;

        // attribute_deprecated
        public int qstride;

        // attribute_deprecated
        public int qscale_type;

        public IntPtr qp_table_buf; // AVBufferRef *qp_table_buf;
#endif

        public IntPtr hw_frames_ctx; // AVBufferRef* hw_frames_ctx;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AVUtilMethods
    {
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_opt_set(IntPtr obj, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string val, int search_flags);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_frame_alloc();
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_image_alloc(IntPtr[] pointers /*[4]*/, int[] linesizes /*[4]*/ , int w, int h, AVCodec.AVPixelFormat pix_fmt, int align);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_free(IntPtr ptr);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_freep(ref IntPtr ptr); 
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_frame_free(ref IntPtr frame); // (AVFrame** frame);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_get_channel_layout_nb_channels(UInt64 channel_layout);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_strerror(int errnum, byte[] errbuf, uint errbuf_size);
        public static string av_err2str(int errnum)
        {
            var errbuf = new byte[1024];
            if (av_strerror(errnum, errbuf, (uint)errbuf.Length) == 0)
            {
                return new ASCIIEncoding().GetString(errbuf);
            }
            else
                return "";
        }

        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_compare_ts(Int64 ts_a, AVRational tb_a, Int64 ts_b, AVRational tb_b);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_frame_is_writable(IntPtr frame);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_frame_make_writable(IntPtr frame);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_frame_get_buffer(IntPtr frame, int align);
        public delegate void LogCallbackDelegate(IntPtr avcl, int level, IntPtr fmt, ref IntPtr vl);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_log_set_callback(LogCallbackDelegate callback);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_log_format_line2(IntPtr ptr, int level, IntPtr fmt, ref IntPtr vl, IntPtr line, int line_size, ref int print_prefix);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_log_get_level();
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_log_set_level(int level);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_log_set_flags(int arg);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_log_get_flags();
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_get_sample_fmt_name(AVSampleFormat sample_fmt);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_samples_get_buffer_size(IntPtr linesize /* int* */, int nb_channels, int nb_samples, AVSampleFormat sample_fmt, int align);
        [DllImport("avutil-55", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_get_bytes_per_sample(AVSampleFormat sample_fmt);
    }
}
