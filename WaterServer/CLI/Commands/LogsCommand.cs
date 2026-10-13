using System;
using Water;

namespace Water.Host
{
    public sealed class LogsCommand : IHostCommand
    {
        public string Name { get { return "logs"; } }
        public string Summary { get { return "Konsol tamponunun boyu"; } }

        public void Run(WaterApplication app, string line)
        {

            app.Log.Add(11, "Konsol tamponu " + app.Log.Recent().Length + " satir.");

        }
    }
}
