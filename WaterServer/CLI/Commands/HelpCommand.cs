using System;
using Water;

namespace Water.Host
{
    public sealed class HelpCommand : IHostCommand
    {
        public string Name { get { return "help"; } }
        public string Summary { get { return "Komut listesi"; } }

        public void Run(WaterApplication app, string line)
        {
            var commands = CommandBus.Snapshot();
            Array.Sort(commands, (left, right) => string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase));
            app.Log.Add(11, "Water komutları");
            for (var i = 0; i < commands.Length; i++)
                app.Log.Add(7, "  " + commands[i].Name.PadRight(14) + commands[i].Summary);
            app.Log.Add(7, "  setup         ilk kurulum soruları, yalnızca CLI");
            app.Log.Add(7, "  exit          Water sürecini kapat, yalnızca CLI");
            app.Log.Add(7, "  diğer satırlar oyun konsoluna gider");
        }
    }
}
