namespace Water
{
    public static class AmmoNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 19: name = "12 gauge"; return true;
                case 22: name = "5.56"; return true;
                case 27: name = ".44"; return true;
                case 28: name = "7.62"; return true;
                case 29: name = "9mm"; return true;
                default: name = ""; return false;
            }
        }
    }
}
