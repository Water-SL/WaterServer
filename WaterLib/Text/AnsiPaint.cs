using System;

namespace Water
{

    public static class AnsiPaint
    {
        public static string ForConsole(int color)
        {
            switch (color)
            {
                case 1: return "\u001b[34m";
                case 2: return "\u001b[32m";
                case 3: return "\u001b[36m";
                case 4: return "\u001b[31m";
                case 9: return "\u001b[94m";
                case 10: return "\u001b[92m";
                case 11: return "\u001b[96m";
                case 12: return "\u001b[91m";
                case 13: return "\u001b[95m";
                case 14: return "\u001b[93m";
                default: return "\u001b[37m";
            }
        }
    }

}
