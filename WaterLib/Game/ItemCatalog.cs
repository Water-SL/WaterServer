using System;

namespace Water
{

    public static class ItemCatalog
    {
        private static readonly string[] _names = Build();

        public static string Name(int id)
        {
            string named;
            if (NameTables.TryItem(id, out named))
                return named;

            var index = id + 1;
            if (index < 0 || index >= _names.Length || _names[index] == null)
                return id < 0 ? "" : ("#" + id.ToString());
            return _names[index];
        }

        private static string[] Build()
        {
            var _names = new string[80];
            _names[0] = "None";
            _names[1] = "Janitor keycard";
            _names[2] = "Scientist keycard";
            _names[3] = "Research Coordinator keycard";
            _names[4] = "Zone Manager keycard";
            _names[5] = "Guard keycard";
            _names[6] = "MTF Private keycard";
            _names[7] = "Containment Engineer keycard";
            _names[8] = "MTF Operative keycard";
            _names[9] = "MTF Captain keycard";
            _names[10] = "Facility Manager keycard";
            _names[11] = "Chaos Insurgency keycard";
            _names[12] = "O5 keycard";
            _names[13] = "Radio";
            _names[14] = "COM-15";
            _names[15] = "Medkit";
            _names[16] = "Flashlight";
            _names[17] = "Micro HID";
            _names[18] = "SCP-500";
            _names[19] = "SCP-207";
            _names[20] = "12 gauge";
            _names[21] = "E11-SR";
            _names[22] = "Crossvec";
            _names[23] = "5.56";
            _names[24] = "FSP-9";
            _names[25] = "Logicer";
            _names[26] = "HE grenade";
            _names[27] = "Flashbang";
            _names[28] = ".44";
            _names[29] = "7.62";
            _names[30] = "9mm";
            _names[31] = "COM-18";
            _names[32] = "SCP-018";
            _names[33] = "SCP-268";
            _names[34] = "Adrenaline";
            _names[35] = "Painkillers";
            _names[36] = "Coin";
            _names[37] = "Light armor";
            _names[38] = "Combat armor";
            _names[39] = "Heavy armor";
            _names[40] = "Revolver";
            _names[41] = "AK";
            _names[42] = "Shotgun";
            _names[43] = "SCP-330";
            _names[44] = "SCP-2176";
            _names[45] = "SCP-244-A";
            _names[46] = "SCP-244-B";
            _names[47] = "SCP-1853";
            _names[48] = "Particle Disruptor";
            _names[49] = "COM-45";
            _names[50] = "SCP-1576";
            _names[51] = "Jailbird";
            _names[52] = "Anti SCP-207";
            _names[53] = "FR-MG-0";
            _names[54] = "A7";
            _names[55] = "Lantern";
            _names[56] = "SCP-1344";
            _names[57] = "Snowball";
            _names[58] = "Coal";
            _names[59] = "Special coal";
            _names[60] = "SCP-1507 tape";
            _names[61] = "Ragdoll mover";
            _names[62] = "Surface pass";
            _names[63] = "SCP-127";
            _names[64] = "Custom TF keycard";
            _names[65] = "Custom Site-02 keycard";
            _names[66] = "Custom management keycard";
            _names[67] = "Custom metal case";
            _names[68] = "Marshmallow";
            _names[69] = "SCP-1509";
            _names[70] = "SCP-021-J";
            return _names;
        }
    }

}
