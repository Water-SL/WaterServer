namespace Water
{
    public static class ScpObjectNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 31: name = "SCP-018"; return true;
                case 32: name = "SCP-268"; return true;
                case 42: name = "SCP-330"; return true;
                case 43: name = "SCP-2176"; return true;
                case 44: name = "SCP-244-A"; return true;
                case 45: name = "SCP-244-B"; return true;
                case 49: name = "SCP-1576"; return true;
                case 55: name = "SCP-1344"; return true;
                case 59: name = "SCP-1507 tape"; return true;
                case 69: name = "SCP-021-J"; return true;
                default: name = ""; return false;
            }
        }
    }
}
