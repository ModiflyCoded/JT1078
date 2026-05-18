using System;
using System.Collections.Generic;
using System.Linq;
using JT1078.Protocol.Enums;

namespace JT1078.Protocol
{
    public class JT1078FrameReassembler
    {
        private readonly Dictionary<ulong, FrameContext> _frames = new();
        private const int MaxPendingFrames = 10;

        public JT1078Package Merge(JT1078Package pkg)
        {
            if (pkg.Label3.SubpackageType == JT1078SubPackageType.AtomicPacket)
                return pkg;

            lock (_frames)
            {
                var staleKeys = _frames.Keys.Where(t => t < pkg.Timestamp - 5000).ToList();
                foreach (var key in staleKeys)
                {
                    _frames.Remove(key);
                }

                if (_frames.Count > MaxPendingFrames && !_frames.ContainsKey(pkg.Timestamp))
                {
                    var oldestKey = _frames.Keys.Min();
                    _frames.Remove(oldestKey);
                }

                if (!_frames.TryGetValue(pkg.Timestamp, out var frame))
                {
                    frame = new FrameContext();
                    _frames[pkg.Timestamp] = frame;
                }

                frame.Add(pkg);

                if (frame.IsComplete())
                {
                    _frames.Remove(pkg.Timestamp);
                    return frame.GetMergedPackage();
                }

                return default;
            }
        }

        private class FrameContext
        {
            private readonly Dictionary<ushort, JT1078Package> _packets = new();
            private ushort? _firstSN;
            private ushort? _lastSN;

            public void Add(JT1078Package pkg)
            {
                _packets[pkg.SN] = pkg;
                if (pkg.Label3.SubpackageType == JT1078SubPackageType.FirstPacket) _firstSN = pkg.SN;
                if (pkg.Label3.SubpackageType == JT1078SubPackageType.LastPacket) _lastSN = pkg.SN;
            }

            public bool IsComplete()
            {
                if (!_firstSN.HasValue || !_lastSN.HasValue) return false;
                int expectedCount = (_lastSN.Value - _firstSN.Value + 65536) % 65536 + 1;
                return _packets.Count >= expectedCount;
            }

            public JT1078Package GetMergedPackage()
            {
                var sorted = _packets.Values
                    .OrderBy(p => (p.SN - _firstSN.Value + 65536) % 65536)
                    .ToList();

                var templatePacket = sorted.First();
                var totalLength = sorted.Sum(p => p.Bodies.Length);

                byte[] poolBytes = JT1078ArrayPool.Rent(totalLength);
                Span<byte> span = poolBytes;

                int offset = 0;
                foreach (var p in sorted)
                {
                    p.Bodies.CopyTo(span.Slice(offset));
                    offset += p.Bodies.Length;
                }

                templatePacket.Bodies = span.Slice(0, totalLength).ToArray();
                JT1078ArrayPool.Return(poolBytes);

                return templatePacket;
            }
        }
    }
}
