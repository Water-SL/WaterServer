using System;
using Water;

namespace Water.Host
{
    public sealed class RolesCommand : IHostCommand
    {
        public string Name { get { return "roles"; } }
        public string Summary { get { return "Rol katalogu"; } }

        public void Run(WaterApplication app, string line)
        {

            for (var i = -1; i <= 29; i++)
                app.Log.Add(7, i + "  " + RoleCatalog.Name(i) + "  " + RoleCatalog.Team(i));

        }
    }
}
