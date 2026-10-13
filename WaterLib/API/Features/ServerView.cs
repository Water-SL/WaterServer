using System;

namespace Water
{

    public sealed class ServerView
    {
        public bool Running { get; set; }
        public int Pid { get; set; }
        public double Tps { get; set; }
        public double RelayHz { get; set; }
        public int Players { get; set; }
        public long Tick { get; set; }
        public string Runtime { get; set; } = "";
    }

}
