using System;
using Water;

namespace Water.Host
{
    public sealed class IdleCommand : IHostCommand
    {
        public string Name { get { return "idle"; } }
        public string Summary { get { return "Sunucunun idle bayragi"; } }

        public void Run(WaterApplication app, string line)
        {

            app.Log.Add(11, app.Game.Idle ? "Sunucu idle." : "Sunucu idle degil.");

        }
    }
}
