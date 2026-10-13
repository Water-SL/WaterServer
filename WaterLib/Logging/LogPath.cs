using System.IO;

namespace Water
{
    public static class LogPath
    {
        public static string Resolve()
        {
            var path = System.Environment.GetEnvironmentVariable("WATER_LOG");
            if (string.IsNullOrWhiteSpace(path))
                return "";
            return path;
        }
    }
}
