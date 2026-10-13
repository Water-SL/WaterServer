using System;
using System.Collections.Generic;

namespace Water
{
    public sealed class WaterCommandHandler
    {
        public WaterCommandHandler(string name, string summary, Action<string> execute)
        {
            Name = name ?? "";
            Summary = summary ?? "";
            Execute = execute ?? (_ => { });
        }

        public string Name { get; }
        public string Summary { get; }
        public Action<string> Execute { get; }
    }

    public static class CommandBus
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<string, WaterCommandHandler> Handlers = new Dictionary<string, WaterCommandHandler>(StringComparer.OrdinalIgnoreCase);

        public static void Register(string name, string summary, Action<string> execute)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Command name is required.", nameof(name));

            lock (Gate)
                Handlers[name.Trim()] = new WaterCommandHandler(name.Trim(), summary, execute);
        }

        public static bool Remove(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            lock (Gate)
                return Handlers.Remove(name.Trim());
        }

        public static bool TryRun(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;

            var text = line.Trim();
            var space = text.IndexOf(' ');
            var name = space < 0 ? text : text.Substring(0, space);
            WaterCommandHandler? handler;
            lock (Gate)
            {
                if (!Handlers.TryGetValue(name, out handler) || handler == null)
                    return false;
            }

            handler.Execute(text);
            WaterApi.Commands.Publish(text);
            return true;
        }

        public static WaterCommandHandler[] Snapshot()
        {
            lock (Gate)
            {
                var copy = new WaterCommandHandler[Handlers.Count];
                Handlers.Values.CopyTo(copy, 0);
                return copy;
            }
        }
    }
}
