namespace Water
{
    public static class FirearmNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 13: name = "COM-15"; return true;
                case 16: name = "Micro HID"; return true;
                case 20: name = "E11-SR"; return true;
                case 21: name = "Crossvec"; return true;
                case 23: name = "FSP-9"; return true;
                case 24: name = "Logicer"; return true;
                case 30: name = "COM-18"; return true;
                case 39: name = "Revolver"; return true;
                case 40: name = "AK"; return true;
                case 41: name = "Shotgun"; return true;
                case 47: name = "Particle Disruptor"; return true;
                case 48: name = "COM-45"; return true;
                case 50: name = "Jailbird"; return true;
                case 52: name = "FR-MG-0"; return true;
                case 53: name = "A7"; return true;
                case 62: name = "SCP-127"; return true;
                case 68: name = "SCP-1509"; return true;
                default: name = ""; return false;
            }
        }
    }
}
