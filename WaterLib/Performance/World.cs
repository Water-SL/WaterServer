using System;

namespace Water
{
    public struct PlayerSample
    {
        public int Id;
        public float X;
        public float Y;
        public float Z;
        public float Yaw;
        public float Pitch;
        public int Held;
        public string Name;
    }

    public struct ToySample
    {
        public int Id;
        public float X;
        public float Y;
        public float Z;
        public float Yaw;
    }

    public struct ItemSample
    {
        public int Id;
        public int Type;
        public float X;
        public float Y;
        public float Z;
        public int Owner;
    }

    public sealed class WorldSnapshot
    {
        public long Tick;
        public long Timestamp;
        public long TickDuration;
        public PlayerSample[] Players = new PlayerSample[32];
        public int PlayerCount;
        public ToySample[] Toys = new ToySample[16];
        public int ToyCount;
        public ItemSample[] Items = new ItemSample[32];
        public int ItemCount;

        public void ResetCounts()
        {
            PlayerCount = 0;
            ToyCount = 0;
            ItemCount = 0;
        }

        public void AddPlayer(PlayerSample sample)
        {
            Ensure(ref Players, PlayerCount + 1);
            Players[PlayerCount++] = sample;
        }

        public void AddToy(ToySample sample)
        {
            Ensure(ref Toys, ToyCount + 1);
            Toys[ToyCount++] = sample;
        }

        public void AddItem(ItemSample sample)
        {
            Ensure(ref Items, ItemCount + 1);
            Items[ItemCount++] = sample;
        }

        private static void Ensure<T>(ref T[] array, int needed)
        {
            ArrayGrow.Ensure(ref array, needed);
        }
    }

    public struct SnapshotView
    {
        public long Tick;
        public long Timestamp;
        public PlayerSample[] Players;
        public int PlayerCount;
        public ToySample[] Toys;
        public int ToyCount;
        public ItemSample[] Items;
        public int ItemCount;
    }

    public sealed class SnapshotBus
    {
        private readonly WorldSnapshot[] _slots =
        {
            new WorldSnapshot(),
            new WorldSnapshot(),
            new WorldSnapshot()
        };

        private int _published = -1;
        private int _reading = -1;

        public WorldSnapshot BeginWrite()
        {
            var published = System.Threading.Volatile.Read(ref _published);
            var reading = System.Threading.Volatile.Read(ref _reading);
            return _slots[SlotPicker.Pick(published, reading, 3)];
        }

        public void Publish(WorldSnapshot snapshot)
        {
            for (var i = 0; i < 3; i++)
            {
                if (ReferenceEquals(_slots[i], snapshot))
                {
                    System.Threading.Volatile.Write(ref _published, i);
                    return;
                }
            }
        }

        public WorldSnapshot? Read()
        {
            var index = System.Threading.Volatile.Read(ref _published);
            if (index < 0)
                return null;

            System.Threading.Volatile.Write(ref _reading, index);
            return _slots[index];
        }

        public void EndRead()
        {
            System.Threading.Volatile.Write(ref _reading, -1);
        }
    }

    public static class SnapshotCopy
    {
        public static void Copy(WorldSnapshot source, WorldSnapshot destination)
        {
            destination.Tick = source.Tick;
            destination.Timestamp = source.Timestamp;
            destination.TickDuration = source.TickDuration;
            destination.PlayerCount = source.PlayerCount;
            destination.ToyCount = source.ToyCount;
            destination.ItemCount = source.ItemCount;
            Grow(ref destination.Players, source.PlayerCount);
            Grow(ref destination.Toys, source.ToyCount);
            Grow(ref destination.Items, source.ItemCount);

            Array.Copy(source.Players, destination.Players, source.PlayerCount);
            Array.Copy(source.Toys, destination.Toys, source.ToyCount);
            Array.Copy(source.Items, destination.Items, source.ItemCount);
        }

        public static SnapshotView View(WorldSnapshot snapshot)
        {
            return new SnapshotView
            {
                Tick = snapshot.Tick,
                Timestamp = snapshot.Timestamp,
                Players = snapshot.Players,
                PlayerCount = snapshot.PlayerCount,
                Toys = snapshot.Toys,
                ToyCount = snapshot.ToyCount,
                Items = snapshot.Items,
                ItemCount = snapshot.ItemCount
            };
        }

        private static void Grow<T>(ref T[] array, int needed)
        {
            if (needed > array.Length)
                Array.Resize(ref array, Math.Max(needed, array.Length * 2));
        }
    }
}
