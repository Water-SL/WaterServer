using System;

namespace Water
{

    public sealed class PluginRecord
    {
        public string Name { get; set; } = "";
        public string Source { get; set; } = "";
        public bool Enabled { get; set; }
        public string Fault { get; set; } = "";
    }

}
