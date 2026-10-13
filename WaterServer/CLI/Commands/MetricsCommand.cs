using System;
using Water;

namespace Water.Host
{
    public sealed class MetricsCommand : IHostCommand
    {
        public string Name { get { return "metrics"; } }
        public string Summary { get { return "Role sayaclari"; } }

        public void Run(WaterApplication app, string line)
        {

            app.Log.Add(11, "frames " + WaterMeters.Frames.Value
                + "  peak " + WaterMeters.PlayerPeak.Peak
                + "  stalls " + WaterMeters.Stalls.Stalls
                + "  hz " + WaterMeters.RelayRate.PerSecond.ToString("0.0"));

        }
    }
}
