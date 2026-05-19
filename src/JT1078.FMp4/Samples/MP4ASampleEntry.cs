using JT1078.FMp4.Interfaces;
using JT1078.FMp4.MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT1078.FMp4.Samples
{
    public class MP4ASampleEntry : AudioSampleEntry
    {
        public MP4ASampleEntry() : base("mp4a")
        {
        }

        public ElementaryStreamDescriptorBox ESDescriptorBox { get; set; }

        public override void ToBuffer(ref FMp4MessagePackWriter writer)
        {
            Start(ref writer);
            WriterSampleEntryToBuffer(ref writer);
            WriterAudioSampleEntryToBuffer(ref writer);
            if (ESDescriptorBox != null)
            {
                ESDescriptorBox.ToBuffer(ref writer);
            }
            End(ref writer);
        }
    }
}
