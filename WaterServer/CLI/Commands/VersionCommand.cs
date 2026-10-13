using System;
using Water;

namespace Water.Host
{
    public sealed class VersionCommand : IHostCommand
    {
        public string Name { get { return "version"; } }
        public string Summary { get { return "Water surumu ve calisma zamani"; } }

        public void Run(WaterApplication app, string line)
        {

            app.Log.Add(11, WaterProduct.Name + " " + WaterProduct.Version + "  " + WaterRuntime.Framework);

        }
    }
}
