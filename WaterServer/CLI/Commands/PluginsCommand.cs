using System;
using Water;

namespace Water.Host
{
    public sealed class PluginsCommand : IHostCommand
    {
        public string Name { get { return "plugins"; } }
        public string Summary { get { return "Yuklu moduller"; } }

        public void Run(WaterApplication app, string line)
        {

            var modules = WaterApi.Modules.Snapshot();
            if (modules.Length == 0)
            {
                app.Log.Add(14, "Modul yok.");
                return;
            }

            for (var i = 0; i < modules.Length; i++)
                app.Log.Add(7, modules[i].Name + "  " + modules[i].Role + "  " + modules[i].Framework);

        }
    }
}
