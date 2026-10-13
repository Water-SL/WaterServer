namespace Water
{
    public static class KeycardNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 0: name = "Janitor keycard"; return true;
                case 1: name = "Scientist keycard"; return true;
                case 2: name = "Research Coordinator keycard"; return true;
                case 3: name = "Zone Manager keycard"; return true;
                case 4: name = "Guard keycard"; return true;
                case 5: name = "MTF Private keycard"; return true;
                case 6: name = "Containment Engineer keycard"; return true;
                case 7: name = "MTF Operative keycard"; return true;
                case 8: name = "MTF Captain keycard"; return true;
                case 9: name = "Facility Manager keycard"; return true;
                case 10: name = "Chaos Insurgency keycard"; return true;
                case 11: name = "O5 keycard"; return true;
                case 61: name = "Surface pass"; return true;
                case 63: name = "Custom TF keycard"; return true;
                case 64: name = "Custom Site-02 keycard"; return true;
                case 65: name = "Custom management keycard"; return true;
                case 66: name = "Custom metal case"; return true;
                default: name = ""; return false;
            }
        }
    }
}
