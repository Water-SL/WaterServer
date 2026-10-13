using System;

namespace Water
{

    public static class HeartbeatRule
    {
        public const int SilentSeconds = 40;
        public const int GraceSeconds = 120;

        public static bool ShouldRestart(bool sawHeartbeat, double silentSeconds, double uptimeSeconds)
        {
            return sawHeartbeat && silentSeconds > SilentSeconds && uptimeSeconds > 5;
        }

        public static bool Waiting(bool sawHeartbeat, double uptimeSeconds)
        {
            return !sawHeartbeat && uptimeSeconds > GraceSeconds;
        }
    }

}
