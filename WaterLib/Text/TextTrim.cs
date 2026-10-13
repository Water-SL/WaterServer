using System;

namespace Water
{

    public static class TextTrim
    {
        public static string Limit(string text, int max)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= max)
                return text ?? "";
            if (max <= 1)
                return text.Substring(0, max);
            return text.Substring(0, max - 1) + "…";
        }

        public static string Join(string[] parts, string separator)
        {
            if (parts == null || parts.Length == 0)
                return "";
            return string.Join(separator ?? "", parts);
        }
    }

}
