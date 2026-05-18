using System;
using System.Collections.Generic;
using System.Text;

namespace JT1078.Protocol.Enums
{
    public enum JT1078DataType : byte
    {
        /// <summary>
        /// Video I-frame (Intra-coded frame / Keyframe)
        /// </summary>
        VideoIFrame = 0,

        /// <summary>
        /// Video P-frame (Predicted frame)
        /// </summary>
        VideoPFrame = 1,

        /// <summary>
        /// Video B-frame (Bi-directional predicted frame)
        /// </summary>
        VideoBFrame = 2,

        /// <summary>
        /// Audio frame
        /// </summary>
        AudioFrame = 3,

        /// <summary>
        /// Transparent data (Pass-through / Custom data)
        /// </summary>
        TransparentData = 4
    }
}
