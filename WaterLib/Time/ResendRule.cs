using System;

namespace Water
{

    public static class ResendRule
    {
        public const int SilentMs = 80;
        public const int GapMs = 30;

        public static bool Due(long silentMilliseconds, long sinceLastMilliseconds)
        {
            return silentMilliseconds >= SilentMs && sinceLastMilliseconds >= GapMs;
        }
    }

}
