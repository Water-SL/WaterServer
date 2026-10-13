using System;
using System.Diagnostics;
using System.Threading;

namespace Water
{
    public static class TickClock
    {
        public static long Interval(int hz)
        {
            if (hz < 1)
                hz = 1;
            if (hz > 1000)
                hz = 1000;
            return Stopwatch.Frequency / hz;
        }

        public static void WaitUntil(ref long next, long interval)
        {
            next += interval;
            var remaining = next - Stopwatch.GetTimestamp();
            if (remaining <= 0)
            {
                next = Stopwatch.GetTimestamp();
                return;
            }

            var milliseconds = (int)(remaining * 1000 / Stopwatch.Frequency);
            if (milliseconds > 1)
                Thread.Sleep(milliseconds - 1);

            while (Stopwatch.GetTimestamp() < next)
            {
            }
        }
    }
}
