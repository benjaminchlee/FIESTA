#define FF_API_OLD_OPEN_CALLBACKS
#define FF_API_LAVF_AVCTX
#define FF_API_LAVF_FRAC
#define FF_API_LAVF_FMT_RAWPICTURE

using FFmpegWrapper.AVCodec;
using FFmpegWrapper.AVUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFmpegWrapper.AVFormat
{
    public struct AVFormatConst
    {
        public const int AVFMT_NOFILE = 0x0001;
        public const int AVFMT_NEEDNUMBER = 0x0002;
        public const int AVFMT_SHOW_IDS = 0x0008;
#if FF_API_LAVF_FMT_RAWPICTURE
        public const int AVFMT_RAWPICTURE = 0x0020;
#endif
        public const int AVFMT_GLOBALHEADER = 0x0040;
        public const int AVFMT_NOTIMESTAMPS = 0x0080;
        public const int AVFMT_GENERIC_INDEX = 0x0100;
        public const int AVFMT_TS_DISCONT = 0x0200;
        public const int AVFMT_VARIABLE_FPS = 0x0400;
        public const int AVFMT_NODIMENSIONS = 0x0800;
        public const int AVFMT_NOSTREAMS = 0x1000;
        public const int AVFMT_NOBINSEARCH = 0x2000;
        public const int AVFMT_NOGENSEARCH = 0x4000;
        public const int AVFMT_NO_BYTE_SEEK = 0x8000;
        public const int AVFMT_ALLOW_FLUSH = 0x10000;
        public const int AVFMT_TS_NONSTRICT = 0x20000;
        public const int AVFMT_TS_NEGATIVE = 0x40000;
        public const int AVFMT_SEEK_TO_PTS = 0x4000000;

        public const int AVIO_FLAG_READ = 1;
        public const int AVIO_FLAG_WRITE = 2;
        public const int AVIO_FLAG_READ_WRITE = (AVIO_FLAG_READ | AVIO_FLAG_WRITE);

    }
    public enum AVDurationEstimationMethod
    {
        AVFMT_DURATION_FROM_PTS,    ///< Duration accurately estimated from PTSes
        AVFMT_DURATION_FROM_STREAM, ///< Duration estimated from a stream with a known duration
        AVFMT_DURATION_FROM_BITRATE ///< Duration estimated from bitrate (less accurate)
    };
#if FF_API_LAVF_FRAC
    public struct AVFrac
    {
        Int64 val, num, den;
    }
#endif

    [StructLayout(LayoutKind.Sequential)]
    public class AVOutputFormat
    {
        public IntPtr name; // const char* name;
        public IntPtr long_name; // const char* long_name;
        public IntPtr mime_type; // const char* mime_type;
        public IntPtr extensions; // const char* extensions;
        public AVCodecID audio_codec;    /**< default audio codec */
        public AVCodecID video_codec;    /**< default video codec */
        public AVCodecID subtitle_codec; /**< default subtitle codec */
        public int flags;
        public IntPtr codec_tag; // const struct AVCodecTag * const * codec_tag;
        public IntPtr priv_class; // const AVClass* priv_class; 

        /*****************************************************************
         * No fields below this line are part of the public API. They
         * may not be used outside of libavformat and can be changed and
         * removed at will.
         * New public fields should be added right above.
         *****************************************************************
         */

        // ...
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AVFormatContext
    {
        public IntPtr av_class; // const AVClass* av_class;
        public IntPtr iformat; // struct AVInputFormat *iformat;
        public IntPtr oformat; // struct AVOutputFormat *oformat;
        public IntPtr priv_data; // void* priv_data;
        public IntPtr pb; // AVIOContext* pb;
        public int ctx_flags;
        public uint nb_streams;
        public IntPtr streams; // AVStream** streams;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public string filename;
        public Int64 start_time;
        public Int64 duration;
        public Int64 bit_rate;
        public uint packet_size;
        public int max_delay;
        public int flags;
        public const int AVFMT_FLAG_GENPTS = 0x0001;
        public const int AVFMT_FLAG_IGNIDX = 0x0002;
        public const int AVFMT_FLAG_NONBLOCK = 0x0004;
        public const int AVFMT_FLAG_IGNDTS = 0x0008;
        public const int AVFMT_FLAG_NOFILLIN = 0x0010;
        public const int AVFMT_FLAG_NOPARSE = 0x0020;
        public const int AVFMT_FLAG_NOBUFFER = 0x0040;
        public const int AVFMT_FLAG_CUSTOM_IO = 0x0080;
        public const int AVFMT_FLAG_DISCARD_CORRUPT = 0x0100;
        public const int AVFMT_FLAG_FLUSH_PACKETS = 0x0200;
        public const int AVFMT_FLAG_BITEXACT = 0x0400;
        public const int AVFMT_FLAG_MP4A_LATM = 0x8000;
        public const int AVFMT_FLAG_SORT_DTS = 0x10000;
        public const int AVFMT_FLAG_PRIV_OPT = 0x20000;
        public const int AVFMT_FLAG_KEEP_SIDE_DATA = 0x40000;
        public const int AVFMT_FLAG_FAST_SEEK = 0x80000;

        public Int64 probesize;
        public Int64 max_analyze_duration;
        public IntPtr key;
        public int keylen;
        public uint nb_programs;
        public IntPtr programs; // AVProgram** programs;
        public AVCodecID video_codec_id;
        public AVCodecID audio_codec_id;
        public AVCodecID subtitle_codec_id;
        public uint max_index_size;
        public uint max_picture_buffer;
        public uint nb_chapters;
        public IntPtr chapters; // AVChapter** chapters//;
        public IntPtr metadata; // AVDictionary* metadata;
        public Int64 start_time_realtime;
        public int fps_probe_size;
        public int error_recognition;
        public IntPtr interrupt_callback; // AVIOInterruptCB interrupt_callback;
        public int debug;
        public const int FF_FDEBUG_TS = 0x0001;
        public Int64 max_interleave_delta;
        public int strict_std_compliance;
        public int event_flags;
        public const int AVFMT_EVENT_FLAG_METADATA_UPDATED = 0x0001;
        public int max_ts_probe;
        public int avoid_negative_ts;
        public const int AVFMT_AVOID_NEG_TS_AUTO = -1;
        public const int AVFMT_AVOID_NEG_TS_MAKE_NON_NEGATIVE = 1;
        public const int AVFMT_AVOID_NEG_TS_MAKE_ZERO = 2;
        public int ts_id;
        public int audio_preload;
        public int max_chunk_duration;
        public int max_chunk_size;
        public int use_wallclock_as_timestamps;
        public int avio_flags;
        public AVDurationEstimationMethod duration_estimation_method;    
        public Int64 skip_initial_bytes;
        public uint correct_ts_overflow;
        public int seek2any;
        public int flush_packets;
        public int probe_score;
        public int format_probesize;
        public IntPtr codec_whitelist;
        public IntPtr format_whitelist;
        public IntPtr @internal; // AVFormatInternal* internal;
        public int io_repositioned;
        public IntPtr video_codec; // AVCodec* video_codec;
        public IntPtr audio_codec; // AVCodec* audio_codec;
        public IntPtr subtitle_codec; // AVCodec* subtitle_codec;
        public IntPtr data_codec; // AVCodec* data_codec;
        public int metadata_header_padding;
        public IntPtr opaque;
        public IntPtr control_message_cb; // av_format_control_message control_message_cb;
        public Int64 output_ts_offset;
        public IntPtr dump_separator;
        AVCodecID data_codec_id;

#if FF_API_OLD_OPEN_CALLBACKS
        // attribute_deprecated
        public IntPtr open_cb; // int (*open_cb)(struct AVFormatContext *s, AVIOContext **p, const char *url, int flags, const AVIOInterruptCB *int_cb, AVDictionary **options);
#endif
        public IntPtr protocol_whitelist;

        public IntPtr io_open;// int (*io_open)(struct AVFormatContext *s, AVIOContext** pb, const char* url, int flags, AVDictionary **options);
        public IntPtr io_close; // void(*io_close)(struct AVFormatContext *s, AVIOContext* pb);

        public IntPtr protocol_blacklist;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct AVProbeData
    {
        IntPtr filename;// const char* filename;
        IntPtr buf;// unsigned char* buf; /**< Buffer must have AVPROBE_PADDING_SIZE of extra allocated bytes filled with zero. */
        int buf_size;       /**< Size of buf except extra allocated bytes */
        IntPtr mime_type;// const char* mime_type; /**< mime_type, when known. */
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AVStream
    {
        public int index;    /**< stream index in AVFormatContext */
        public int id;
#if FF_API_LAVF_AVCTX
        // attribute_deprecated
        public IntPtr codec;// AVCodecContext *codec;
#endif
        public IntPtr priv_data;

#if FF_API_LAVF_FRAC
        // attribute_deprecated
        public AVFrac pts;
#endif
        public AVRational time_base;
        public Int64 start_time;
        public Int64 duration;

        public Int64 nb_frames;                 ///< number of frames in this stream if known or 0

        public int disposition; /**< AV_DISPOSITION_* bit field */

        public AVDiscard discard; ///< Selects which packets can be discarded at will and do not need to be demuxed.

        public AVRational sample_aspect_ratio;

        public IntPtr metadata; // AVDictionary* metadata;
        public AVRational avg_frame_rate;
        public AVPacket attached_pic;
        public IntPtr side_data; // AVPacketSideData* side_data;
        public int nb_side_data;
        public int event_flags;
        public const int AVSTREAM_EVENT_FLAG_METADATA_UPDATED = 0x0001;

        /*****************************************************************
         * All fields below this line are not part of the public API. They
         * may not be used outside of libavformat and can be changed and
         * removed at will.
         * New public fields should be added right above.
         *****************************************************************
         */

        const int MAX_STD_TIMEBASES = (30 * 12 + 30 + 3 + 6);
        
        IntPtr info; // struct { ... } *

        int pts_wrap_bits; /**< number of bits in pts (used for wrapping control) */
        Int64 first_dts;
        Int64 cur_dts;
        Int64 last_IP_pts;
        int last_IP_duration;
        int probe_packets;
        int codec_info_nb_frames;
        int need_parsing;// enum AVStreamParseType need_parsing;
        IntPtr parser;// struct AVCodecParserContext * parser;

        IntPtr last_in_packet_buffer;//  struct AVPacketList * last_in_packet_buffer;
        AVProbeData probe_data;
        const int MAX_REORDER_DELAY = 16;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_REORDER_DELAY + 1)]
        Int64[] pts_buffer;

        IntPtr index_entries; // AVIndexEntry* index_entries; /**< Only used if the format does not support seeking natively. */
        int nb_index_entries;
        uint index_entries_allocated_size;
        AVRational r_frame_rate;
        int stream_identifier;

        Int64 interleaver_chunk_size;
        Int64 interleaver_chunk_duration;

        int request_probe;
        int skip_to_keyframe;
        int skip_samples;
        Int64 start_skip_samples;
        Int64 first_discard_sample;
        Int64 last_discard_sample;
        int nb_decoded_frames;
        Int64 mux_ts_offset;
        Int64 pts_wrap_reference;
        int pts_wrap_behavior;
        int update_initial_durations_done;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_REORDER_DELAY + 1)]
        Int64[] pts_reorder_error;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_REORDER_DELAY + 1)]
        byte[] pts_reorder_error_count;
        Int64 last_dts_for_order_check;
        byte dts_ordered;
        byte dts_misordered;
        int inject_global_side_data;
        IntPtr recommended_encoder_configuration; // char* recommended_encoder_configuration;
        AVRational display_aspect_ratio;
        IntPtr priv_pts;// struct FFFrac * priv_pts;
        IntPtr @internal; // AVStreamInternal* internal;

        public IntPtr codecpar;// AVCodecParameters* codecpar;
    }

    public class AVFormatMethods
    {
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_register_all();
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avformat_alloc_output_context2(out IntPtr ctx, IntPtr oformat, string format_name, string filename);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avformat_new_stream(IntPtr formatCtx, IntPtr codec);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_dump_format(IntPtr formatCtx, int index, string url, int is_output);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avio_open(out IntPtr avioContext, string url, int flags);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avio_closep(ref IntPtr avioContext);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avformat_write_header(IntPtr formatCtx, IntPtr options);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_interleaved_write_frame(IntPtr formatCtx, IntPtr pkt);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int av_write_trailer(IntPtr formatCtx);
        [DllImport("avformat-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avformat_free_context(IntPtr formatCtx);
    }
}
