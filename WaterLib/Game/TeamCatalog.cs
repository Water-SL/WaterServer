using System;

namespace Water
{

    public static class TeamCatalog
    {
        public static string Name(int id)
        {
            switch (id)
            {
                case 0: return "SCPs";
                case 1: return "Foundation Forces";
                case 2: return "Chaos Insurgency";
                case 3: return "Scientists";
                case 4: return "Class-D";
                case 5: return "Dead";
                case 6: return "Other";
                case 7: return "Flamingos";
                default: return "Unknown";
            }
        }
    }

}
