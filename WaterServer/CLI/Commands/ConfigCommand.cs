using System;
using System.Text;
using Water;

namespace Water.Host
{
    public sealed class ConfigCommand : IHostCommand
    {
        public string Name { get { return "config"; } }
        public string Summary { get { return "Ayari goster veya degistir: config set relayhz 64"; } }

        public void Run(WaterApplication app, string line)
        {
            var parts = CommandLine.Parts(line);
            if (parts.Length >= 4 && string.Equals(parts[1], "set", StringComparison.OrdinalIgnoreCase))
            {
                ConfigEditor.Apply(app, parts[2], parts[3]);
                return;
            }

            var builder = new StringBuilder();
            builder.Append("port ").Append(app.Settings.GamePort);
            builder.Append("  restart ").Append(app.Settings.RestartOnCrash ? "1" : "0");
            app.Log.Add(11, builder.ToString());
            app.Log.Add(7, "oyun " + app.Settings.GameDirectory);
        }
    }
}
