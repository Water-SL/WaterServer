using System;
using System.IO;

namespace Water
{
    public sealed class FileSink : ILogSink
    {
        private readonly string _path;
        private readonly object _gate = new object();

        public FileSink(string path)
        {
            _path = path;
        }

        public void Write(LogRecord record)
        {
            if (string.IsNullOrEmpty(_path))
                return;

            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            lock (_gate)
                File.AppendAllText(_path, LogFormat.Line(record) + Environment.NewLine);
        }
    }
}
