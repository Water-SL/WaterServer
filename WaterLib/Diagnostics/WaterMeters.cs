using System;

namespace Water
{

    public static class WaterMeters
    {
        public static readonly Counter Frames = new Counter();
        public static readonly Counter Bytes = new Counter();
        public static readonly Gauge Players = new Gauge();
        public static readonly Gauge Toys = new Gauge();
        public static readonly Gauge Items = new Gauge();
        public static readonly RateWindow RelayRate = new RateWindow();
        public static readonly StallMeter Stalls = new StallMeter();
        public static readonly Histogram GapMs = new Histogram(5, 16, 33, 50, 80, 120);
        public static readonly PeakHold PlayerPeak = new PeakHold();

        public static void Frame(int players, int toys, int items, bool stall, int bytes, long nowTicks)
        {
            Frames.Add(1);
            Bytes.Add(bytes);
            Players.Set(players);
            Toys.Set(toys);
            Items.Set(items);
            PlayerPeak.Observe(players);
            Stalls.Mark(stall);
            RelayRate.Mark(nowTicks);
        }
    }

}
