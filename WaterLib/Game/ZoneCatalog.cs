using System;

namespace Water
{

    public static class ZoneCatalog
    {
        public static string Name(int id)
        {
            switch (id)
            {
                case 1: return "Light Containment";
                case 2: return "Heavy Containment";
                case 3: return "Entrance";
                case 4: return "Surface";
                case 5: return "Other";
                default: return "None";
            }
        }
    }

}
