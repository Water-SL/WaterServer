using System;
using System.IO;

namespace Water.Host
{
    public static class StopSequence
    {
        public static void Request(Action send, Action kill)
        {
            try
            {
                send();
                return;
            }
            catch
            {
            }

            try { kill(); } catch { }
        }
    }
}
