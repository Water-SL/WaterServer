namespace Water
{
    public sealed class LevelFilter : ILogSink
    {
        private readonly LogLevel _minimum;
        private readonly ILogSink _next;

        public LevelFilter(LogLevel minimum, ILogSink next)
        {
            _minimum = minimum;
            _next = next;
        }

        public void Write(LogRecord record)
        {
            if (record.Level >= _minimum && _next != null)
                _next.Write(record);
        }
    }
}
