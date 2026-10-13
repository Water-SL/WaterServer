using System;
using Water;

namespace Water.Host
{
    public sealed class StatusCommand : IHostCommand
    {
        public string Name { get { return "status"; } }
        public string Summary { get { return "TPS, role ve oyuncu sayisi"; } }

        public void Run(WaterApplication app, string line)
        {

            var state = app.Game.Running ? Locale.Get("status") : Locale.Get("stopped");
            app.Log.Add(11, state + "  pid " + app.Game.Pid + "  port " + app.Settings.GamePort);
            app.Log.Add(11, Locale.Get("tps.hint"));

        }
    }
}
