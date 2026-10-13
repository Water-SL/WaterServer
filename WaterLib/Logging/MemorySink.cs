namespace Water
{
    public sealed class MemorySink : ILogSink
    {
        private readonly LogRecord[] _records;
        private int _count;
        private int _next;

        public MemorySink(int capacity)
        {
            _records = new LogRecord[capacity < 8 ? 8 : capacity];
        }

        public int Count { get { return _count; } }

        public void Write(LogRecord record)
        {
            _records[_next] = record;
            _next = (_next + 1) % _records.Length;
            if (_count < _records.Length)
                _count++;
        }
    }
}
