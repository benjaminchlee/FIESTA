using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VPx.Decoder
{
    public struct DecoderConst
    {
        public const int VPX_DECODER_ABI_VERSION = 3 + VPxConst.VPX_CODEC_ABI_VERSION;
    }
    public struct vpx_codec_dec_cfg
    {
        public uint threads; /**< Maximum number of threads to use, default 1 */
        public uint w;       /**< Width */
        public uint h;       /**< Height */
    }

}
