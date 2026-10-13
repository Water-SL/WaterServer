namespace Water
{
    public static class UtilityNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 12: name = "Radio"; return true;
                case 15: name = "Flashlight"; return true;
                case 25: name = "HE grenade"; return true;
                case 26: name = "Flashbang"; return true;
                case 35: name = "Coin"; return true;
                case 54: name = "Lantern"; return true;
                case 56: name = "Snowball"; return true;
                case 57: name = "Coal"; return true;
                case 58: name = "Special coal"; return true;
                case 60: name = "Ragdoll mover"; return true;
                case 67: name = "Marshmallow"; return true;
                default: name = ""; return false;
            }
        }
    }
}
