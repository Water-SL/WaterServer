using System;

namespace Water.Host
{
    public static class Truth
    {
        public static bool IsOn(string value)
        {
            return value == "1" || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(value, "on", StringComparison.OrdinalIgnoreCase);
        }
    }
}
