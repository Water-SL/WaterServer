namespace Water
{
    public static class MedicalNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 14: name = "Medkit"; return true;
                case 17: name = "SCP-500"; return true;
                case 18: name = "SCP-207"; return true;
                case 33: name = "Adrenaline"; return true;
                case 34: name = "Painkillers"; return true;
                case 46: name = "SCP-1853"; return true;
                case 51: name = "Anti SCP-207"; return true;
                default: name = ""; return false;
            }
        }
    }
}
