using System;

namespace Water
{

    public static class WaterChannels
    {
        public static readonly EventHub<DisconnectInfo> Disconnected = new EventHub<DisconnectInfo>();
        public static readonly EventHub<HeartbeatInfo> Heartbeats = new EventHub<HeartbeatInfo>();
        public static readonly EventHub<CaptureStats> Captures = new EventHub<CaptureStats>();
        public static readonly EventHub<ConfigChange> ConfigChanged = new EventHub<ConfigChange>();
        public static readonly EventHub<PluginFaultInfo> PluginFaults = new EventHub<PluginFaultInfo>();

        public static void Capture(long tick, int players, int toys, int items)
        {
            Captures.Publish(new CaptureStats { Tick = tick, Players = players, Toys = toys, Items = items });
        }
    }

}
