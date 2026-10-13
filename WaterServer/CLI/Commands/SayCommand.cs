using System;
using Water;

namespace Water.Host
{
    public sealed class SayCommand : IHostCommand
    {
        public string Name { get { return "say"; } }
        public string Summary { get { return "Oyun konsoluna mesaj birakir"; } }

        public void Run(WaterApplication app, string line)
        {

            var rest = CommandLine.Rest(line);
            if (rest.Length == 0)
                throw new System.InvalidOperationException("Kullanim: say mesaj");
            app.Game.Command(rest);

        }
    }
}
