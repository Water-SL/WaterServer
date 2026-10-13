namespace Water
{
    public static class SpecialRoleNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case -1: name = "None"; return true;
                case 2: name = "Spectator"; return true;
                case 14: name = "Tutorial"; return true;
                case 17: name = "Custom"; return true;
                case 21: name = "Overwatch"; return true;
                case 22: name = "Filmmaker"; return true;
                case 24: name = "Destroyed"; return true;
                case 25: name = "Flamingo"; return true;
                case 26: name = "Alpha Flamingo"; return true;
                case 27: name = "Zombie Flamingo"; return true;
                case 28: name = "NTF Flamingo"; return true;
                case 29: name = "Chaos Flamingo"; return true;
                default: name = ""; return false;
            }
        }
    }
}
