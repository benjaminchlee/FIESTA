#define FF_API_CONVERGENCE_DURATION
#define FF_API_XVMC
#define FF_API_VIMA_DECODER
#define FF_API_VOXWARE
#define AV_CODEC_ID_VOXWARE
#define FF_API_CODEC_NAME
#define FF_API_STREAM_CODEC_TAG
#define FF_API_ASPECT_EXTENDED
#define FF_API_MOTION_EST
#define FF_API_RC_STRATEGY
#define FF_API_PRIVATE_OPT
#define FF_API_AFD
#define FF_API_QUANT_BIAS
#define FF_API_MPV_OPT
#define FF_API_UNUSED_MEMBERS
#define FF_API_CODER_TYPE
#define FF_API_RTP_CALLBACK
#define FF_API_STAT_BITS
#define FF_API_OLD_MSMPEG4
#define FF_API_AC_VLC
#define FF_API_DEBUG_MV
#define FF_API_ARCH_SH4
#define FF_API_IDCT_XVIDMMX
#define FF_API_ARCH_SPARC
#define FF_IDCT_SIMPLEALPHA
#define FF_API_ARCH_ALPHA
#define FF_API_LOWRES
#define FF_API_CODED_FRAME
#define FF_API_ERROR_RATE
#define FF_API_VBV_DELAY
#define FF_API_SIDEDATA_ONLY_PKT
#define FF_API_VDPAU
#define FF_API_VAAPI
#define FF_API_ASS_TIMING

using FFmpegWrapper.AVUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace FFmpegWrapper.AVCodec
{
    public struct AVCodecFlags
    {
        public const int AV_CODEC_FLAG_UNALIGNED = (1 << 0);
        public const int AV_CODEC_FLAG_QSCALE = (1 << 1);
        public const int AV_CODEC_FLAG_4MV = (1 << 2);
        public const int AV_CODEC_FLAG_OUTPUT_CORRUPT = (1 << 3);
        public const int AV_CODEC_FLAG_QPEL = (1 << 4);
        public const int AV_CODEC_FLAG_PASS1 = (1 << 9);
        public const int AV_CODEC_FLAG_PASS2 = (1 << 10);
        public const int AV_CODEC_FLAG_LOOP_FILTER = (1 << 11);
        public const int AV_CODEC_FLAG_GRAY = (1 << 13);
        public const int AV_CODEC_FLAG_PSNR = (1 << 15);
        public const int AV_CODEC_FLAG_TRUNCATED = (1 << 16);
        public const int AV_CODEC_FLAG_INTERLACED_DCT = (1 << 18);
        public const int AV_CODEC_FLAG_LOW_DELAY = (1 << 19);
        public const int AV_CODEC_FLAG_GLOBAL_HEADER = (1 << 22);
        public const int AV_CODEC_FLAG_BITEXACT = (1 << 23);
        public const int AV_CODEC_FLAG_AC_PRED = (1 << 24);
        public const int AV_CODEC_FLAG_INTERLACED_ME = (1 << 29);
        public const uint AV_CODEC_FLAG_CLOSED_GOP = (1U << 31);
        public const int AV_CODEC_FLAG2_FAST = (1 << 0);
        public const int AV_CODEC_FLAG2_NO_OUTPUT = (1 << 2);
        public const int AV_CODEC_FLAG2_LOCAL_HEADER = (1 << 3);
        public const int AV_CODEC_FLAG2_DROP_FRAME_TIMECODE = (1 << 13);
        public const int AV_CODEC_FLAG2_CHUNKS = (1 << 15);
        public const int AV_CODEC_FLAG2_IGNORE_CROP = (1 << 16);
        public const int AV_CODEC_FLAG2_SHOW_ALL = (1 << 22);
        public const int AV_CODEC_FLAG2_EXPORT_MVS = (1 << 28);
        public const int AV_CODEC_FLAG2_SKIP_MANUAL = (1 << 29);
        public const int AV_CODEC_FLAG2_RO_FLUSH_NOOP = (1 << 30);
    }

    public struct AVCodecCapabilities
    {
        public const int AV_CODEC_CAP_DRAW_HORIZ_BAND = (1 << 0);
        public const int AV_CODEC_CAP_DR1 = (1 << 1);
        public const int AV_CODEC_CAP_TRUNCATED = (1 << 3);
    }

    public enum AVCodecID
    {
        AV_CODEC_ID_NONE,

        /* video codecs */
        AV_CODEC_ID_MPEG1VIDEO,
        AV_CODEC_ID_MPEG2VIDEO, ///< preferred ID for MPEG-1/2 video decoding
#if FF_API_XVMC
        AV_CODEC_ID_MPEG2VIDEO_XVMC,
#endif 
        AV_CODEC_ID_H261,
        AV_CODEC_ID_H263,
        AV_CODEC_ID_RV10,
        AV_CODEC_ID_RV20,
        AV_CODEC_ID_MJPEG,
        AV_CODEC_ID_MJPEGB,
        AV_CODEC_ID_LJPEG,
        AV_CODEC_ID_SP5X,
        AV_CODEC_ID_JPEGLS,
        AV_CODEC_ID_MPEG4,
        AV_CODEC_ID_RAWVIDEO,
        AV_CODEC_ID_MSMPEG4V1,
        AV_CODEC_ID_MSMPEG4V2,
        AV_CODEC_ID_MSMPEG4V3,
        AV_CODEC_ID_WMV1,
        AV_CODEC_ID_WMV2,
        AV_CODEC_ID_H263P,
        AV_CODEC_ID_H263I,
        AV_CODEC_ID_FLV1,
        AV_CODEC_ID_SVQ1,
        AV_CODEC_ID_SVQ3,
        AV_CODEC_ID_DVVIDEO,
        AV_CODEC_ID_HUFFYUV,
        AV_CODEC_ID_CYUV,
        AV_CODEC_ID_H264,
        AV_CODEC_ID_INDEO3,
        AV_CODEC_ID_VP3,
        AV_CODEC_ID_THEORA,
        AV_CODEC_ID_ASV1,
        AV_CODEC_ID_ASV2,
        AV_CODEC_ID_FFV1,
        AV_CODEC_ID_4XM,
        AV_CODEC_ID_VCR1,
        AV_CODEC_ID_CLJR,
        AV_CODEC_ID_MDEC,
        AV_CODEC_ID_ROQ,
        AV_CODEC_ID_INTERPLAY_VIDEO,
        AV_CODEC_ID_XAN_WC3,
        AV_CODEC_ID_XAN_WC4,
        AV_CODEC_ID_RPZA,
        AV_CODEC_ID_CINEPAK,
        AV_CODEC_ID_WS_VQA,
        AV_CODEC_ID_MSRLE,
        AV_CODEC_ID_MSVIDEO1,
        AV_CODEC_ID_IDCIN,
        AV_CODEC_ID_8BPS,
        AV_CODEC_ID_SMC,
        AV_CODEC_ID_FLIC,
        AV_CODEC_ID_TRUEMOTION1,
        AV_CODEC_ID_VMDVIDEO,
        AV_CODEC_ID_MSZH,
        AV_CODEC_ID_ZLIB,
        AV_CODEC_ID_QTRLE,
        AV_CODEC_ID_TSCC,
        AV_CODEC_ID_ULTI,
        AV_CODEC_ID_QDRAW,
        AV_CODEC_ID_VIXL,
        AV_CODEC_ID_QPEG,
        AV_CODEC_ID_PNG,
        AV_CODEC_ID_PPM,
        AV_CODEC_ID_PBM,
        AV_CODEC_ID_PGM,
        AV_CODEC_ID_PGMYUV,
        AV_CODEC_ID_PAM,
        AV_CODEC_ID_FFVHUFF,
        AV_CODEC_ID_RV30,
        AV_CODEC_ID_RV40,
        AV_CODEC_ID_VC1,
        AV_CODEC_ID_WMV3,
        AV_CODEC_ID_LOCO,
        AV_CODEC_ID_WNV1,
        AV_CODEC_ID_AASC,
        AV_CODEC_ID_INDEO2,
        AV_CODEC_ID_FRAPS,
        AV_CODEC_ID_TRUEMOTION2,
        AV_CODEC_ID_BMP,
        AV_CODEC_ID_CSCD,
        AV_CODEC_ID_MMVIDEO,
        AV_CODEC_ID_ZMBV,
        AV_CODEC_ID_AVS,
        AV_CODEC_ID_SMACKVIDEO,
        AV_CODEC_ID_NUV,
        AV_CODEC_ID_KMVC,
        AV_CODEC_ID_FLASHSV,
        AV_CODEC_ID_CAVS,
        AV_CODEC_ID_JPEG2000,
        AV_CODEC_ID_VMNC,
        AV_CODEC_ID_VP5,
        AV_CODEC_ID_VP6,
        AV_CODEC_ID_VP6F,
        AV_CODEC_ID_TARGA,
        AV_CODEC_ID_DSICINVIDEO,
        AV_CODEC_ID_TIERTEXSEQVIDEO,
        AV_CODEC_ID_TIFF,
        AV_CODEC_ID_GIF,
        AV_CODEC_ID_DXA,
        AV_CODEC_ID_DNXHD,
        AV_CODEC_ID_THP,
        AV_CODEC_ID_SGI,
        AV_CODEC_ID_C93,
        AV_CODEC_ID_BETHSOFTVID,
        AV_CODEC_ID_PTX,
        AV_CODEC_ID_TXD,
        AV_CODEC_ID_VP6A,
        AV_CODEC_ID_AMV,
        AV_CODEC_ID_VB,
        AV_CODEC_ID_PCX,
        AV_CODEC_ID_SUNRAST,
        AV_CODEC_ID_INDEO4,
        AV_CODEC_ID_INDEO5,
        AV_CODEC_ID_MIMIC,
        AV_CODEC_ID_RL2,
        AV_CODEC_ID_ESCAPE124,
        AV_CODEC_ID_DIRAC,
        AV_CODEC_ID_BFI,
        AV_CODEC_ID_CMV,
        AV_CODEC_ID_MOTIONPIXELS,
        AV_CODEC_ID_TGV,
        AV_CODEC_ID_TGQ,
        AV_CODEC_ID_TQI,
        AV_CODEC_ID_AURA,
        AV_CODEC_ID_AURA2,
        AV_CODEC_ID_V210X,
        AV_CODEC_ID_TMV,
        AV_CODEC_ID_V210,
        AV_CODEC_ID_DPX,
        AV_CODEC_ID_MAD,
        AV_CODEC_ID_FRWU,
        AV_CODEC_ID_FLASHSV2,
        AV_CODEC_ID_CDGRAPHICS,
        AV_CODEC_ID_R210,
        AV_CODEC_ID_ANM,
        AV_CODEC_ID_BINKVIDEO,
        AV_CODEC_ID_IFF_ILBM,
        AV_CODEC_ID_IFF_BYTERUN1 = AV_CODEC_ID_IFF_ILBM,
        AV_CODEC_ID_KGV1,
        AV_CODEC_ID_YOP,
        AV_CODEC_ID_VP8,
        AV_CODEC_ID_PICTOR,
        AV_CODEC_ID_ANSI,
        AV_CODEC_ID_A64_MULTI,
        AV_CODEC_ID_A64_MULTI5,
        AV_CODEC_ID_R10K,
        AV_CODEC_ID_MXPEG,
        AV_CODEC_ID_LAGARITH,
        AV_CODEC_ID_PRORES,
        AV_CODEC_ID_JV,
        AV_CODEC_ID_DFA,
        AV_CODEC_ID_WMV3IMAGE,
        AV_CODEC_ID_VC1IMAGE,
        AV_CODEC_ID_UTVIDEO,
        AV_CODEC_ID_BMV_VIDEO,
        AV_CODEC_ID_VBLE,
        AV_CODEC_ID_DXTORY,
        AV_CODEC_ID_V410,
        AV_CODEC_ID_XWD,
        AV_CODEC_ID_CDXL,
        AV_CODEC_ID_XBM,
        AV_CODEC_ID_ZEROCODEC,
        AV_CODEC_ID_MSS1,
        AV_CODEC_ID_MSA1,
        AV_CODEC_ID_TSCC2,
        AV_CODEC_ID_MTS2,
        AV_CODEC_ID_CLLC,
        AV_CODEC_ID_MSS2,
        AV_CODEC_ID_VP9,
        AV_CODEC_ID_AIC,
        AV_CODEC_ID_ESCAPE130,
        AV_CODEC_ID_G2M,
        AV_CODEC_ID_WEBP,
        AV_CODEC_ID_HNM4_VIDEO,
        AV_CODEC_ID_HEVC,
        AV_CODEC_ID_H265 = AV_CODEC_ID_HEVC,
        AV_CODEC_ID_FIC,
        AV_CODEC_ID_ALIAS_PIX,
        AV_CODEC_ID_BRENDER_PIX,
        AV_CODEC_ID_PAF_VIDEO,
        AV_CODEC_ID_EXR,
        AV_CODEC_ID_VP7,
        AV_CODEC_ID_SANM,
        AV_CODEC_ID_SGIRLE,
        AV_CODEC_ID_MVC1,
        AV_CODEC_ID_MVC2,
        AV_CODEC_ID_HQX,
        AV_CODEC_ID_TDSC,
        AV_CODEC_ID_HQ_HQA,
        AV_CODEC_ID_HAP,
        AV_CODEC_ID_DDS,
        AV_CODEC_ID_DXV,
        AV_CODEC_ID_SCREENPRESSO,
        AV_CODEC_ID_RSCC,

        AV_CODEC_ID_Y41P = 0x8000,
        AV_CODEC_ID_AVRP,
        AV_CODEC_ID_012V,
        AV_CODEC_ID_AVUI,
        AV_CODEC_ID_AYUV,
        AV_CODEC_ID_TARGA_Y216,
        AV_CODEC_ID_V308,
        AV_CODEC_ID_V408,
        AV_CODEC_ID_YUV4,
        AV_CODEC_ID_AVRN,
        AV_CODEC_ID_CPIA,
        AV_CODEC_ID_XFACE,
        AV_CODEC_ID_SNOW,
        AV_CODEC_ID_SMVJPEG,
        AV_CODEC_ID_APNG,
        AV_CODEC_ID_DAALA,
        AV_CODEC_ID_CFHD,
        AV_CODEC_ID_TRUEMOTION2RT,
        AV_CODEC_ID_M101,
        AV_CODEC_ID_MAGICYUV,
        AV_CODEC_ID_SHEERVIDEO,
        AV_CODEC_ID_YLC,

        /* various PCM "codecs" */
        AV_CODEC_ID_FIRST_AUDIO = 0x10000,     ///< A dummy id pointing at the start of audio codecs
        AV_CODEC_ID_PCM_S16LE = 0x10000,
        AV_CODEC_ID_PCM_S16BE,
        AV_CODEC_ID_PCM_U16LE,
        AV_CODEC_ID_PCM_U16BE,
        AV_CODEC_ID_PCM_S8,
        AV_CODEC_ID_PCM_U8,
        AV_CODEC_ID_PCM_MULAW,
        AV_CODEC_ID_PCM_ALAW,
        AV_CODEC_ID_PCM_S32LE,
        AV_CODEC_ID_PCM_S32BE,
        AV_CODEC_ID_PCM_U32LE,
        AV_CODEC_ID_PCM_U32BE,
        AV_CODEC_ID_PCM_S24LE,
        AV_CODEC_ID_PCM_S24BE,
        AV_CODEC_ID_PCM_U24LE,
        AV_CODEC_ID_PCM_U24BE,
        AV_CODEC_ID_PCM_S24DAUD,
        AV_CODEC_ID_PCM_ZORK,
        AV_CODEC_ID_PCM_S16LE_PLANAR,
        AV_CODEC_ID_PCM_DVD,
        AV_CODEC_ID_PCM_F32BE,
        AV_CODEC_ID_PCM_F32LE,
        AV_CODEC_ID_PCM_F64BE,
        AV_CODEC_ID_PCM_F64LE,
        AV_CODEC_ID_PCM_BLURAY,
        AV_CODEC_ID_PCM_LXF,
        AV_CODEC_ID_S302M,
        AV_CODEC_ID_PCM_S8_PLANAR,
        AV_CODEC_ID_PCM_S24LE_PLANAR,
        AV_CODEC_ID_PCM_S32LE_PLANAR,
        AV_CODEC_ID_PCM_S16BE_PLANAR,
        /* new PCM "codecs" should be added right below this line starting with
         * an explicit value of for example 0x10800
         */

        /* various ADPCM codecs */
        AV_CODEC_ID_ADPCM_IMA_QT = 0x11000,
        AV_CODEC_ID_ADPCM_IMA_WAV,
        AV_CODEC_ID_ADPCM_IMA_DK3,
        AV_CODEC_ID_ADPCM_IMA_DK4,
        AV_CODEC_ID_ADPCM_IMA_WS,
        AV_CODEC_ID_ADPCM_IMA_SMJPEG,
        AV_CODEC_ID_ADPCM_MS,
        AV_CODEC_ID_ADPCM_4XM,
        AV_CODEC_ID_ADPCM_XA,
        AV_CODEC_ID_ADPCM_ADX,
        AV_CODEC_ID_ADPCM_EA,
        AV_CODEC_ID_ADPCM_G726,
        AV_CODEC_ID_ADPCM_CT,
        AV_CODEC_ID_ADPCM_SWF,
        AV_CODEC_ID_ADPCM_YAMAHA,
        AV_CODEC_ID_ADPCM_SBPRO_4,
        AV_CODEC_ID_ADPCM_SBPRO_3,
        AV_CODEC_ID_ADPCM_SBPRO_2,
        AV_CODEC_ID_ADPCM_THP,
        AV_CODEC_ID_ADPCM_IMA_AMV,
        AV_CODEC_ID_ADPCM_EA_R1,
        AV_CODEC_ID_ADPCM_EA_R3,
        AV_CODEC_ID_ADPCM_EA_R2,
        AV_CODEC_ID_ADPCM_IMA_EA_SEAD,
        AV_CODEC_ID_ADPCM_IMA_EA_EACS,
        AV_CODEC_ID_ADPCM_EA_XAS,
        AV_CODEC_ID_ADPCM_EA_MAXIS_XA,
        AV_CODEC_ID_ADPCM_IMA_ISS,
        AV_CODEC_ID_ADPCM_G722,
        AV_CODEC_ID_ADPCM_IMA_APC,
        AV_CODEC_ID_ADPCM_VIMA,
#if FF_API_VIMA_DECODER
        AV_CODEC_ID_VIMA = AV_CODEC_ID_ADPCM_VIMA,
#endif
        AV_CODEC_ID_ADPCM_AFC = 0x11800,
        AV_CODEC_ID_ADPCM_IMA_OKI,
        AV_CODEC_ID_ADPCM_DTK,
        AV_CODEC_ID_ADPCM_IMA_RAD,
        AV_CODEC_ID_ADPCM_G726LE,
        AV_CODEC_ID_ADPCM_THP_LE,
        AV_CODEC_ID_ADPCM_PSX,
        AV_CODEC_ID_ADPCM_AICA,
        AV_CODEC_ID_ADPCM_IMA_DAT4,
        AV_CODEC_ID_ADPCM_MTAF,

        /* AMR */
        AV_CODEC_ID_AMR_NB = 0x12000,
        AV_CODEC_ID_AMR_WB,

        /* RealAudio codecs*/
        AV_CODEC_ID_RA_144 = 0x13000,
        AV_CODEC_ID_RA_288,

        /* various DPCM codecs */
        AV_CODEC_ID_ROQ_DPCM = 0x14000,
        AV_CODEC_ID_INTERPLAY_DPCM,
        AV_CODEC_ID_XAN_DPCM,
        AV_CODEC_ID_SOL_DPCM,

        AV_CODEC_ID_SDX2_DPCM = 0x14800,

        /* audio codecs */
        AV_CODEC_ID_MP2 = 0x15000,
        AV_CODEC_ID_MP3, ///< preferred ID for decoding MPEG audio layer 1, 2 or 3
        AV_CODEC_ID_AAC,
        AV_CODEC_ID_AC3,
        AV_CODEC_ID_DTS,
        AV_CODEC_ID_VORBIS,
        AV_CODEC_ID_DVAUDIO,
        AV_CODEC_ID_WMAV1,
        AV_CODEC_ID_WMAV2,
        AV_CODEC_ID_MACE3,
        AV_CODEC_ID_MACE6,
        AV_CODEC_ID_VMDAUDIO,
        AV_CODEC_ID_FLAC,
        AV_CODEC_ID_MP3ADU,
        AV_CODEC_ID_MP3ON4,
        AV_CODEC_ID_SHORTEN,
        AV_CODEC_ID_ALAC,
        AV_CODEC_ID_WESTWOOD_SND1,
        AV_CODEC_ID_GSM, ///< as in Berlin toast format
        AV_CODEC_ID_QDM2,
        AV_CODEC_ID_COOK,
        AV_CODEC_ID_TRUESPEECH,
        AV_CODEC_ID_TTA,
        AV_CODEC_ID_SMACKAUDIO,
        AV_CODEC_ID_QCELP,
        AV_CODEC_ID_WAVPACK,
        AV_CODEC_ID_DSICINAUDIO,
        AV_CODEC_ID_IMC,
        AV_CODEC_ID_MUSEPACK7,
        AV_CODEC_ID_MLP,
        AV_CODEC_ID_GSM_MS, /* as found in WAV */
        AV_CODEC_ID_ATRAC3,
#if FF_API_VOXWARE
        AV_CODEC_ID_VOXWARE,
#endif
        AV_CODEC_ID_APE,
        AV_CODEC_ID_NELLYMOSER,
        AV_CODEC_ID_MUSEPACK8,
        AV_CODEC_ID_SPEEX,
        AV_CODEC_ID_WMAVOICE,
        AV_CODEC_ID_WMAPRO,
        AV_CODEC_ID_WMALOSSLESS,
        AV_CODEC_ID_ATRAC3P,
        AV_CODEC_ID_EAC3,
        AV_CODEC_ID_SIPR,
        AV_CODEC_ID_MP1,
        AV_CODEC_ID_TWINVQ,
        AV_CODEC_ID_TRUEHD,
        AV_CODEC_ID_MP4ALS,
        AV_CODEC_ID_ATRAC1,
        AV_CODEC_ID_BINKAUDIO_RDFT,
        AV_CODEC_ID_BINKAUDIO_DCT,
        AV_CODEC_ID_AAC_LATM,
        AV_CODEC_ID_QDMC,
        AV_CODEC_ID_CELT,
        AV_CODEC_ID_G723_1,
        AV_CODEC_ID_G729,
        AV_CODEC_ID_8SVX_EXP,
        AV_CODEC_ID_8SVX_FIB,
        AV_CODEC_ID_BMV_AUDIO,
        AV_CODEC_ID_RALF,
        AV_CODEC_ID_IAC,
        AV_CODEC_ID_ILBC,
        AV_CODEC_ID_OPUS,
        AV_CODEC_ID_COMFORT_NOISE,
        AV_CODEC_ID_TAK,
        AV_CODEC_ID_METASOUND,
        AV_CODEC_ID_PAF_AUDIO,
        AV_CODEC_ID_ON2AVC,
        AV_CODEC_ID_DSS_SP,

        AV_CODEC_ID_FFWAVESYNTH = 0x15800,
        AV_CODEC_ID_SONIC,
        AV_CODEC_ID_SONIC_LS,
        AV_CODEC_ID_EVRC,
        AV_CODEC_ID_SMV,
        AV_CODEC_ID_DSD_LSBF,
        AV_CODEC_ID_DSD_MSBF,
        AV_CODEC_ID_DSD_LSBF_PLANAR,
        AV_CODEC_ID_DSD_MSBF_PLANAR,
        AV_CODEC_ID_4GV,
        AV_CODEC_ID_INTERPLAY_ACM,
        AV_CODEC_ID_XMA1,
        AV_CODEC_ID_XMA2,
        AV_CODEC_ID_DST,

        /* subtitle codecs */
        AV_CODEC_ID_FIRST_SUBTITLE = 0x17000,          ///< A dummy ID pointing at the start of subtitle codecs.
        AV_CODEC_ID_DVD_SUBTITLE = 0x17000,
        AV_CODEC_ID_DVB_SUBTITLE,
        AV_CODEC_ID_TEXT,  ///< raw UTF-8 text
        AV_CODEC_ID_XSUB,
        AV_CODEC_ID_SSA,
        AV_CODEC_ID_MOV_TEXT,
        AV_CODEC_ID_HDMV_PGS_SUBTITLE,
        AV_CODEC_ID_DVB_TELETEXT,
        AV_CODEC_ID_SRT,

        AV_CODEC_ID_MICRODVD = 0x17800,
        AV_CODEC_ID_EIA_608,
        AV_CODEC_ID_JACOSUB,
        AV_CODEC_ID_SAMI,
        AV_CODEC_ID_REALTEXT,
        AV_CODEC_ID_STL,
        AV_CODEC_ID_SUBVIEWER1,
        AV_CODEC_ID_SUBVIEWER,
        AV_CODEC_ID_SUBRIP,
        AV_CODEC_ID_WEBVTT,
        AV_CODEC_ID_MPL2,
        AV_CODEC_ID_VPLAYER,
        AV_CODEC_ID_PJS,
        AV_CODEC_ID_ASS,
        AV_CODEC_ID_HDMV_TEXT_SUBTITLE,

        /* other specific kind of codecs (generally used for attachments) */
        AV_CODEC_ID_FIRST_UNKNOWN = 0x18000,           ///< A dummy ID pointing at the start of various fake codecs.
        AV_CODEC_ID_TTF = 0x18000,

        AV_CODEC_ID_BINTEXT = 0x18800,
        AV_CODEC_ID_XBIN,
        AV_CODEC_ID_IDF,
        AV_CODEC_ID_OTF,
        AV_CODEC_ID_SMPTE_KLV,
        AV_CODEC_ID_DVD_NAV,
        AV_CODEC_ID_TIMED_ID3,
        AV_CODEC_ID_BIN_DATA,


        AV_CODEC_ID_PROBE = 0x19000, ///< codec_id is not known (like AV_CODEC_ID_NONE) but lavf should attempt to identify it

        AV_CODEC_ID_MPEG2TS = 0x20000, /**< _FAKE_ codec to indicate a raw MPEG-2 TS
                                * stream (only used by libavformat) */
        AV_CODEC_ID_MPEG4SYSTEMS = 0x20001, /**< _FAKE_ codec to indicate a MPEG-4 Systems
                                * stream (only used by libavformat) */
        AV_CODEC_ID_FFMETADATA = 0x21000,   ///< Dummy codec for streams containing only metadata information.
        AV_CODEC_ID_WRAPPED_AVFRAME = 0x21001, ///< Passthrough codec, AVFrames wrapped in AVPacket
    };

    public enum AVPixelFormat
    {
        AV_PIX_FMT_NONE = -1,
        AV_PIX_FMT_YUV420P,   ///< planar YUV 4:2:0, 12bpp, (1 Cr & Cb sample per 2x2 Y samples)
        AV_PIX_FMT_YUYV422,   ///< packed YUV 4:2:2, 16bpp, Y0 Cb Y1 Cr
        AV_PIX_FMT_RGB24,     ///< packed RGB 8:8:8, 24bpp, RGBRGB...
        AV_PIX_FMT_BGR24,     ///< packed RGB 8:8:8, 24bpp, BGRBGR...
        AV_PIX_FMT_YUV422P,   ///< planar YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
        AV_PIX_FMT_YUV444P,   ///< planar YUV 4:4:4, 24bpp, (1 Cr & Cb sample per 1x1 Y samples)
        AV_PIX_FMT_YUV410P,   ///< planar YUV 4:1:0,  9bpp, (1 Cr & Cb sample per 4x4 Y samples)
        AV_PIX_FMT_YUV411P,   ///< planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples)
        AV_PIX_FMT_GRAY8,     ///<        Y        ,  8bpp
        AV_PIX_FMT_MONOWHITE, ///<        Y        ,  1bpp, 0 is white, 1 is black, in each byte pixels are ordered from the msb to the lsb
        AV_PIX_FMT_MONOBLACK, ///<        Y        ,  1bpp, 0 is black, 1 is white, in each byte pixels are ordered from the msb to the lsb
        AV_PIX_FMT_PAL8,      ///< 8 bits with AV_PIX_FMT_RGB32 palette
        AV_PIX_FMT_YUVJ420P,  ///< planar YUV 4:2:0, 12bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV420P and setting color_range
        AV_PIX_FMT_YUVJ422P,  ///< planar YUV 4:2:2, 16bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV422P and setting color_range
        AV_PIX_FMT_YUVJ444P,  ///< planar YUV 4:4:4, 24bpp, full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV444P and setting color_range
#if FF_API_XVMC
        AV_PIX_FMT_XVMC_MPEG2_MC,///< XVideo Motion Acceleration via common packet passing
        AV_PIX_FMT_XVMC_MPEG2_IDCT,
        AV_PIX_FMT_XVMC = AV_PIX_FMT_XVMC_MPEG2_IDCT,
#endif 
        AV_PIX_FMT_UYVY422,   ///< packed YUV 4:2:2, 16bpp, Cb Y0 Cr Y1
        AV_PIX_FMT_UYYVYY411, ///< packed YUV 4:1:1, 12bpp, Cb Y0 Y1 Cr Y2 Y3
        AV_PIX_FMT_BGR8,      ///< packed RGB 3:3:2,  8bpp, (msb)2B 3G 3R(lsb)
        AV_PIX_FMT_BGR4,      ///< packed RGB 1:2:1 bitstream,  4bpp, (msb)1B 2G 1R(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
        AV_PIX_FMT_BGR4_BYTE, ///< packed RGB 1:2:1,  8bpp, (msb)1B 2G 1R(lsb)
        AV_PIX_FMT_RGB8,      ///< packed RGB 3:3:2,  8bpp, (msb)2R 3G 3B(lsb)
        AV_PIX_FMT_RGB4,      ///< packed RGB 1:2:1 bitstream,  4bpp, (msb)1R 2G 1B(lsb), a byte contains two pixels, the first pixel in the byte is the one composed by the 4 msb bits
        AV_PIX_FMT_RGB4_BYTE, ///< packed RGB 1:2:1,  8bpp, (msb)1R 2G 1B(lsb)
        AV_PIX_FMT_NV12,      ///< planar YUV 4:2:0, 12bpp, 1 plane for Y and 1 plane for the UV components, which are interleaved (first byte U and the following byte V)
        AV_PIX_FMT_NV21,      ///< as above, but U and V bytes are swapped

        AV_PIX_FMT_ARGB,      ///< packed ARGB 8:8:8:8, 32bpp, ARGBARGB...
        AV_PIX_FMT_RGBA,      ///< packed RGBA 8:8:8:8, 32bpp, RGBARGBA...
        AV_PIX_FMT_ABGR,      ///< packed ABGR 8:8:8:8, 32bpp, ABGRABGR...
        AV_PIX_FMT_BGRA,      ///< packed BGRA 8:8:8:8, 32bpp, BGRABGRA...

        AV_PIX_FMT_GRAY16BE,  ///<        Y        , 16bpp, big-endian
        AV_PIX_FMT_GRAY16LE,  ///<        Y        , 16bpp, little-endian
        AV_PIX_FMT_YUV440P,   ///< planar YUV 4:4:0 (1 Cr & Cb sample per 1x2 Y samples)
        AV_PIX_FMT_YUVJ440P,  ///< planar YUV 4:4:0 full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV440P and setting color_range
        AV_PIX_FMT_YUVA420P,  ///< planar YUV 4:2:0, 20bpp, (1 Cr & Cb sample per 2x2 Y & A samples)
#if FF_API_VDPAU
        AV_PIX_FMT_VDPAU_H264,///< H.264 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
        AV_PIX_FMT_VDPAU_MPEG1,///< MPEG-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
        AV_PIX_FMT_VDPAU_MPEG2,///< MPEG-2 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
        AV_PIX_FMT_VDPAU_WMV3,///< WMV3 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
        AV_PIX_FMT_VDPAU_VC1, ///< VC-1 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
#endif
        AV_PIX_FMT_RGB48BE,   ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as big-endian
        AV_PIX_FMT_RGB48LE,   ///< packed RGB 16:16:16, 48bpp, 16R, 16G, 16B, the 2-byte value for each R/G/B component is stored as little-endian

        AV_PIX_FMT_RGB565BE,  ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), big-endian
        AV_PIX_FMT_RGB565LE,  ///< packed RGB 5:6:5, 16bpp, (msb)   5R 6G 5B(lsb), little-endian
        AV_PIX_FMT_RGB555BE,  ///< packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), big-endian   , X=unused/undefined
        AV_PIX_FMT_RGB555LE,  ///< packed RGB 5:5:5, 16bpp, (msb)1X 5R 5G 5B(lsb), little-endian, X=unused/undefined

        AV_PIX_FMT_BGR565BE,  ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), big-endian
        AV_PIX_FMT_BGR565LE,  ///< packed BGR 5:6:5, 16bpp, (msb)   5B 6G 5R(lsb), little-endian
        AV_PIX_FMT_BGR555BE,  ///< packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), big-endian   , X=unused/undefined
        AV_PIX_FMT_BGR555LE,  ///< packed BGR 5:5:5, 16bpp, (msb)1X 5B 5G 5R(lsb), little-endian, X=unused/undefined

#if FF_API_VAAPI
        /** @name Deprecated pixel formats */
        /**@{*/
        AV_PIX_FMT_VAAPI_MOCO, ///< HW acceleration through VA API at motion compensation entry-point, Picture.data[3] contains a vaapi_render_state struct which contains macroblocks as well as various fields extracted from headers
        AV_PIX_FMT_VAAPI_IDCT, ///< HW acceleration through VA API at IDCT entry-point, Picture.data[3] contains a vaapi_render_state struct which contains fields extracted from headers
        AV_PIX_FMT_VAAPI_VLD,  ///< HW decoding through VA API, Picture.data[3] contains a VASurfaceID
        /**@}*/
        AV_PIX_FMT_VAAPI = AV_PIX_FMT_VAAPI_VLD,
#else
        /**
         *  Hardware acceleration through VA-API, data[3] contains a
         *  VASurfaceID.
         */
        AV_PIX_FMT_VAAPI,
#endif

        AV_PIX_FMT_YUV420P16LE,  ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV420P16BE,  ///< planar YUV 4:2:0, 24bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV422P16LE,  ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV422P16BE,  ///< planar YUV 4:2:2, 32bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P16LE,  ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P16BE,  ///< planar YUV 4:4:4, 48bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
#if FF_API_VDPAU
        AV_PIX_FMT_VDPAU_MPEG4,  ///< MPEG-4 HW decoding with VDPAU, data[0] contains a vdpau_render_state struct which contains the bitstream of the slices as well as various fields extracted from headers
#endif
        AV_PIX_FMT_DXVA2_VLD,    ///< HW decoding through DXVA2, Picture.data[3] contains a LPDIRECT3DSURFACE9 pointer

        AV_PIX_FMT_RGB444LE,  ///< packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), little-endian, X=unused/undefined
        AV_PIX_FMT_RGB444BE,  ///< packed RGB 4:4:4, 16bpp, (msb)4X 4R 4G 4B(lsb), big-endian,    X=unused/undefined
        AV_PIX_FMT_BGR444LE,  ///< packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), little-endian, X=unused/undefined
        AV_PIX_FMT_BGR444BE,  ///< packed BGR 4:4:4, 16bpp, (msb)4X 4B 4G 4R(lsb), big-endian,    X=unused/undefined
        AV_PIX_FMT_YA8,       ///< 8 bits gray, 8 bits alpha

        AV_PIX_FMT_Y400A = AV_PIX_FMT_YA8, ///< alias for AV_PIX_FMT_YA8
        AV_PIX_FMT_GRAY8A = AV_PIX_FMT_YA8, ///< alias for AV_PIX_FMT_YA8

        AV_PIX_FMT_BGR48BE,   ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as big-endian
        AV_PIX_FMT_BGR48LE,   ///< packed RGB 16:16:16, 48bpp, 16B, 16G, 16R, the 2-byte value for each R/G/B component is stored as little-endian

        /**
         * The following 12 formats have the disadvantage of needing 1 format for each bit depth.
         * Notice that each 9/10 bits sample is stored in 16 bits with extra padding.
         * If you want to support multiple bit depths, then using AV_PIX_FMT_YUV420P16* with the bpp stored separately is better.
         */
        AV_PIX_FMT_YUV420P9BE, ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P9LE, ///< planar YUV 4:2:0, 13.5bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV420P10BE,///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P10LE,///< planar YUV 4:2:0, 15bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV422P10BE,///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P10LE,///< planar YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P9BE, ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P9LE, ///< planar YUV 4:4:4, 27bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P10BE,///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P10LE,///< planar YUV 4:4:4, 30bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV422P9BE, ///< planar YUV 4:2:2, 18bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P9LE, ///< planar YUV 4:2:2, 18bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_VDA_VLD,    ///< hardware decoding through VDA
        AV_PIX_FMT_GBRP,      ///< planar GBR 4:4:4 24bpp
        AV_PIX_FMT_GBRP9BE,   ///< planar GBR 4:4:4 27bpp, big-endian
        AV_PIX_FMT_GBRP9LE,   ///< planar GBR 4:4:4 27bpp, little-endian
        AV_PIX_FMT_GBRP10BE,  ///< planar GBR 4:4:4 30bpp, big-endian
        AV_PIX_FMT_GBRP10LE,  ///< planar GBR 4:4:4 30bpp, little-endian
        AV_PIX_FMT_GBRP16BE,  ///< planar GBR 4:4:4 48bpp, big-endian
        AV_PIX_FMT_GBRP16LE,  ///< planar GBR 4:4:4 48bpp, little-endian
        AV_PIX_FMT_YUVA422P,  ///< planar YUV 4:2:2 24bpp, (1 Cr & Cb sample per 2x1 Y & A samples)
        AV_PIX_FMT_YUVA444P,  ///< planar YUV 4:4:4 32bpp, (1 Cr & Cb sample per 1x1 Y & A samples)
        AV_PIX_FMT_YUVA420P9BE,  ///< planar YUV 4:2:0 22.5bpp, (1 Cr & Cb sample per 2x2 Y & A samples), big-endian
        AV_PIX_FMT_YUVA420P9LE,  ///< planar YUV 4:2:0 22.5bpp, (1 Cr & Cb sample per 2x2 Y & A samples), little-endian
        AV_PIX_FMT_YUVA422P9BE,  ///< planar YUV 4:2:2 27bpp, (1 Cr & Cb sample per 2x1 Y & A samples), big-endian
        AV_PIX_FMT_YUVA422P9LE,  ///< planar YUV 4:2:2 27bpp, (1 Cr & Cb sample per 2x1 Y & A samples), little-endian
        AV_PIX_FMT_YUVA444P9BE,  ///< planar YUV 4:4:4 36bpp, (1 Cr & Cb sample per 1x1 Y & A samples), big-endian
        AV_PIX_FMT_YUVA444P9LE,  ///< planar YUV 4:4:4 36bpp, (1 Cr & Cb sample per 1x1 Y & A samples), little-endian
        AV_PIX_FMT_YUVA420P10BE, ///< planar YUV 4:2:0 25bpp, (1 Cr & Cb sample per 2x2 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA420P10LE, ///< planar YUV 4:2:0 25bpp, (1 Cr & Cb sample per 2x2 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA422P10BE, ///< planar YUV 4:2:2 30bpp, (1 Cr & Cb sample per 2x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA422P10LE, ///< planar YUV 4:2:2 30bpp, (1 Cr & Cb sample per 2x1 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA444P10BE, ///< planar YUV 4:4:4 40bpp, (1 Cr & Cb sample per 1x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA444P10LE, ///< planar YUV 4:4:4 40bpp, (1 Cr & Cb sample per 1x1 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA420P16BE, ///< planar YUV 4:2:0 40bpp, (1 Cr & Cb sample per 2x2 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA420P16LE, ///< planar YUV 4:2:0 40bpp, (1 Cr & Cb sample per 2x2 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA422P16BE, ///< planar YUV 4:2:2 48bpp, (1 Cr & Cb sample per 2x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA422P16LE, ///< planar YUV 4:2:2 48bpp, (1 Cr & Cb sample per 2x1 Y & A samples, little-endian)
        AV_PIX_FMT_YUVA444P16BE, ///< planar YUV 4:4:4 64bpp, (1 Cr & Cb sample per 1x1 Y & A samples, big-endian)
        AV_PIX_FMT_YUVA444P16LE, ///< planar YUV 4:4:4 64bpp, (1 Cr & Cb sample per 1x1 Y & A samples, little-endian)

        AV_PIX_FMT_VDPAU,     ///< HW acceleration through VDPAU, Picture.data[3] contains a VdpVideoSurface

        AV_PIX_FMT_XYZ12LE,      ///< packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as little-endian, the 4 lower bits are set to 0
        AV_PIX_FMT_XYZ12BE,      ///< packed XYZ 4:4:4, 36 bpp, (msb) 12X, 12Y, 12Z (lsb), the 2-byte value for each X/Y/Z is stored as big-endian, the 4 lower bits are set to 0
        AV_PIX_FMT_NV16,         ///< interleaved chroma YUV 4:2:2, 16bpp, (1 Cr & Cb sample per 2x1 Y samples)
        AV_PIX_FMT_NV20LE,       ///< interleaved chroma YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_NV20BE,       ///< interleaved chroma YUV 4:2:2, 20bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian

        AV_PIX_FMT_RGBA64BE,     ///< packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
        AV_PIX_FMT_RGBA64LE,     ///< packed RGBA 16:16:16:16, 64bpp, 16R, 16G, 16B, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian
        AV_PIX_FMT_BGRA64BE,     ///< packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as big-endian
        AV_PIX_FMT_BGRA64LE,     ///< packed RGBA 16:16:16:16, 64bpp, 16B, 16G, 16R, 16A, the 2-byte value for each R/G/B/A component is stored as little-endian

        AV_PIX_FMT_YVYU422,   ///< packed YUV 4:2:2, 16bpp, Y0 Cr Y1 Cb

        AV_PIX_FMT_VDA,          ///< HW acceleration through VDA, data[3] contains a CVPixelBufferRef

        AV_PIX_FMT_YA16BE,       ///< 16 bits gray, 16 bits alpha (big-endian)
        AV_PIX_FMT_YA16LE,       ///< 16 bits gray, 16 bits alpha (little-endian)

        AV_PIX_FMT_GBRAP,        ///< planar GBRA 4:4:4:4 32bpp
        AV_PIX_FMT_GBRAP16BE,    ///< planar GBRA 4:4:4:4 64bpp, big-endian
        AV_PIX_FMT_GBRAP16LE,    ///< planar GBRA 4:4:4:4 64bpp, little-endian
        /**
         *  HW acceleration through QSV, data[3] contains a pointer to the
         *  mfxFrameSurface1 structure.
         */
        AV_PIX_FMT_QSV,
        /**
         * HW acceleration though MMAL, data[3] contains a pointer to the
         * MMAL_BUFFER_HEADER_T structure.
         */
        AV_PIX_FMT_MMAL,

        AV_PIX_FMT_D3D11VA_VLD,  ///< HW decoding through Direct3D11, Picture.data[3] contains a ID3D11VideoDecoderOutputView pointer

        /**
         * HW acceleration through CUDA. data[i] contain CUdeviceptr pointers
         * exactly as for system memory frames.
         */
        AV_PIX_FMT_CUDA,

        AV_PIX_FMT_0RGB = 0x123 + 4,///< packed RGB 8:8:8, 32bpp, XRGBXRGB...   X=unused/undefined
        AV_PIX_FMT_RGB0,        ///< packed RGB 8:8:8, 32bpp, RGBXRGBX...   X=unused/undefined
        AV_PIX_FMT_0BGR,        ///< packed BGR 8:8:8, 32bpp, XBGRXBGR...   X=unused/undefined
        AV_PIX_FMT_BGR0,        ///< packed BGR 8:8:8, 32bpp, BGRXBGRX...   X=unused/undefined

        AV_PIX_FMT_YUV420P12BE, ///< planar YUV 4:2:0,18bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P12LE, ///< planar YUV 4:2:0,18bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV420P14BE, ///< planar YUV 4:2:0,21bpp, (1 Cr & Cb sample per 2x2 Y samples), big-endian
        AV_PIX_FMT_YUV420P14LE, ///< planar YUV 4:2:0,21bpp, (1 Cr & Cb sample per 2x2 Y samples), little-endian
        AV_PIX_FMT_YUV422P12BE, ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P12LE, ///< planar YUV 4:2:2,24bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV422P14BE, ///< planar YUV 4:2:2,28bpp, (1 Cr & Cb sample per 2x1 Y samples), big-endian
        AV_PIX_FMT_YUV422P14LE, ///< planar YUV 4:2:2,28bpp, (1 Cr & Cb sample per 2x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P12BE, ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P12LE, ///< planar YUV 4:4:4,36bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_YUV444P14BE, ///< planar YUV 4:4:4,42bpp, (1 Cr & Cb sample per 1x1 Y samples), big-endian
        AV_PIX_FMT_YUV444P14LE, ///< planar YUV 4:4:4,42bpp, (1 Cr & Cb sample per 1x1 Y samples), little-endian
        AV_PIX_FMT_GBRP12BE,    ///< planar GBR 4:4:4 36bpp, big-endian
        AV_PIX_FMT_GBRP12LE,    ///< planar GBR 4:4:4 36bpp, little-endian
        AV_PIX_FMT_GBRP14BE,    ///< planar GBR 4:4:4 42bpp, big-endian
        AV_PIX_FMT_GBRP14LE,    ///< planar GBR 4:4:4 42bpp, little-endian
        AV_PIX_FMT_YUVJ411P,    ///< planar YUV 4:1:1, 12bpp, (1 Cr & Cb sample per 4x1 Y samples) full scale (JPEG), deprecated in favor of AV_PIX_FMT_YUV411P and setting color_range

        AV_PIX_FMT_BAYER_BGGR8,    ///< bayer, BGBG..(odd line), GRGR..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_RGGB8,    ///< bayer, RGRG..(odd line), GBGB..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_GBRG8,    ///< bayer, GBGB..(odd line), RGRG..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_GRBG8,    ///< bayer, GRGR..(odd line), BGBG..(even line), 8-bit samples */
        AV_PIX_FMT_BAYER_BGGR16LE, ///< bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_BGGR16BE, ///< bayer, BGBG..(odd line), GRGR..(even line), 16-bit samples, big-endian */
        AV_PIX_FMT_BAYER_RGGB16LE, ///< bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_RGGB16BE, ///< bayer, RGRG..(odd line), GBGB..(even line), 16-bit samples, big-endian */
        AV_PIX_FMT_BAYER_GBRG16LE, ///< bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_GBRG16BE, ///< bayer, GBGB..(odd line), RGRG..(even line), 16-bit samples, big-endian */
        AV_PIX_FMT_BAYER_GRBG16LE, ///< bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, little-endian */
        AV_PIX_FMT_BAYER_GRBG16BE, ///< bayer, GRGR..(odd line), BGBG..(even line), 16-bit samples, big-endian */
#if !FF_API_XVMC
        AV_PIX_FMT_XVMC,///< XVideo Motion Acceleration via common packet passing
#endif 
        AV_PIX_FMT_YUV440P10LE, ///< planar YUV 4:4:0,20bpp, (1 Cr & Cb sample per 1x2 Y samples), little-endian
        AV_PIX_FMT_YUV440P10BE, ///< planar YUV 4:4:0,20bpp, (1 Cr & Cb sample per 1x2 Y samples), big-endian
        AV_PIX_FMT_YUV440P12LE, ///< planar YUV 4:4:0,24bpp, (1 Cr & Cb sample per 1x2 Y samples), little-endian
        AV_PIX_FMT_YUV440P12BE, ///< planar YUV 4:4:0,24bpp, (1 Cr & Cb sample per 1x2 Y samples), big-endian
        AV_PIX_FMT_AYUV64LE,    ///< packed AYUV 4:4:4,64bpp (1 Cr & Cb sample per 1x1 Y & A samples), little-endian
        AV_PIX_FMT_AYUV64BE,    ///< packed AYUV 4:4:4,64bpp (1 Cr & Cb sample per 1x1 Y & A samples), big-endian

        AV_PIX_FMT_VIDEOTOOLBOX, ///< hardware decoding through Videotoolbox

        AV_PIX_FMT_P010LE, ///< like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, little-endian
        AV_PIX_FMT_P010BE, ///< like NV12, with 10bpp per component, data in the high bits, zeros in the low bits, big-endian

        AV_PIX_FMT_GBRAP12BE,  ///< planar GBR 4:4:4:4 48bpp, big-endian
        AV_PIX_FMT_GBRAP12LE,  ///< planar GBR 4:4:4:4 48bpp, little-endian

        AV_PIX_FMT_GBRAP10BE,  ///< planar GBR 4:4:4:4 40bpp, big-endian
        AV_PIX_FMT_GBRAP10LE,  ///< planar GBR 4:4:4:4 40bpp, little-endian

        AV_PIX_FMT_MEDIACODEC, ///< hardware decoding through MediaCodec

        AV_PIX_FMT_NB,        ///< number of pixel formats, DO NOT USE THIS if you want to link with shared libav* because the number of formats might differ between versions
    };

    public enum AVDiscard
    {
        AVDISCARD_NONE = -16, ///< discard nothing
        AVDISCARD_DEFAULT = 0, ///< discard useless packets like 0 size packets in avi
        AVDISCARD_NONREF = 8, ///< discard all non reference
        AVDISCARD_BIDIR = 16, ///< discard all bidirectional frames
        AVDISCARD_NONINTRA = 24, ///< discard all non intra frames
        AVDISCARD_NONKEY = 32, ///< discard all frames except keyframes
        AVDISCARD_ALL = 48, ///< discard all
    };

    public enum AVAudioServiceType
    {
        AV_AUDIO_SERVICE_TYPE_MAIN = 0,
        AV_AUDIO_SERVICE_TYPE_EFFECTS = 1,
        AV_AUDIO_SERVICE_TYPE_VISUALLY_IMPAIRED = 2,
        AV_AUDIO_SERVICE_TYPE_HEARING_IMPAIRED = 3,
        AV_AUDIO_SERVICE_TYPE_DIALOGUE = 4,
        AV_AUDIO_SERVICE_TYPE_COMMENTARY = 5,
        AV_AUDIO_SERVICE_TYPE_EMERGENCY = 6,
        AV_AUDIO_SERVICE_TYPE_VOICE_OVER = 7,
        AV_AUDIO_SERVICE_TYPE_KARAOKE = 8,
        AV_AUDIO_SERVICE_TYPE_NB, ///< Not part of ABI
    };

    public enum AVFieldOrder
    {
        AV_FIELD_UNKNOWN,
        AV_FIELD_PROGRESSIVE,
        AV_FIELD_TT,          //< Top coded_first, top displayed first
        AV_FIELD_BB,          //< Bottom coded first, bottom displayed first
        AV_FIELD_TB,          //< Top coded first, bottom displayed first
        AV_FIELD_BT,          //< Bottom coded first, top displayed first
    };

    [StructLayout(LayoutKind.Sequential)]
    public class AVPacket
    {
        public IntPtr buf; // AVBufferRef* buf;

        public Int64 pts;

        public Int64 dts;

        public IntPtr data;

        public int size;

        public int stream_index;

        public int flags;

        public IntPtr side_data; // AVPacketSideData* side_data;
        public int side_data_elems;

        public Int64 duration;

        public  Int64 pos;                            ///< byte position in stream, -1 if unknown

#if FF_API_CONVERGENCE_DURATION
    // attribute_deprecated
        public Int64 convergence_duration;
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AVCodecContext
    {
        public IntPtr av_class; // const AVClass* av_class;
        public int log_level_offset;

        AVMediaType codec_type; /* see AVMEDIA_TYPE_xxx */
        IntPtr codec;// const struct AVCodec  *codec;
#if FF_API_CODEC_NAME
        //attribute_deprecated
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string codec_name; // char             codec_name[32];
#endif
        public AVCodecID codec_id; /* see AV_CODEC_ID_xxx */

    
        public uint codec_tag;

#if FF_API_STREAM_CODEC_TAG
    //attribute_deprecated
    uint stream_codec_tag;
#endif

        public IntPtr priv_data;
        public IntPtr @internal;
        public IntPtr opaque; 

        public Int64 bit_rate;

        public int bit_rate_tolerance;

        public int global_quality;

        public int compression_level;

        public const int FF_COMPRESSION_DEFAULT = -1;

        public int flags;

        public int flags2;

        public IntPtr extradata; // uint8_t* extradata;

        public int extradata_size;

        public AVRational time_base;

        public int ticks_per_frame;

        public int delay;

        public int width, height;

        public int coded_width, coded_height;

#if FF_API_ASPECT_EXTENDED
        public const int FF_ASPECT_EXTENDED = 15;
#endif

        public int gop_size;

        public AVPixelFormat pix_fmt;

#if FF_API_MOTION_EST
    // attribute_deprecated 
        public int me_method;
#endif

        public IntPtr draw_horiz_band; //void (*draw_horiz_band)(struct AVCodecContext *s, const AVFrame* src, int offset[AV_NUM_DATA_POINTERS], int y, int type, int height);
        public IntPtr get_format; // enum AVPixelFormat (*get_format)(struct AVCodecContext *s, const enum AVPixelFormat * fmt);

        public int max_b_frames;
        
        public float b_quant_factor;

#if FF_API_RC_STRATEGY
        /** @deprecated use codec private option instead */
        //attribute_deprecated 
        public int rc_strategy;
        public const int FF_RC_STRATEGY_XVID = 1;
#endif

#if FF_API_PRIVATE_OPT
        
        // attribute_deprecated
        public int b_frame_strategy;
#endif
        public float b_quant_offset;

        public int has_b_frames;

#if FF_API_PRIVATE_OPT
        
        // attribute_deprecated
        public int mpeg_quant;
#endif
        public float i_quant_factor;

        public float i_quant_offset;

        public float lumi_masking;

        public float temporal_cplx_masking;

        public float spatial_cplx_masking;

        public float p_masking;

        public float dark_masking;

        public int slice_count;

#if FF_API_PRIVATE_OPT
    // attribute_deprecated
     public int prediction_method;
        public const int FF_PRED_LEFT   = 0;
        public const int FF_PRED_PLANE  = 1;
        public const int FF_PRED_MEDIAN = 2;
#endif

        public IntPtr slice_offset; // int* slice_offset;

        public AVRational sample_aspect_ratio;

        public int me_cmp;

        public int me_sub_cmp;
        
        public int mb_cmp;

        public int ildct_cmp;

        public const int FF_CMP_SAD = 0;
        public const int FF_CMP_SSE = 1;
        public const int FF_CMP_SATD = 2;
        public const int FF_CMP_DCT = 3;
        public const int FF_CMP_PSNR = 4;
        public const int FF_CMP_BIT = 5;
        public const int FF_CMP_RD = 6;
        public const int FF_CMP_ZERO = 7;
        public const int FF_CMP_VSAD = 8;
        public const int FF_CMP_VSSE = 9;
        public const int FF_CMP_NSSE = 10;
        public const int FF_CMP_W53 = 11;
        public const int FF_CMP_W97 = 12;
        public const int FF_CMP_DCTMAX = 13;
        public const int FF_CMP_DCT264 = 14;
        public const int FF_CMP_CHROMA = 256;

        public int dia_size;

        public int last_predictor_count;

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int pre_me;
#endif

        public int me_pre_cmp;

        public int pre_dia_size;

        public int me_subpel_quality;

#if FF_API_AFD
        //attribute_deprecated 
        public int dtg_active_format;
        public const int FF_DTG_AFD_SAME = 8;
        public const int FF_DTG_AFD_4_3 = 9;
        public const int FF_DTG_AFD_16_9 = 10;
        public const int FF_DTG_AFD_14_9 = 11;
        public const int FF_DTG_AFD_4_3_SP_14_9 = 13;
        public const int FF_DTG_AFD_16_9_SP_14_9 = 14;
        public const int FF_DTG_AFD_SP_4_3 = 15;
#endif 
        
        public int me_range;

#if FF_API_QUANT_BIAS
        // attribute_deprecated 
        public int intra_quant_bias;
        public const int FF_DEFAULT_QUANT_BIAS = 999999;

        // attribute_deprecated 
        int inter_quant_bias;
#endif
        int slice_flags;
        public const int SLICE_FLAG_CODED_ORDER = 0x0001;
        public const int SLICE_FLAG_ALLOW_FIELD = 0x0002;
        public const int SLICE_FLAG_ALLOW_PLANE = 0x0004;

#if FF_API_XVMC
        // attribute_deprecated 
        public int xvmc_acceleration;
#endif 

        public int mb_decision;
        const int FF_MB_DECISION_SIMPLE = 0;
        const int FF_MB_DECISION_BITS = 1;
        const int FF_MB_DECISION_RD = 2;

        public IntPtr intra_matrix; // uint16_t* intra_matrix;

        public IntPtr inter_matrix; // uint16_t* inter_matrix;

#if FF_API_PRIVATE_OPT
        //attribute_deprecated
        public int scenechange_threshold;

        //attribute_deprecated
        public int noise_reduction;
#endif

#if FF_API_MPV_OPT
    // // attribute_deprecated
    public int me_threshold;

    // attribute_deprecated
    public int mb_threshold;
#endif
        public int intra_dc_precision;

        public int skip_top;

        public int skip_bottom;

#if FF_API_MPV_OPT
        // attribute_deprecated
        public float border_masking;
#endif
        public int mb_lmin;

        public int mb_lmax;

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int me_penalty_compensation;
#endif
        public int bidir_refine;

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int brd_scale;
#endif

        public int keyint_min;

        public int refs;

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int chromaoffset;
#endif

#if FF_API_UNUSED_MEMBERS
        //attribute_deprecated 
        public int scenechange_factor;
#endif
        public int mv0_threshold;

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int b_sensitivity;
#endif

        public AVColorPrimaries color_primaries;

        public AVColorTransferCharacteristic color_trc;

        public AVColorSpace colorspace;

        public AVColorRange color_range;

        public AVChromaLocation chroma_sample_location;

        public int slices;
        public AVFieldOrder field_order;

        public int sample_rate; ///< samples per second
        public int channels;    ///< number of audio channels

        public AVSampleFormat sample_fmt;  ///< sample format

        public int frame_size;

        public int frame_number;

        public int block_align;

        public int cutoff;

        public UInt64 channel_layout;

        public UInt64 request_channel_layout;

        public AVAudioServiceType audio_service_type;

        public AVSampleFormat request_sample_fmt;

        public IntPtr get_buffer2; // int (*get_buffer2)(struct AVCodecContext *s, AVFrame* frame, int flags);

        public int refcounted_frames;

        public float qcompress;  ///< amount of qscale change between easy & hard scenes (0.0-1.0)
        public float qblur;      ///< amount of qscale smoothing over time (0.0-1.0)

        public int qmin;

        public int qmax;

        public int max_qdiff;

#if FF_API_MPV_OPT
        // attribute_deprecated
        public float rc_qsquish;

        // attribute_deprecated
        public float rc_qmod_amp;
        // attribute_deprecated
        public int rc_qmod_freq;
#endif

        public int rc_buffer_size;

        public int rc_override_count;
        IntPtr rc_override; // RcOverride* rc_override;

#if FF_API_MPV_OPT
        // attribute_deprecated
        IntPtr rc_eq; // const char *rc_eq;
#endif

        public Int64 rc_max_rate;

        public Int64 rc_min_rate;

#if FF_API_MPV_OPT
        /**
         * @deprecated use encoder private options instead
         */
        // attribute_deprecated
        public float rc_buffer_aggressivity;

        // attribute_deprecated
        public float rc_initial_cplx;
#endif

        public float rc_max_available_vbv_use;

        public float rc_min_vbv_overflow_use;

        public int rc_initial_buffer_occupancy;

#if FF_API_CODER_TYPE
        public const int FF_CODER_TYPE_VLC = 0;
        public const int FF_CODER_TYPE_AC = 1;
        public const int FF_CODER_TYPE_RAW = 2;
        public const int FF_CODER_TYPE_RLE = 3;
#if FF_API_UNUSED_MEMBERS
        public const int FF_CODER_TYPE_DEFLATE= 4;
#endif
        // attribute_deprecated
        public int coder_type;
#endif

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int context_model;
#endif

#if FF_API_MPV_OPT
        // attribute_deprecated
        public int lmin;

        // attribute_deprecated
        public int lmax;
#endif

#if FF_API_PRIVATE_OPT
        // attribute_deprecated
        public int frame_skip_threshold;

        // attribute_deprecated
        public int frame_skip_factor;

        // attribute_deprecated
        public int frame_skip_exp;

        // attribute_deprecated
        public int frame_skip_cmp;
#endif

        public int trellis;

#if FF_API_PRIVATE_OPT

        // attribute_deprecated
        public int min_prediction_order;

        // attribute_deprecated
        public int max_prediction_order;

        // attribute_deprecated
        public Int64 timecode_frame_start;
#endif

#if FF_API_RTP_CALLBACK
        // attribute_deprecated
        IntPtr rtp_callback; //  void (*rtp_callback)(struct AVCodecContext *avctx, void *data, int size, int mb_nb);
#endif

#if FF_API_PRIVATE_OPT

        // attribute_deprecated
        public int rtp_payload_size;
#endif

#if FF_API_STAT_BITS
        // attribute_deprecated
        public int mv_bits;
        // attribute_deprecated
        public int header_bits;
        // attribute_deprecated
        public int i_tex_bits;
        // attribute_deprecated
        public int p_tex_bits;
        // attribute_deprecated
        public int i_count;
        // attribute_deprecated
        public int p_count;
        // attribute_deprecated
        public int skip_count;
        // attribute_deprecated
        public int misc_bits;

        // attribute_deprecated
        public int frame_bits;
#endif

        public IntPtr stats_out; // char* stats_out;

        public IntPtr stats_in; // char* stats_in;

        public int workaround_bugs;
        public const int FF_BUG_AUTODETECT = 1;  ///< autodetection
#if FF_API_OLD_MSMPEG4
        public const int FF_BUG_OLD_MSMPEG4 = 2;
#endif
        public const int FF_BUG_XVID_ILACE = 4;
        public const int FF_BUG_UMP4 = 8;
        public const int FF_BUG_NO_PADDING = 16;
        public const int FF_BUG_AMV = 32;
#if FF_API_AC_VLC
        public const int FF_BUG_AC_VLC = 0;
#endif
        public const int FF_BUG_QPEL_CHROMA = 64;
        public const int FF_BUG_STD_QPEL = 128;
        public const int FF_BUG_QPEL_CHROMA2 = 256;
        public const int FF_BUG_DIRECT_BLOCKSIZE = 512;
        public const int FF_BUG_EDGE = 1024;
        public const int FF_BUG_HPEL_CHROMA = 2048;
        public const int FF_BUG_DC_CLIP = 4096;
        public const int FF_BUG_MS = 8192;
        public const int FF_BUG_TRUNCATED = 16384;

        public int strict_std_compliance;
        public const int FF_COMPLIANCE_VERY_STRICT = 2;
        public const int FF_COMPLIANCE_STRICT = 1;
        public const int FF_COMPLIANCE_NORMAL = 0;
        public const int FF_COMPLIANCE_UNOFFICIAL = -1;
        public const int FF_COMPLIANCE_EXPERIMENTAL = -2;

        public int error_concealment;
        public const int FF_EC_GUESS_MVS = 1;
        public const int FF_EC_DEBLOCK = 2;
        public const int FF_EC_FAVOR_INTER = 256;

        public int debug;
        public const int FF_DEBUG_PICT_INFO = 1;
        public const int FF_DEBUG_RC = 2;
        public const int FF_DEBUG_BITSTREAM = 4;
        public const int FF_DEBUG_MB_TYPE = 8;
        public const int FF_DEBUG_QP = 16;
#if FF_API_DEBUG_MV
        public const int FF_DEBUG_MV = 32;
#endif
        public const int FF_DEBUG_DCT_COEFF = 0x00000040;
        public const int FF_DEBUG_SKIP = 0x00000080;
        public const int FF_DEBUG_STARTCODE = 0x00000100;
#if FF_API_UNUSED_MEMBERS
        public const int FF_DEBUG_PTS = 0x00000200;
#endif 
        public const int FF_DEBUG_ER = 0x00000400;
        public const int FF_DEBUG_MMCO = 0x00000800;
        public const int FF_DEBUG_BUGS = 0x00001000;
#if FF_API_DEBUG_MV
        public const int FF_DEBUG_VIS_QP = 0x00002000;
        public const int FF_DEBUG_VIS_MB_TYPE = 0x00004000;
#endif
        public const int FF_DEBUG_BUFFERS = 0x00008000;
        public const int FF_DEBUG_THREADS = 0x00010000;
        public const int FF_DEBUG_GREEN_MD = 0x00800000;
        public const int FF_DEBUG_NOMC = 0x01000000;

#if FF_API_DEBUG_MV    
        public int debug_mv;
        public const int FF_DEBUG_VIS_MV_P_FOR = 0x00000001;
        public const int FF_DEBUG_VIS_MV_B_FOR = 0x00000002;
        public const int FF_DEBUG_VIS_MV_B_BACK = 0x00000004;
#endif

        int err_recognition;

        public const int AV_EF_CRCCHECK = (1 << 0);
        public const int AV_EF_BITSTREAM = (1 << 1);
        public const int AV_EF_BUFFER = (1 << 2);
        public const int AV_EF_EXPLODE = (1 << 3);

        public const int AV_EF_IGNORE_ERR = (1 << 15);
        public const int AV_EF_CAREFUL = (1 << 16);
        public const int AV_EF_COMPLIANT = (1 << 17);
        public const int AV_EF_AGGRESSIVE = (1 << 18);
        
        public Int64 reordered_opaque;
        public IntPtr hwaccel; // struct AVHWAccel *hwaccel;

        public IntPtr hwaccel_context; // void* hwaccel_context

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = AVFrame.AV_NUM_DATA_POINTERS)]
        public UInt64[] error;

        public int dct_algo;
        public const int FF_DCT_AUTO = 0;
        public const int FF_DCT_FASTINT = 1;
        public const int FF_DCT_INT = 2;
        public const int FF_DCT_MMX = 3;
        public const int FF_DCT_ALTIVEC = 5;
        public const int FF_DCT_FAAN = 6;

        public int idct_algo;
        public const int FF_IDCT_AUTO = 0;
        public const int FF_IDCT_INT = 1;
        public const int FF_IDCT_SIMPLE = 2;
        public const int FF_IDCT_SIMPLEMMX = 3;
        public const int FF_IDCT_ARM = 7;
        public const int FF_IDCT_ALTIVEC = 8;
#if FF_API_ARCH_SH4
        public const int FF_IDCT_SH4 = 9;
#endif
        public const int FF_IDCT_SIMPLEARM = 10;
#if FF_API_UNUSED_MEMBERS
        public const int FF_IDCT_IPP = 13;
#endif 
        public const int FF_IDCT_XVID = 14;
#if FF_API_IDCT_XVIDMMX
        public const int FF_IDCT_XVIDMMX = 14;
#endif 
        public const int FF_IDCT_SIMPLEARMV5TE = 16;
        public const int FF_IDCT_SIMPLEARMV6 = 17;
#if FF_API_ARCH_SPARC
        public const int FF_IDCT_SIMPLEVIS = 18;
#endif
        public const int FF_IDCT_FAAN = 20;
        public const int FF_IDCT_SIMPLENEON = 22;
#if FF_API_ARCH_ALPHA
        public const int FF_IDCT_SIMPLEALPHA   =23;
#endif
        public const int FF_IDCT_SIMPLEAUTO = 128;

        public int bits_per_coded_sample;

        public int bits_per_raw_sample;

#if FF_API_LOWRES
     public int lowres;
#endif

#if FF_API_CODED_FRAME
        // attribute_deprecated 
        public IntPtr coded_frame; // AVFrame *coded_frame;
#endif

        public int thread_count;

        public int thread_type;
        public const int FF_THREAD_FRAME = 1;
        public const int FF_THREAD_SLICE = 2;

        public int active_thread_type;

        public int thread_safe_callbacks;

        public IntPtr execute; // int (*execute)(struct AVCodecContext *c, int (*func)(struct AVCodecContext *c2, void* arg), void* arg2, int* ret, int count, int size);

        public IntPtr execute2; // int (*execute2)(struct AVCodecContext *c, int (*func)(struct AVCodecContext *c2, void* arg, int jobnr, int threadnr), void* arg2, int* ret, int count);

        public int nsse_weight;

        public int profile;
        public const int FF_PROFILE_UNKNOWN = -99;
        public const int FF_PROFILE_RESERVED = -100;

        public const int FF_PROFILE_AAC_MAIN = 0;
        public const int FF_PROFILE_AAC_LOW = 1;
        public const int FF_PROFILE_AAC_SSR = 2;
        public const int FF_PROFILE_AAC_LTP = 3;
        public const int FF_PROFILE_AAC_HE = 4;
        public const int FF_PROFILE_AAC_HE_V2 = 28;
        public const int FF_PROFILE_AAC_LD = 22;
        public const int FF_PROFILE_AAC_ELD = 38;
        public const int FF_PROFILE_MPEG2_AAC_LOW = 128;
        public const int FF_PROFILE_MPEG2_AAC_HE = 131;

        public const int FF_PROFILE_DTS = 20;
        public const int FF_PROFILE_DTS_ES = 30;
        public const int FF_PROFILE_DTS_96_24   = 40;
        public const int FF_PROFILE_DTS_HD_HRA  = 50;
        public const int FF_PROFILE_DTS_HD_MA   = 60;
        public const int FF_PROFILE_DTS_EXPRESS = 70;

        public const int FF_PROFILE_MPEG2_422 = 0;
        public const int FF_PROFILE_MPEG2_HIGH = 1;
        public const int FF_PROFILE_MPEG2_SS = 2;
        public const int FF_PROFILE_MPEG2_SNR_SCALABLE = 3;
        public const int FF_PROFILE_MPEG2_MAIN = 4;
        public const int FF_PROFILE_MPEG2_SIMPLE = 5;

        public const int FF_PROFILE_H264_CONSTRAINED = (1 << 9);
        public const int FF_PROFILE_H264_INTRA = (1 << 11);

        public const int FF_PROFILE_H264_BASELINE = 66;
        public const int FF_PROFILE_H264_CONSTRAINED_BASELINE = (66 | FF_PROFILE_H264_CONSTRAINED);
        public const int FF_PROFILE_H264_MAIN = 77;
        public const int FF_PROFILE_H264_EXTENDED = 88;
        public const int FF_PROFILE_H264_HIGH = 100;
        public const int FF_PROFILE_H264_HIGH_10 = 110;
        public const int FF_PROFILE_H264_HIGH_10_INTRA = (110 | FF_PROFILE_H264_INTRA);
        public const int FF_PROFILE_H264_MULTIVIEW_HIGH = 118;
        public const int FF_PROFILE_H264_HIGH_422 = 122;
        public const int FF_PROFILE_H264_HIGH_422_INTRA = (122 | FF_PROFILE_H264_INTRA);
        public const int FF_PROFILE_H264_STEREO_HIGH = 128;
        public const int FF_PROFILE_H264_HIGH_444 = 144;
        public const int FF_PROFILE_H264_HIGH_444_PREDICTIVE = 244;
        public const int FF_PROFILE_H264_HIGH_444_INTRA = (244 | FF_PROFILE_H264_INTRA);
        public const int FF_PROFILE_H264_CAVLC_444 = 44;

        public const int FF_PROFILE_VC1_SIMPLE = 0;
        public const int FF_PROFILE_VC1_MAIN = 1;
        public const int FF_PROFILE_VC1_COMPLEX = 2;
        public const int FF_PROFILE_VC1_ADVANCED = 3;

        public const int FF_PROFILE_MPEG4_SIMPLE = 0;
        public const int FF_PROFILE_MPEG4_SIMPLE_SCALABLE = 1;
        public const int FF_PROFILE_MPEG4_CORE = 2;
        public const int FF_PROFILE_MPEG4_MAIN = 3;
        public const int FF_PROFILE_MPEG4_N_BIT = 4;
        public const int FF_PROFILE_MPEG4_SCALABLE_TEXTURE = 5;
        public const int FF_PROFILE_MPEG4_SIMPLE_FACE_ANIMATION = 6;
        public const int FF_PROFILE_MPEG4_BASIC_ANIMATED_TEXTURE = 7;
        public const int FF_PROFILE_MPEG4_HYBRID = 8;
        public const int FF_PROFILE_MPEG4_ADVANCED_REAL_TIME = 9;
        public const int FF_PROFILE_MPEG4_CORE_SCALABLE = 10;
        public const int FF_PROFILE_MPEG4_ADVANCED_CODING = 11;
        public const int FF_PROFILE_MPEG4_ADVANCED_CORE = 12;
        public const int FF_PROFILE_MPEG4_ADVANCED_SCALABLE_TEXTURE = 13;
        public const int FF_PROFILE_MPEG4_SIMPLE_STUDIO = 14;
        public const int FF_PROFILE_MPEG4_ADVANCED_SIMPLE = 15;

        public const int FF_PROFILE_JPEG2000_CSTREAM_RESTRICTION_0 = 1;
        public const int FF_PROFILE_JPEG2000_CSTREAM_RESTRICTION_1 = 2;
        public const int FF_PROFILE_JPEG2000_CSTREAM_NO_RESTRICTION = 32768;
        public const int FF_PROFILE_JPEG2000_DCINEMA_2K = 3;
        public const int FF_PROFILE_JPEG2000_DCINEMA_4K = 4;

        public const int FF_PROFILE_VP9_0 = 0;
        public const int FF_PROFILE_VP9_1 = 1;
        public const int FF_PROFILE_VP9_2 = 2;
        public const int FF_PROFILE_VP9_3 = 3;

        public const int FF_PROFILE_HEVC_MAIN = 1;
        public const int FF_PROFILE_HEVC_MAIN_10 = 2;
        public const int FF_PROFILE_HEVC_MAIN_STILL_PICTURE = 3;
        public const int FF_PROFILE_HEVC_REXT = 4;

        public int level;
        public const int FF_LEVEL_UNKNOWN = -99;

        public AVDiscard skip_loop_filter;

        public AVDiscard skip_idct;

        public AVDiscard skip_frame;

        public IntPtr subtitle_header; // uint8_t* subtitle_header;
        public int subtitle_header_size;

#if FF_API_ERROR_RATE
    // attribute_deprecated
    public int error_rate;
#endif

#if FF_API_VBV_DELAY
        // attribute_deprecated
        public UInt64 vbv_delay;
#endif

#if FF_API_SIDEDATA_ONLY_PKT
        // attribute_deprecated
        public int side_data_only_packets;
#endif

        public int initial_padding;

        public AVRational framerate;

        public AVPixelFormat sw_pix_fmt;

    /**
     * Timebase in which pkt_dts/pts and AVPacket.dts/pts are.
     * Code outside libavcodec should access this field using:
     * av_codec_{get,set}_pkt_timebase(avctx)
     * - encoding unused.
     * - decoding set by user.
     */
    AVRational pkt_timebase;

        public IntPtr codec_descriptor; // const AVCodecDescriptor* codec_descriptor;

#if !FF_API_LOWRES
        public int lowres;
#endif

        public Int64 pts_correction_num_faulty_pts; /// Number of incorrect PTS values so far
        public Int64 pts_correction_num_faulty_dts; /// Number of incorrect DTS values so far
        public Int64 pts_correction_last_pts;       /// PTS of the last frame
        public Int64 pts_correction_last_dts;       /// DTS of the last frame

        public IntPtr sub_charenc; // char* sub_charenc;

        public int sub_charenc_mode;
        public const int FF_SUB_CHARENC_MODE_DO_NOTHING = -1;
        public const int FF_SUB_CHARENC_MODE_AUTOMATIC = 0;
        public const int FF_SUB_CHARENC_MODE_PRE_DECODER = 1;

        public int skip_alpha;

        public int seek_preroll;

#if !FF_API_DEBUG_MV
        public int debug_mv;
        public const int FF_DEBUG_VIS_MV_P_FOR = 0x00000001;
        public const int FF_DEBUG_VIS_MV_B_FOR = 0x00000002;
        public const int FF_DEBUG_VIS_MV_B_BACK = 0x00000004;
#endif

        public IntPtr chroma_intra_matrix; // uint16_t* chroma_intra_matrix;

        public IntPtr dump_separator; // uint8_t* dump_separator;

        public IntPtr codec_whitelist; // char* codec_whitelist;

        public uint properties;
        public const int FF_CODEC_PROPERTY_LOSSLESS = 0x00000001;
        public const int FF_CODEC_PROPERTY_CLOSED_CAPTIONS = 0x00000002;

        public IntPtr coded_side_data; // AVPacketSideData* coded_side_data;
        public int nb_coded_side_data;

        public IntPtr hw_frames_ctx; // AVBufferRef* hw_frames_ctx;

        public int sub_text_format;
        public const int FF_SUB_TEXT_FMT_ASS = 0;
#if FF_API_ASS_TIMING
        public const int FF_SUB_TEXT_FMT_ASS_WITH_TIMINGS = 1;
#endif
    }

    [StructLayout(LayoutKind.Sequential)]
    public class AVCodec
    {
        public IntPtr name; // const char* name;
        public IntPtr long_name; // const char* long_name;
        public AVMediaType type;
        public AVCodecID id;
        public int capabilities;
        public IntPtr supported_framerates; // const AVRational* supported_framerates; ///< array of supported framerates, or NULL if any, array is terminated by {0,0}
        public IntPtr pix_fmts; //const enum AVPixelFormat *pix_fmts;     ///< array of supported pixel formats, or NULL if unknown, array is terminated by -1
        public IntPtr supported_samplerates; //const int* supported_samplerates;       ///< array of supported audio samplerates, or NULL if unknown, array is terminated by 0
        public IntPtr sample_fmts; // const enum AVSampleFormat *sample_fmts; ///< array of supported sample formats, or NULL if unknown, array is terminated by -1
        public IntPtr channel_layouts; // const uint64_t* channel_layouts;         ///< array of support channel layouts, or NULL if unknown. array is terminated by 0
        public byte max_lowres;                     ///< maximum value for lowres supported by the decoder, no direct access, use av_codec_get_max_lowres()
        public IntPtr priv_class; // const AVClass* priv_class;              ///< AVClass for the private context
        public IntPtr profiles; // const AVProfile* profiles;              ///< array of recognized profiles, or NULL if unknown, array is terminated by {FF_PROFILE_UNKNOWN}

        /*****************************************************************
         * No fields below this line are part of the public API. They
         * may not be used outside of libavcodec and can be changed and
         * removed at will.
         * New public fields should be added right above.
         *****************************************************************
         */
        // ...
    }

    public class AVCodecMethods
    {
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_register_all();
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_encoder(AVCodecID codecID);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_decoder(AVCodecID codecID);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_find_decoder_by_name(string name);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr avcodec_alloc_context3(IntPtr codec);

        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_open2(IntPtr codecCtx, IntPtr codec, IntPtr/* AVDictionary ** */ options);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_close(IntPtr codecCtx);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        // deprecated
        public static extern int avcodec_encode_video2(IntPtr codecCtx, IntPtr pkt, IntPtr frame, out int got_packet_ptr);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        // deprecated
        public static extern int avcodec_decode_video2(IntPtr codecCtx, IntPtr frame, out int got_picture_ptr, IntPtr pkt);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr av_packet_alloc();
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_packet_free(ref IntPtr pkt);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_init_packet(IntPtr pkt);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_packet_unref(IntPtr pkt);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern string avcodec_get_name(AVCodecID id);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void av_packet_rescale_ts(IntPtr pkt, AVRational tb_src, AVRational tb_dst);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern void avcodec_free_context(ref IntPtr codecCtx);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_parameters_from_context(IntPtr par, IntPtr codecCtx);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_encode_audio2(IntPtr codecCtx, IntPtr pkt, IntPtr frame, out int got_packet_ptr);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_decode_audio4(IntPtr avctx, IntPtr frame, out int got_picture_ptr, IntPtr pkt);
        [DllImport("avcodec-57", CallingConvention = CallingConvention.Cdecl)]
        public static extern int avcodec_fill_audio_frame(IntPtr frame, int nb_channels, AVSampleFormat sample_fmt, IntPtr buf, int buf_size, int align);

    }
}
