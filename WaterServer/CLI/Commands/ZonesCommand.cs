using System;
using Water;

namespace Water.Host
{
    public sealed class ZonesCommand : IHostCommand
    {
        public string Name { get { return "zones"; } }
        public string Summary { get { return "Bolge katalogu"; } }

        public void Run(WaterApplication app, string line)
        {

            for (var i = 0; i <= 5; i++)
                app.Log.Add(7, i + "  " + ZoneCatalog.Name(i));

        }
    }
}
