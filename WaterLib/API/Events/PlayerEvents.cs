using System;

namespace Water
{
    public static class PlayerEvents
    {
        public static event Action<PlayerJoinInfo>? Joined;

        public static event Action<PlayerJoinInfo>? Left;

        public static void RaiseJoined(PlayerJoinInfo player)
        {
            if (player == null)
                return;

            WaterPlayers.Add(player);
            WaterApi.PlayerJoined.Publish(player);
            var handler = Joined;
            if (handler != null)
                handler(player);
        }

        public static void RaiseLeft(PlayerJoinInfo player)
        {
            if (player == null)
                return;

            WaterPlayers.Remove(player.Id);
            WaterApi.PlayerLeft.Publish(player);
            var handler = Left;
            if (handler != null)
                handler(player);
        }
    }
}
