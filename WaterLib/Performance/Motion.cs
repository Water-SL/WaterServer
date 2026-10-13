using System;

namespace Water
{
    public static class Motion
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + ((b - a) * t);
        }

        public static float Repeat(float value, float length)
        {
            if (length <= 0f)
                return 0f;

            return value - ((float)Math.Floor(value / length) * length);
        }

        public static float LerpAngle(float a, float b, float t)
        {
            var delta = Repeat(b - a, 360f);
            if (delta > 180f)
                delta -= 360f;

            return a + (delta * t);
        }
    }

    public sealed class RelayFrame
    {
        public long Tick;
        public long Timestamp;
        public float Alpha;
        public bool Extrapolating;
        public PlayerSample[] Players = new PlayerSample[32];
        public int PlayerCount;
        public ToySample[] Toys = new ToySample[16];
        public int ToyCount;
        public ItemSample[] Items = new ItemSample[32];
        public int ItemCount;

        public void Clear()
        {
            Tick = 0;
            Timestamp = 0;
            Alpha = 0f;
            Extrapolating = false;
            PlayerCount = 0;
            ToyCount = 0;
            ItemCount = 0;
        }

        public RelayFrame Clone()
        {
            var copy = new RelayFrame
            {
                Tick = Tick,
                Timestamp = Timestamp,
                Alpha = Alpha,
                Extrapolating = Extrapolating,
                PlayerCount = PlayerCount,
                ToyCount = ToyCount,
                ItemCount = ItemCount,
                Players = new PlayerSample[Math.Max(PlayerCount, 1)],
                Toys = new ToySample[Math.Max(ToyCount, 1)],
                Items = new ItemSample[Math.Max(ItemCount, 1)]
            };

            if (PlayerCount > 0)
                Array.Copy(Players, copy.Players, PlayerCount);
            if (ToyCount > 0)
                Array.Copy(Toys, copy.Toys, ToyCount);
            if (ItemCount > 0)
                Array.Copy(Items, copy.Items, ItemCount);

            return copy;
        }
    }

    public static class RelayComposer
    {
        public const float MaxExtrapolation = 0.35f;

        public static void Compose(SnapshotView previous, SnapshotView latest, long nowTicks, RelayFrame output)
        {
            output.Clear();
            output.Tick = latest.Tick;
            output.Timestamp = nowTicks;

            var alpha = 1f;
            var extrapolating = false;
            var denom = latest.Timestamp - previous.Timestamp;

            if (denom > 0)
            {
                var span = (double)(nowTicks - previous.Timestamp) / denom;
                if (span > 1d)
                {
                    extrapolating = true;
                    alpha = (float)span;
                    if (alpha > 1f + MaxExtrapolation)
                        alpha = 1f + MaxExtrapolation;
                }
                else if (span < 0d)
                {
                    alpha = 0f;
                }
                else
                {
                    alpha = (float)span;
                }
            }

            output.Alpha = alpha;
            output.Extrapolating = extrapolating;
            ComposePlayers(previous, latest, alpha, output);
            ComposeToys(previous, latest, alpha, output);
            ComposeItems(previous, latest, alpha, output);
        }

        private static void ComposePlayers(SnapshotView previous, SnapshotView latest, float alpha, RelayFrame output)
        {
            for (var i = 0; i < latest.PlayerCount; i++)
            {
                var current = latest.Players[i];
                var priorIndex = FindPlayer(previous, current.Id);
                if (priorIndex >= 0 && alpha < 1f || priorIndex >= 0 && alpha > 1f)
                {
                    var prior = previous.Players[priorIndex];
                    current.X = Motion.Lerp(prior.X, current.X, alpha);
                    current.Y = Motion.Lerp(prior.Y, current.Y, alpha);
                    current.Z = Motion.Lerp(prior.Z, current.Z, alpha);
                    current.Yaw = Motion.LerpAngle(prior.Yaw, current.Yaw, alpha);
                    current.Pitch = Motion.LerpAngle(prior.Pitch, current.Pitch, alpha);
                }

                AddPlayer(output, current);
            }
        }

        private static void ComposeToys(SnapshotView previous, SnapshotView latest, float alpha, RelayFrame output)
        {
            for (var i = 0; i < latest.ToyCount; i++)
            {
                var current = latest.Toys[i];
                var priorIndex = FindToy(previous, current.Id);
                if (priorIndex >= 0)
                {
                    var prior = previous.Toys[priorIndex];
                    current.X = Motion.Lerp(prior.X, current.X, alpha);
                    current.Y = Motion.Lerp(prior.Y, current.Y, alpha);
                    current.Z = Motion.Lerp(prior.Z, current.Z, alpha);
                    current.Yaw = Motion.LerpAngle(prior.Yaw, current.Yaw, alpha);
                }

                AddToy(output, current);
            }
        }

        private static void ComposeItems(SnapshotView previous, SnapshotView latest, float alpha, RelayFrame output)
        {
            for (var i = 0; i < latest.ItemCount; i++)
            {
                var current = latest.Items[i];
                var priorIndex = FindItem(previous, current.Id);
                if (priorIndex >= 0)
                {
                    var prior = previous.Items[priorIndex];
                    current.X = Motion.Lerp(prior.X, current.X, alpha);
                    current.Y = Motion.Lerp(prior.Y, current.Y, alpha);
                    current.Z = Motion.Lerp(prior.Z, current.Z, alpha);
                }

                AddItem(output, current);
            }
        }

        private static int FindPlayer(SnapshotView view, int id)
        {
            for (var i = 0; i < view.PlayerCount; i++)
            {
                if (view.Players[i].Id == id)
                    return i;
            }

            return -1;
        }

        private static int FindToy(SnapshotView view, int id)
        {
            for (var i = 0; i < view.ToyCount; i++)
            {
                if (view.Toys[i].Id == id)
                    return i;
            }

            return -1;
        }

        private static int FindItem(SnapshotView view, int id)
        {
            for (var i = 0; i < view.ItemCount; i++)
            {
                if (view.Items[i].Id == id)
                    return i;
            }

            return -1;
        }

        private static void AddPlayer(RelayFrame frame, PlayerSample sample)
        {
            if (frame.PlayerCount == frame.Players.Length)
                Array.Resize(ref frame.Players, frame.Players.Length * 2);

            frame.Players[frame.PlayerCount++] = sample;
        }

        private static void AddToy(RelayFrame frame, ToySample sample)
        {
            if (frame.ToyCount == frame.Toys.Length)
                Array.Resize(ref frame.Toys, frame.Toys.Length * 2);

            frame.Toys[frame.ToyCount++] = sample;
        }

        private static void AddItem(RelayFrame frame, ItemSample sample)
        {
            if (frame.ItemCount == frame.Items.Length)
                Array.Resize(ref frame.Items, frame.Items.Length * 2);

            frame.Items[frame.ItemCount++] = sample;
        }
    }
}
