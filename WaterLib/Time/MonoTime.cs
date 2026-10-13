using System;

namespace Water
{

    public static class MonoTime
    {
        public static long Now { get { return System.Diagnostics.Stopwatch.GetTimestamp(); } }

        public static double SecondsSince(long then)
        {
            if (then <= 0)
                return 0;
            return (Now - then) / (double)System.Diagnostics.Stopwatch.Frequency;
        }

        public static long MillisecondsSince(long then)
        {
            return (long)(SecondsSince(then) * 1000d);
        }
    }

}
