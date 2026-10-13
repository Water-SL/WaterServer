using System;

namespace Water
{

    public sealed class RateWindow
    {
        private readonly object _gate = new object();
        private long _start;
        private int _count;
        private double _last;

        public double PerSecond { get { lock (_gate) return _last; } }

        public void Mark(long nowTicks)
        {
            lock (_gate)
            {
                if (_start == 0)
                    _start = nowTicks;
                _count++;
                var elapsed = (nowTicks - _start) / (double)System.Diagnostics.Stopwatch.Frequency;
                if (elapsed < 1d)
                    return;
                _last = _count / elapsed;
                _count = 0;
                _start = nowTicks;
            }
        }
    }

}
