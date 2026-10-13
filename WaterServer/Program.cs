using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Water;

namespace Water.Host
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            EnableColor();
            if (Has(args, "--self-test"))
                return SelfTest();

            var store = new SettingsStore();
            var settings = store.Load();
            using var app = new WaterApplication(settings, store);
            using var cancel = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancel.Cancel();
            };

            CliHost.Run(app, cancel.Token);
            return 0;
        }

        private static int SelfTest()
        {
            var home = Path.Combine(Path.GetTempPath(), "water-selftest-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(home);
            Environment.SetEnvironmentVariable("WATER_HOME", home);
            try
            {
                var store = new SettingsStore();
                var settings = new WaterSettings
                {
                    Configured = true,
                    GamePort = 7777,
                    BindAddress = "127.0.0.1"
                };
                store.Save(settings);
                using var app = new WaterApplication(settings, store);
                app.Dispatch("version");
                var recent = app.Log.Recent();
                for (var i = 0; i < recent.Length; i++)
                {
                    if (recent[i].Text.IndexOf(WaterProduct.Name, StringComparison.Ordinal) >= 0)
                    {
                        Console.WriteLine("Water self-test ok.");
                        return 0;
                    }
                }

                throw new InvalidOperationException("version command did not log.");
            }
            finally
            {
                Environment.SetEnvironmentVariable("WATER_HOME", null);
                try { Directory.Delete(home, true); } catch { }
            }
        }

        private static bool Has(string[] args, string name)
        {
            for (var i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static void EnableColor()
        {
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
                return;

            try
            {
                var handle = GetStdHandle(-11);
                uint mode;
                if (GetConsoleMode(handle, out mode))
                    SetConsoleMode(handle, mode | 0x0004);
            }
            catch
            {
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(int handle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr handle, out uint mode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr handle, uint mode);
    }
}
