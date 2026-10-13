using System;

namespace Water
{

    public static class PluginScan
    {
        public static string[] Dlls(string directory)
        {
            if (string.IsNullOrEmpty(directory) || !System.IO.Directory.Exists(directory))
                return Array.Empty<string>();
            return System.IO.Directory.GetFiles(directory, "*.dll");
        }
    }

}
