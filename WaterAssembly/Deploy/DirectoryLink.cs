using System;

namespace Water.Patch
{

    public static class DirectoryLink
    {
        public static void LinkTree(string sourceData, string targetData)
        {
            foreach (var directory in System.IO.Directory.GetDirectories(sourceData))
            {
                var name = System.IO.Path.GetFileName(directory);
                if (string.Equals(name, "Managed", StringComparison.OrdinalIgnoreCase))
                    continue;

                var link = System.IO.Path.Combine(targetData, name);
                if (System.IO.Directory.Exists(link))
                    continue;

                Link(link, directory);
            }

            foreach (var file in System.IO.Directory.GetFiles(sourceData))
            {
                var name = System.IO.Path.GetFileName(file);
                var destination = System.IO.Path.Combine(targetData, name);
                if (string.Equals(name, "ScriptingAssemblies.json", StringComparison.OrdinalIgnoreCase))
                {
                    FileSync.ReplaceOwned(file, destination);
                    continue;
                }

                if (System.IO.File.Exists(destination))
                    continue;

                if (!TryFile(destination, file))
                    System.IO.File.Copy(file, destination, false);
            }
        }

        public static void Link(string link, string target)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c mklink /J \"" + link + "\" \"" + target + "\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                });
                process.WaitForExit();
                if (!System.IO.Directory.Exists(link))
                    throw new System.IO.IOException("Could not create a junction for " + link + ". " + process.StandardError.ReadToEnd());
                return;
            }

            var unix = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ln",
                Arguments = "-s \"" + target + "\" \"" + link + "\"",
                CreateNoWindow = true,
                UseShellExecute = false
            });
            unix.WaitForExit();
            if (!System.IO.Directory.Exists(link))
                throw new System.IO.IOException("Could not symlink " + link);
        }

        public static bool TryFile(string link, string target)
        {
            try
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (CreateHardLink(link, target, System.IntPtr.Zero) && System.IO.File.Exists(link))
                        return true;
                    if (CreateSymbolicLink(link, target, 0) && System.IO.File.Exists(link))
                        return true;
                    return false;
                }

                var process = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "ln",
                    Arguments = "-s \"" + target + "\" \"" + link + "\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                process.WaitForExit();
                return process.ExitCode == 0 && System.IO.File.Exists(link);
            }
            catch
            {
                return false;
            }
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateHardLink(string link, string existing, System.IntPtr security);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateSymbolicLink(string link, string target, int flags);
    }

}
