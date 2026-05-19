using JT1078.FMp4.Interfaces;
using JT1078.FMp4.MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT1078.FMp4
{
    public class SoundMediaHeaderBox : FullBox, IFMp4MessagePackFormatter
    {
        public SoundMediaHeaderBox(byte version=0, uint flags=0) : base("smhd", version, flags)
        {
        }

        public ushort Balance { get; set; }
        public ushort Reserved { get; set; }

        public void ToBuffer(ref FMp4MessagePackWriter writer)
        {
            Start(ref writer);
            WriterFullBoxToBuffer(ref writer);
            writer.WriteUInt16(Balance);
            writer.WriteUInt16(Reserved);
            End(ref writer);
        }
    }
}
