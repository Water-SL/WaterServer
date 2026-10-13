using System;
using Water;

namespace Water.Host
{
    public sealed class HooksCommand : IHostCommand
    {
        public string Name { get { return "hooks"; } }
        public string Summary { get { return "Yama kancalari"; } }

        public void Run(WaterApplication app, string line)
        {

            var sites = Water.Patch.HookCatalog.Sites;
            for (var i = 0; i < sites.Length; i++)
                app.Log.Add(7, sites[i].Key + "  " + sites[i].Hook);
            app.Log.Add(11, "pipeline " + Water.Patch.HookPipeline.Count);

        }
    }
}
