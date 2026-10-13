using System;

namespace Water
{

    public static class RoleCatalog
    {
        private static readonly string[] Names = Build();

        public static string Name(int id)
        {
            string named;
            if (NameTables.TryRole(id, out named))
                return named;

            var index = id + 1;
            if (index < 0 || index >= Names.Length || Names[index] == null)
                return "#" + id.ToString();
            return Names[index];
        }

        public static string Team(int id)
        {
            switch (id)
            {
                case 0: case 3: case 5: case 7: case 9: case 10: case 16: case 23: return "SCPs";
                case 4: case 11: case 12: case 13: case 15: return "Foundation";
                case 8: case 18: case 19: case 20: return "Chaos";
                case 6: return "Scientists";
                case 1: return "Class-D";
                case 2: case 21: case 24: return "Dead";
                default:
                    if (id >= 25 && id <= 29) return "Flamingos";
                    return "Other";
            }
        }

        private static string[] Build()
        {
            var map = new string[40];
            map[0] = "None";
            map[1] = "SCP-173";
            map[2] = "Class-D";
            map[3] = "Spectator";
            map[4] = "SCP-106";
            map[5] = "NTF Specialist";
            map[6] = "SCP-049";
            map[7] = "Scientist";
            map[8] = "SCP-079";
            map[9] = "CI Conscript";
            map[10] = "SCP-096";
            map[11] = "SCP-049-2";
            map[12] = "NTF Sergeant";
            map[13] = "NTF Captain";
            map[14] = "NTF Private";
            map[15] = "Tutorial";
            map[16] = "Facility Guard";
            map[17] = "SCP-939";
            map[18] = "Custom";
            map[19] = "CI Rifleman";
            map[20] = "CI Marauder";
            map[21] = "CI Repressor";
            map[22] = "Overwatch";
            map[23] = "Filmmaker";
            map[24] = "SCP-3114";
            map[25] = "Destroyed";
            map[26] = "Flamingo";
            map[27] = "Alpha Flamingo";
            map[28] = "Zombie Flamingo";
            map[29] = "NTF Flamingo";
            map[30] = "Chaos Flamingo";
            return map;
        }
    }

}
