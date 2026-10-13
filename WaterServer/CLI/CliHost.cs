using System;
using System.IO;
using System.Threading;
using Water;

namespace Water.Host
{
    public static class CliHost
    {
        public static void Run(WaterApplication app, CancellationToken token)
        {
            Prepare(app);
            Banner(app);
            app.Log.Written += Print;
            var recent = app.Log.Recent();
            for (var i = 0; i < recent.Length; i++)
                Print(recent[i]);

            while (!token.IsCancellationRequested)
            {
                Console.Write("\u001b[38;2;0;212;255mwater\u001b[0m › ");
                string? input;
                try
                {
                    input = Console.ReadLine();
                }
                catch (IOException)
                {
                    return;
                }

                if (input == null)
                    return;

                if (!Handle(app, input.Trim()))
                    return;
            }
        }

        private static bool Handle(WaterApplication app, string input)
        {
            if (input.Length == 0)
                return true;

            var name = input.Split(' ')[0];
            if (string.Equals(name, "exit", StringComparison.OrdinalIgnoreCase) || string.Equals(name, "quit", StringComparison.OrdinalIgnoreCase))
                return false;

            if (string.Equals(name, "setup", StringComparison.OrdinalIgnoreCase))
            {
                Setup(app);
                return true;
            }

            try
            {
                app.Dispatch(input);
            }
            catch (Exception ex)
            {
                app.Log.Add(12, ex.Message);
            }

            return true;
        }

        private static void Prepare(WaterApplication app)
        {
            var settings = app.Settings;
            if (settings.Language != "en" && settings.Language != "tr")
            {
                Console.WriteLine();
                Console.WriteLine("Language / Dil");
                Console.WriteLine("  1  English");
                Console.WriteLine("  2  Turkce");
                Console.Write("> ");
                var pick = Console.ReadLine();
                settings.Language = pick != null && pick.Trim() == "2" ? "tr" : "en";
            }

            Locale.Use(settings.Language);
            var directory = string.IsNullOrWhiteSpace(settings.GameDirectory) ? WaterApplication.SuggestedGame() : settings.GameDirectory;
            var installed = WaterInstalled(directory);
            if (!installed)
            {
                Console.WriteLine(Locale.Get("missing"));
                directory = Ask(Locale.Get("folder.ask"), directory);
                installed = WaterInstalled(directory);
            }
            else
            {
                Console.WriteLine(Locale.Get("installed"));
            }

            settings.GameDirectory = directory;
            settings.GamePort = AskInt(Locale.Get("port.ask"), settings.GamePort == 0 ? 7777 : settings.GamePort);
            settings.Configured = true;
            if (string.IsNullOrEmpty(settings.BindAddress))
                settings.BindAddress = "127.0.0.1";
            app.SaveSettings();

            if (!installed && !WaterInstalled(directory))
            {
                Console.WriteLine(Locale.Get("starting"));
            }

            try
            {
                Console.WriteLine(Locale.Get("starting"));
                app.Game.Start();
                Console.WriteLine(Locale.Get("ready"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool WaterInstalled(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
                return false;

            return System.IO.File.Exists(System.IO.Path.Combine(directory, "WaterSL.exe"))
                || System.IO.File.Exists(System.IO.Path.Combine(directory, "WaterSL.x86_64"));
        }

        private static void Setup(WaterApplication app)
        {
            var current = app.Settings;
            var directory = Ask("Dedicated server folder", string.IsNullOrEmpty(current.GameDirectory) ? WaterApplication.SuggestedGame() : current.GameDirectory);
            current.GameDirectory = directory;
            current.GamePort = AskInt("Game port", current.GamePort == 0 ? 7777 : current.GamePort);
            current.Configured = true;
            if (string.IsNullOrEmpty(current.BindAddress))
                current.BindAddress = "127.0.0.1";
            app.SaveSettings();
            app.Log.Add(10, "Saved.");
        }

        private static string Ask(string label, string current)
        {
            Console.Write(label + (current.Length == 0 ? ": " : " [" + current + "]: "));
            var value = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(value))
                return current;
            return value.Trim();
        }

        private static int AskInt(string label, int current)
        {
            int parsed;
            var text = Ask(label, current.ToString());
            return int.TryParse(text, out parsed) ? parsed : current;
        }

        private static void Banner(WaterApplication app)
        {
            Console.WriteLine();
            Console.WriteLine("\u001b[38;2;0;212;255m  WATER\u001b[0m  " + WaterProduct.Version);
            Console.WriteLine("  SCP: Secret Laboratory");
            Console.WriteLine("  " + Locale.Get("tps.hint"));
            Console.WriteLine();
        }

        private static void Print(LogLine line)
        {
            Console.WriteLine("\u001b[38;2;90;110;120m" + line.Time.ToString("HH:mm:ss") + "\u001b[0m " + AnsiPaint.ForConsole(line.Color) + line.Text + "\u001b[0m");
        }
    }
}
