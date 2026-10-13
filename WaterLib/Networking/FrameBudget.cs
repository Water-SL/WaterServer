using System;

namespace Water
{

    public static class FrameBudget
    {
        public const int MaxPacket = 65535;
        public const int MaxName = 64;
        public const int Header = 28;

        public static bool Fits(int offset, int needed, int length)
        {
            return offset >= 0 && needed >= 0 && offset <= length - needed;
        }
    }

}
