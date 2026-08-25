using JT1078.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT1078.Protocol.Audio
{
    public class AudioCodecFactory
    {
        private readonly AdpcmCodec adpcmCodec = new AdpcmCodec();
        private readonly G711ACodec g711ACodec = new G711ACodec();
        private readonly G711UCodec g711UCodec = new G711UCodec();
        public IAudioEncoder AACEncoder { get; set; }
        //海思芯片编码的音频需要移除海思头，可能还有其他的海思头
        private static byte[] HI = new byte[] { 0x00, 0x01, 0x52, 0x00 };
        public byte[] Encode(JT1078AVType aVType, byte[] bodies)
        {
            byte[] pcm = null;
            switch (aVType)
            {
                case JT1078AVType.ADPCM:
                    // return bodies;
                    ReadOnlySpan<byte> adpcm = bodies;
                    if (adpcm.StartsWith(HI)) adpcm = adpcm.Slice(4);
                    pcm = adpcmCodec.ToPcm(adpcm.Slice(4).ToArray(), new AdpcmState()
                    {
                        Valprev = (short)((adpcm[1] << 8) | adpcm[0]),
                        Index = adpcm[2],
                        Reserved = adpcm[3]
                    });
                    break;
                case JT1078AVType.G711A:
                    pcm = g711ACodec.ToPcm(bodies, null);
                    break;
                case JT1078AVType.G711U:
                    pcm = g711UCodec.ToPcm(bodies, null);
                    break;
                case JT1078AVType.AACLC:
                    //直接AAC出去
                    return bodies;
                case JT1078AVType.MP3:
                    //直接MP3出去
                    return bodies;
                default:
                    return bodies;
            }

            // if (pcm != null && AACEncoder != null)
            // {
            //     return AACEncoder.Encode(pcm);
            // }

            return pcm;
        }
    }
}
