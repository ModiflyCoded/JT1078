using JT1078.Flv.Extensions;
using JT1078.Flv.Test.Utilities;
using JT1078.Protocol;
using JT1078.Protocol.Audio;
using JT1078.Protocol.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JT1078.Flv.Test;

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

    [Fact]
    public async Task TestEncoderAudio__success__fromRealData()
    {
        FlvEncoder encoder = new FlvEncoder();

        var rawTCPBytes = await File.ReadAllBytesAsync("samples/raw-network-packets/tcpdump_1.bin");
        // var outputPath = "output-rawtcp-flv.mp4";
        var outputPath = "output-rawtcp-flv.mov";
        var logFile = "ffmpeg_rawtcp_flv_log.txt";
        // rawTCPBytes = rawTCPBytes.Take(100000).ToArray();
        // rawTCPBytes = rawTCPBytes.Take(500000).ToArray();
        // rawTCPBytes = rawTCPBytes.Take(50000).ToArray();

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }
        if (File.Exists(logFile))
        {
            File.Delete(logFile);
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Path.Combine("ffmpeg", "ffmpeg.exe"),
                Arguments = $"-f flv -i pipe:0 -c copy -movflags +faststart \"{outputPath}\"",
                RedirectStandardInput = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"FFmpeg: {e.Data}");
                File.AppendAllText(logFile, $"{DateTime.Now}: {e.Data}{Environment.NewLine}");
            }
        };
        process.OutputDataReceived += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"FFmpeg Output: {e.Data}");
                File.AppendAllText(logFile, $"{DateTime.Now}: {e.Data}{Environment.NewLine}");
            }
        };

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();


        var index = 0;
        var neverSentVideo = true;
        var neverSentAudio = true;


        ulong audioBasePackageTimestamp = 0;
        ulong audioLastPackageTimestamp = 0;

        while (index < rawTCPBytes.Length)
        {
            if (index + 4 > rawTCPBytes.Length)
            {
                break;
            }

            if (JT1078PacketUtils.IsJT1078Package(rawTCPBytes[index..]) == false)
            {
                index += 4;
                continue;
            }

            var (fullPacketAvailable, packetSize) = JT1078PacketUtils.IsFullPacketAvailable(rawTCPBytes[index..], rawTCPBytes.Length - index);
            if (fullPacketAvailable == false)
            {
                // full packet not yet available, continue reading
                index++;
                continue;
            }

            var packetBytes = new byte[packetSize];
            Array.Copy(rawTCPBytes, index, packetBytes, 0, packetSize);

            // remove the processed packet from the buffer
            index += packetSize;

            var package = JT1078Serializer.Deserialize(packetBytes);
            var fullpackage = JT1078Serializer.Merge(package, JT808ChannelType.Live);

            if (fullpackage == null)
                continue;

            if (fullpackage.Label3.DataType == JT1078DataType.VideoIFrame && neverSentVideo == true)
            {
                var flvVideoBuffer = encoder.EncoderVideoTag(fullpackage, true);
                process.StandardInput.BaseStream.Write(flvVideoBuffer);
                neverSentVideo = false;
            }
            else if (neverSentVideo == false && fullpackage.Label3.DataType != JT1078DataType.AudioFrame)
            {
                var flvVideoBuffer = encoder.EncoderVideoTag(fullpackage, false);
                process.StandardInput.BaseStream.Write(flvVideoBuffer);

            }
            else if (fullpackage.Label3.DataType == JT1078DataType.AudioFrame && neverSentAudio == true)
            {
                var flvAudioBuffer = encoder.EncoderAudioTag(fullpackage, true);
                process.StandardInput.BaseStream.Write(flvAudioBuffer);
                neverSentAudio = false;
            }
            else if (fullpackage.Label3.DataType == JT1078DataType.AudioFrame && neverSentAudio == false)
            {
                var flvAudioBuffer = encoder.EncoderAudioTag(fullpackage, false);
                process.StandardInput.BaseStream.Write(flvAudioBuffer);

            }

        }
        process.StandardInput.BaseStream.Flush();
        process.StandardInput.Close();
        process.WaitForExit(20000);
    }

}

