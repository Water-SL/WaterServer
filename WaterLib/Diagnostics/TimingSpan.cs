using System;

namespace Water
{

    public struct TimingSpan
    {
        public long Start;
        public long End;

        public long Ticks { get { return End - Start; } }

        public double Milliseconds
        {
            get { return Ticks * 1000d / System.Diagnostics.Stopwatch.Frequency; }
        }

        public static TimingSpan Begin()
        {
            return new TimingSpan { Start = System.Diagnostics.Stopwatch.GetTimestamp() };
        }

        public TimingSpan Finish()
        {
            End = System.Diagnostics.Stopwatch.GetTimestamp();
            return this;
        }
    }

}
