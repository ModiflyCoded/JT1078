using JT1078.Hls.Enums;
using JT1078.Protocol;
using JT1078.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JT1078.Hls.Test
{
    public class HlsAudioTest
    {
        [Fact]
        public void TestCreateAudioPES()
        {
            TSEncoder encoder = new TSEncoder();
            JT1078Package package = new JT1078Package();
            package.SIM = "012345678999";
            package.LogicChannelNumber = 1;
            package.Timestamp = 12345678;
            
            byte[] aacFrame = new byte[] { 0x21, 0x22, 0x23, 0x24 }; // dummy AAC frame

            var pes = encoder.CreateAudioPES(package, aacFrame);
            Assert.NotNull(pes);
            Assert.True(pes.Length > 0);
            
            // TS packet size is 188
            Assert.Equal(0, pes.Length % 188);
        }
    }
}
