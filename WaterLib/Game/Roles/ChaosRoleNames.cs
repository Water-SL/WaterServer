namespace Water
{
    public static class ChaosRoleNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 1: name = "Class-D"; return true;
                case 8: name = "CI Conscript"; return true;
                case 18: name = "CI Rifleman"; return true;
                case 19: name = "CI Marauder"; return true;
                case 20: name = "CI Repressor"; return true;
                default: name = ""; return false;
            }
        }
    }
}
