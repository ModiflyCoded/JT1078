using System;
using System.Collections.Generic;
using System.Text;

namespace JT1078.Protocol.Enums
{
    /// <summary>
    /// 分包处理标记
    /// </summary>
    public enum JT1078SubPackageType : byte
    {
        /// <summary>
        /// Atomic packet (Cannot be split/fragmented)
        /// </summary>
        AtomicPacket = 0,

        /// <summary>
        /// First packet in a fragmented sequence
        /// </summary>
        FirstPacket = 1,

        /// <summary>
        /// Last packet in a fragmented sequence
        /// </summary>
        LastPacket = 2,

        /// <summary>
        /// Intermediate/Middle packet in a fragmented sequence
        /// </summary>
        IntermediatePacket = 3
    }
}
