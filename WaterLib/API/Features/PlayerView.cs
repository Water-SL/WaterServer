using System;

namespace Water
{

    public sealed class PlayerView
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Yaw { get; set; }
        public int Held { get; set; }
        public string HeldName { get { return ItemCatalog.Name(Held); } }
    }

}
