using System;

namespace Water
{

    public sealed class Histogram
    {
        private readonly long[] _bounds;
        private readonly long[] _counts;

        public Histogram(params long[] bounds)
        {
            _bounds = bounds == null || bounds.Length == 0 ? new long[] { 1, 5, 16, 50, 100 } : bounds;
            _counts = new long[_bounds.Length + 1];
        }

        public void Observe(long sample)
        {
            var index = _counts.Length - 1;
            for (var i = 0; i < _bounds.Length; i++)
            {
                if (sample <= _bounds[i])
                {
                    index = i;
                    break;
                }
            }

            System.Threading.Interlocked.Increment(ref _counts[index]);
        }

        public long[] Snapshot()
        {
            var copy = new long[_counts.Length];
            for (var i = 0; i < _counts.Length; i++)
                copy[i] = System.Threading.Interlocked.Read(ref _counts[i]);
            return copy;
        }
    }

}
