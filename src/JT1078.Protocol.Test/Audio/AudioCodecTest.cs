using JT1078.Protocol.Audio;
using JT1078.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JT1078.Protocol.Test.Audio
{
    public class AudioCodecTest
    {
        [Fact]
        public void TestG711A()
        {
            AudioCodecFactory factory = new AudioCodecFactory();
            byte[] g711aData = new byte[] { 0x55, 0x55, 0x55, 0x55 }; // dummy data
            var pcm = factory.Encode(JT1078AVType.G711A, g711aData);
            Assert.NotNull(pcm);
            Assert.True(pcm.Length > 0);
        }

        [Fact]
        public void TestADPCM()
        {
            AudioCodecFactory factory = new AudioCodecFactory();
            // JT1078 ADPCM often has a 4-byte HiSilicon header 00 01 52 00
            byte[] adpcmData = new byte[] { 0x00, 0x01, 0x52, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08 };
            var pcm = factory.Encode(JT1078AVType.ADPCM, adpcmData);
            Assert.NotNull(pcm);
            Assert.True(pcm.Length > 0);
        }
    }
}
