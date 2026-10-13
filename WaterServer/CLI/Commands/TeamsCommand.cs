using System;
using Water;

namespace Water.Host
{
    public sealed class TeamsCommand : IHostCommand
    {
        public string Name { get { return "teams"; } }
        public string Summary { get { return "Takim katalogu"; } }

        public void Run(WaterApplication app, string line)
        {

            for (var i = 0; i <= 7; i++)
                app.Log.Add(7, i + "  " + TeamCatalog.Name(i));

        }
    }
}
