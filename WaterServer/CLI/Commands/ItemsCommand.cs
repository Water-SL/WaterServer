using System;
using Water;

namespace Water.Host
{
    public sealed class ItemsCommand : IHostCommand
    {
        public string Name { get { return "items"; } }
        public string Summary { get { return "Esya katalogu"; } }

        public void Run(WaterApplication app, string line)
        {

            for (var i = -1; i <= 69; i++)
                app.Log.Add(7, i + "  " + ItemCatalog.Name(i));

        }
    }
}
