using JT1078.Flv.Extensions;
using JT1078.Protocol;
using JT1078.Protocol.Audio;
using JT1078.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JT1078.Flv.Test
{
    public class FlvAudioTest
    {
        [Fact]
        public void TestEncoderAudioTag()
        {
            FlvEncoder encoder = new FlvEncoder();
            JT1078Package package = new JT1078Package();
            package.Label2 = new JT1078Label2(0, JT1078AVType.G711A);
            package.Label3 = new JT1078Label3(JT1078DataType.AudioFrame, JT1078SubPackageType.AtomicPacket);
            package.Bodies = new byte[] { 0x55, 0x55, 0x55, 0x55 };
            package.Timestamp = 12345678;
            package.SIM = "012345678999";
            package.LogicChannelNumber = 1;

            var audioTag = encoder.EncoderAudioTag(package, true);
            Assert.NotNull(audioTag);
            Assert.True(audioTag.Length > 0);
        }
    }
}
