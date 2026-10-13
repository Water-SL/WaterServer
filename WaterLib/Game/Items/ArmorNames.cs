namespace Water
{
    public static class ArmorNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 36: name = "Light armor"; return true;
                case 37: name = "Combat armor"; return true;
                case 38: name = "Heavy armor"; return true;
                default: name = ""; return false;
            }
        }
    }
}
