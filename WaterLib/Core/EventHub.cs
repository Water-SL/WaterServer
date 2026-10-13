using System;

namespace Water
{
    public sealed class EventHub<T>
    {
        private readonly object _gate = new object();
        private Action<T>? _handlers;

        public void Subscribe(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_gate)
                _handlers += handler;
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (handler == null)
                return;

            lock (_gate)
                _handlers -= handler;
        }

        public void Publish(T value)
        {
            Action<T>? snapshot;
            lock (_gate)
                snapshot = _handlers;

            if (snapshot != null)
                snapshot(value);
        }
    }

    public sealed class PlayerJoinInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public object? Hub { get; set; }
    }

    public sealed class ModuleCatalog
    {
        private readonly object _gate = new object();
        private ModuleInfo[] _items = Array.Empty<ModuleInfo>();

        public void Add(ModuleInfo info)
        {
            lock (_gate)
            {
                var next = new ModuleInfo[_items.Length + 1];
                Array.Copy(_items, next, _items.Length);
                next[_items.Length] = info;
                _items = next;
            }
        }

        public ModuleInfo[] Snapshot()
        {
            lock (_gate)
            {
                var copy = new ModuleInfo[_items.Length];
                Array.Copy(_items, copy, _items.Length);
                return copy;
            }
        }
    }

    public sealed class ModuleInfo
    {
        public ModuleInfo(string name, string role, string framework)
        {
            Name = name;
            Role = role;
            Framework = framework;
        }

        public string Name { get; }
        public string Role { get; }
        public string Framework { get; }
    }

    public static partial class WaterApi
    {
        public static readonly EventHub<PlayerJoinInfo> PlayerJoined = new EventHub<PlayerJoinInfo>();
        public static readonly EventHub<PlayerJoinInfo> PlayerLeft = new EventHub<PlayerJoinInfo>();
        public static readonly EventHub<RoundInfo> RoundStarted = new EventHub<RoundInfo>();
        public static readonly EventHub<RoundInfo> RoundEnded = new EventHub<RoundInfo>();
        public static readonly EventHub<string> Commands = new EventHub<string>();
        public static readonly EventHub<RelayFrame> Frames = new EventHub<RelayFrame>();
        public static readonly EventHub<string> Log = new EventHub<string>();
        public static readonly ModuleCatalog Modules = new ModuleCatalog();

        private static RelayFrame? _latest;

        public static RelayFrame? Latest
        {
            get { return _latest; }
        }

        public static void PublishFrame(RelayFrame frame)
        {
            _latest = frame;
            Frames.Publish(frame);
        }

        public static void WriteLog(string message)
        {
            WaterLog.Write(LogLevel.Info, message);
            Log.Publish(message);
        }

        public static void Contribute(string name, string role)
        {
            Modules.Add(new ModuleInfo(name, role, WaterRuntime.Framework));
        }
    }
}
