namespace Water
{
    public static class ScpRoleNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 0: name = "SCP-173"; return true;
                case 3: name = "SCP-106"; return true;
                case 5: name = "SCP-049"; return true;
                case 7: name = "SCP-079"; return true;
                case 9: name = "SCP-096"; return true;
                case 10: name = "SCP-049-2"; return true;
                case 16: name = "SCP-939"; return true;
                case 23: name = "SCP-3114"; return true;
                default: name = ""; return false;
            }
        }
    }
}
