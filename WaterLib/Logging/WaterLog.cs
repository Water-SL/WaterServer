using System;

namespace Water
{
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4
    }

    public readonly struct LogRecord
    {
        public LogRecord(DateTime time, LogLevel level, string message)
        {
            Time = time;
            Level = level;
            Message = message ?? "";
        }

        public DateTime Time { get; }
        public LogLevel Level { get; }
        public string Message { get; }
    }

    public static class WaterLog
    {
        public const int Capacity = 512;

        private static readonly object Gate = new object();
        private static readonly LogRecord[] Ring = new LogRecord[Capacity];
        private static int _count;
        private static int _next;
        private static LogLevel _minimum = LogLevel.Info;
        private static readonly FileSink? _file = OpenFile();

        public static event Action<LogRecord>? Written;

        public static LogLevel Minimum
        {
            get { return _minimum; }
            set { _minimum = value; }
        }

        public static void Write(LogLevel level, string message)
        {
            if (level < _minimum || string.IsNullOrEmpty(message))
                return;

            var record = new LogRecord(DateTime.UtcNow, level, message);
            Action<LogRecord>? handlers;
            lock (Gate)
            {
                Ring[_next] = record;
                _next = (_next + 1) % Capacity;
                if (_count < Capacity)
                    _count++;
                handlers = Written;
            }

            if (handlers != null)
                handlers(record);
            if (_file != null)
                _file.Write(record);
        }

        private static FileSink? OpenFile()
        {
            var path = LogPath.Resolve();
            return string.IsNullOrEmpty(path) ? null : new FileSink(path);
        }

        public static LogRecord[] Recent()
        {
            lock (Gate)
            {
                var copy = new LogRecord[_count];
                var start = _count == Capacity ? _next : 0;
                for (var i = 0; i < _count; i++)
                    copy[i] = Ring[(start + i) % Capacity];
                return copy;
            }
        }
    }
}
