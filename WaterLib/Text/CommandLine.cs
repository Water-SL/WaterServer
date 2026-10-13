using System;

namespace Water
{

    public static class CommandLine
    {
        public static string NameOf(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return "";
            var text = line.Trim();
            var space = text.IndexOf(' ');
            return space < 0 ? text : text.Substring(0, space);
        }

        public static string Rest(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return "";
            var text = line.Trim();
            var space = text.IndexOf(' ');
            return space < 0 ? "" : text.Substring(space + 1).Trim();
        }

        public static string[] Parts(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return Array.Empty<string>();
            return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

}
