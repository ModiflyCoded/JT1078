using JT1078.FMp4.Interfaces;
using JT1078.FMp4.MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT1078.FMp4
{
    /// <summary>
    /// esds
    /// </summary>
    public class ElementaryStreamDescriptorBox : FullBox, IFMp4MessagePackFormatter
    {
        public ElementaryStreamDescriptorBox(byte version, uint flags) : base("esds", version, flags)
        {
        }

        /// <summary>
        /// AudioSpecificConfig
        /// </summary>
        public byte[] Config { get; set; }

        public void ToBuffer(ref FMp4MessagePackWriter writer)
        {
            Start(ref writer);
            WriterFullBoxToBuffer(ref writer);

            // ES_Descriptor
            writer.WriteByte(0x03); // tag
            uint esLen = (uint)(3 + 5 + 13 + (Config?.Length ?? 0));
            WriteDescriptorLength(ref writer, esLen);
            writer.WriteUInt16(0x0001); // ES_ID
            writer.WriteByte(0x00); // flags

            // DecoderConfigDescriptor
            writer.WriteByte(0x04); // tag
            uint decConfigLen = (uint)(13 + (Config?.Length ?? 0));
            WriteDescriptorLength(ref writer, decConfigLen);
            writer.WriteByte(0x40); // objectTypeIndication (MPEG-4 Audio)
            writer.WriteByte(0x15); // streamType (AudioStream)
            writer.WriteUInt24(0); // bufferSizeDB
            writer.WriteUInt32(0); // maxBitrate
            writer.WriteUInt32(0); // avgBitrate

            // DecoderSpecificInfo
            if (Config != null && Config.Length > 0)
            {
                writer.WriteByte(0x05); // tag
                WriteDescriptorLength(ref writer, (uint)Config.Length);
                writer.WriteArray(Config);
            }

            // SLConfigDescriptor
            writer.WriteByte(0x06); // tag
            WriteDescriptorLength(ref writer, 1);
            writer.WriteByte(0x02); // predefined

            End(ref writer);
        }

        private void WriteDescriptorLength(ref FMp4MessagePackWriter writer, uint length)
        {
            if (length >= 0x200000) writer.WriteByte((byte)((length >> 21) | 0x80));
            if (length >= 0x4000) writer.WriteByte((byte)((length >> 14) | 0x80));
            if (length >= 0x80) writer.WriteByte((byte)((length >> 7) | 0x80));
            writer.WriteByte((byte)(length & 0x7F));
        }
    }
}
