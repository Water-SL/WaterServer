using System.Diagnostics;
using System.IO;
using System.Text;

namespace Water.Host
{
    public static class LaunchPlan
    {
        public static ProcessStartInfo Create(WaterSettings settings, string executable, int consolePort, string token)
        {
            var arguments = new StringBuilder();
            arguments.Append("-batchmode -nographics -txbuffer ").Append(settings.TxBuffer);
            arguments.Append(" -rxbuffer ").Append(settings.RxBuffer);
            arguments.Append(" -port").Append(settings.GamePort);
            arguments.Append(" -console").Append(consolePort);
            arguments.Append(" -id").Append(System.Environment.ProcessId);
            arguments.Append(" -heartbeat");
            arguments.Append(" -logFile ").Append(Path.Combine(WaterPaths.Root, "game.log"));

            var start = new ProcessStartInfo
            {
                FileName = executable,
                WorkingDirectory = settings.GameDirectory,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = arguments.ToString()
            };

            start.Environment["WATER_SESSION"] = token;
            start.Environment["WATER_PLUGINS"] = PluginPath.ForHost();
            return start;
        }
    }
}
