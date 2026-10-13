using System;

namespace Water
{

    public static class FrameViews
    {
        public static PlayerView[] Players(RelayFrame frame)
        {
            if (frame == null || frame.PlayerCount <= 0)
                return Array.Empty<PlayerView>();

            var list = new PlayerView[frame.PlayerCount];
            for (var i = 0; i < frame.PlayerCount; i++)
            {
                var sample = frame.Players[i];
                list[i] = new PlayerView
                {
                    Id = sample.Id,
                    Name = sample.Name ?? "",
                    X = sample.X,
                    Y = sample.Y,
                    Z = sample.Z,
                    Yaw = sample.Yaw,
                    Held = sample.Held
                };
            }

            return list;
        }
    }

}
