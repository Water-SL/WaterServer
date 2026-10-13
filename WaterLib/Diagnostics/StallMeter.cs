using System;

namespace Water
{

    public sealed class StallMeter
    {
        private long _stalls;

        public long Stalls { get { return System.Threading.Interlocked.Read(ref _stalls); } }

        public void Mark(bool stalled)
        {
            if (stalled)
                System.Threading.Interlocked.Increment(ref _stalls);
        }
    }

}
