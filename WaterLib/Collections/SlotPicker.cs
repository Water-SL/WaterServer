using System;

namespace Water
{

    public static class SlotPicker
    {
        public static int Pick(int published, int reading, int slots)
        {
            for (var i = 0; i < slots; i++)
            {
                if (i != published && i != reading)
                    return i;
            }

            return 0;
        }
    }

}
