using System;

namespace Water
{

    public sealed class PeakHold
    {
        private int _peak;

        public int Peak { get { return System.Threading.Volatile.Read(ref _peak); } }

        public void Observe(int value)
        {
            while (true)
            {
                var current = System.Threading.Volatile.Read(ref _peak);
                if (value <= current)
                    return;
                if (System.Threading.Interlocked.CompareExchange(ref _peak, value, current) == current)
                    return;
            }
        }
    }

}
