using System;

namespace Water
{

    public static class LoopbackGuard
    {
        public static bool IsLocal(System.Net.IPAddress address)
        {
            return address != null && System.Net.IPAddress.IsLoopback(address);
        }
    }

}
