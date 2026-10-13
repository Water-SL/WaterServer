namespace Water
{
    public static class NameTables
    {
        public static bool TryItem(int id, out string name)
        {
            if (KeycardNames.Try(id, out name) || FirearmNames.Try(id, out name) || MedicalNames.Try(id, out name) || AmmoNames.Try(id, out name) || ArmorNames.Try(id, out name) || ScpObjectNames.Try(id, out name) || UtilityNames.Try(id, out name))
                return true;
            name = "";
            return false;
        }

        public static bool TryRole(int id, out string name)
        {
            if (ScpRoleNames.Try(id, out name) || FoundationRoleNames.Try(id, out name) || ChaosRoleNames.Try(id, out name) || SpecialRoleNames.Try(id, out name))
                return true;
            name = "";
            return false;
        }
    }
}
