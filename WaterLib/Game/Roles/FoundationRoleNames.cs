namespace Water
{
    public static class FoundationRoleNames
    {
        public static bool Try(int id, out string name)
        {
            switch (id)
            {
                case 4: name = "NTF Specialist"; return true;
                case 6: name = "Scientist"; return true;
                case 11: name = "NTF Sergeant"; return true;
                case 12: name = "NTF Captain"; return true;
                case 13: name = "NTF Private"; return true;
                case 15: name = "Facility Guard"; return true;
                default: name = ""; return false;
            }
        }
    }
}
