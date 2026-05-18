using JT1078.FMp4.Interfaces;
using JT1078.FMp4.MessagePack;
using JT1078.Protocol;
using JT1078.Protocol.H264;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JT1078.FMp4.Test
{
    public class FMp4AudioTest
    {
        [Fact]
        public void TestMoovWithAudio()
        {
            FMp4Encoder encoder = new FMp4Encoder();
            H264NALU sps = new H264NALU();
            // Dummy SPS for 1280x720
            sps.RawData = new byte[] { 0x67, 0x4d, 0x40, 0x1f, 0xec, 0xa0, 0x28, 0x02, 0xdd, 0x80, 0x88, 0x00, 0x00, 0x03, 0x00, 0x08, 0x00, 0x00, 0x03, 0x01, 0x94, 0x78, 0x0c, 0x18, 0x8e };
            H264NALU pps = new H264NALU();
            pps.RawData = new byte[] { 0x68, 0xee, 0x3c, 0x80 };
            
            byte[] aacConfig = new byte[] { 0x12, 0x10 }; // AAC-LC, 44.1kHz, Stereo (just dummy)

            var moov = encoder.MoovBox(sps, pps, aacConfig);
            Assert.NotNull(moov);
            Assert.True(moov.Length > 0);
        }
    }
}
