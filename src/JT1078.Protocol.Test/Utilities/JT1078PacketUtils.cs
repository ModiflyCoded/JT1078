
using System;
using System.Buffers.Binary;
using JT1078.Protocol;
using JT1078.Protocol.Enums;

namespace JT1078.Protocol.Test.Utilities;

public static class JT1078PacketUtils
{
    public static bool IsJT1078Package(byte[] data)
    {
        if (data.Length < 4)
        {
            return false;
        }

        uint header = BitConverter.ToUInt32(data, 0);
        if (BitConverter.IsLittleEndian)
        {
            header = BinaryPrimitives.ReverseEndianness(header);
        }

        if (header == JT1078Package.FH)
        {
            return true;
        }
        return false;
    }

    public static Tuple<bool, int> IsFullPacketAvailable(byte[] data, int dataLength)
    {
        try
        {
            if (dataLength < 30)
            {
                return Tuple.Create(false, 0);
            }

            int readerIndex = 0;

            // skip straight to DataType
            readerIndex += 15;

            // get data type byte
            byte dataType = data[readerIndex];
            readerIndex += 1;

            //  data type determines which fields are present
            JT1078Label3 label3 = new JT1078Label3(dataType);
            int nextLengthToSkip = 0;
            if (label3.DataType != JT1078DataType.TransparentData) // 0100: Transparently transmit data
            {
                // timestamp is present and 8 bytes long
                nextLengthToSkip += 8;
            }

            // if video frame, skip 4 bytes of additional info.
            // Last I Frame Interval and Last Frame Interval  
            if (label3.DataType == JT1078DataType.VideoIFrame ||
                label3.DataType == JT1078DataType.VideoPFrame ||
                label3.DataType == JT1078DataType.VideoBFrame)
            {
                nextLengthToSkip += 4;
            }
            readerIndex += nextLengthToSkip;

            ushort bodyLength = BitConverter.ToUInt16(data, readerIndex);
            if (BitConverter.IsLittleEndian)
            {
                bodyLength = BinaryPrimitives.ReverseEndianness(bodyLength);
            }

            readerIndex += 2;

            int packageLength = readerIndex + bodyLength;

            if (dataLength >= packageLength)
            {
                return Tuple.Create(true, packageLength);
            }
            return Tuple.Create(false, 0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking JT1078 package: {ex.Message}");
            return Tuple.Create(false, 0);
        }

    }
}