using System;
using System.Collections.Generic;

namespace Water
{
    public static class WaterPlayers
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<int, PlayerJoinInfo> Online = new Dictionary<int, PlayerJoinInfo>();

        public static void Add(PlayerJoinInfo player)
        {
            if (player == null)
                return;

            lock (Gate)
                Online[player.Id] = player;
        }

        public static void Remove(int id)
        {
            lock (Gate)
                Online.Remove(id);
        }

        public static bool TryGet(int id, out PlayerJoinInfo player)
        {
            lock (Gate)
            {
                PlayerJoinInfo? found;
                if (!Online.TryGetValue(id, out found))
                {
                    player = null!;
                    return false;
                }

                player = found;
                return true;
            }
        }

        public static PlayerJoinInfo[] Snapshot()
        {
            lock (Gate)
            {
                var copy = new PlayerJoinInfo[Online.Count];
                Online.Values.CopyTo(copy, 0);
                return copy;
            }
        }
    }
}
