using System;

namespace Water.Patch
{

    public static class ExecutableResolver
    {
        public static string Resolve(string gameDirectory)
        {
            var windows = System.IO.Path.Combine(gameDirectory, "SCPSL.exe");
            if (System.IO.File.Exists(windows))
                return windows;

            var linux = System.IO.Path.Combine(gameDirectory, "SCPSL.x86_64");
            if (System.IO.File.Exists(linux))
                return linux;

            throw new System.IO.FileNotFoundException("SCPSL.exe was not found.", windows);
        }
    }

}
