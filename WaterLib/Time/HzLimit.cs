using System;

namespace Water
{

    public static class HzLimit
    {
        public const int Min = 10;
        public const int Max = 128;

        public static int Clamp(int hz)
        {
            if (hz < Min)
                return Min;
            if (hz > Max)
                return Max;
            return hz;
        }

        public static void Check(int hz)
        {
            if (hz < Min || hz > Max)
                throw new ArgumentOutOfRangeException(nameof(hz), "Röle hızı 10 ile 128 Hz arasında olmalı.");
        }
    }

}
