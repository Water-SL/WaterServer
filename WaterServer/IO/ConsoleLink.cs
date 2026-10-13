using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Water.Host
{
    public readonly struct LogLine
    {
        public LogLine(DateTime time, int color, string text)
        {
            Time = time;
            Color = color;
            Text = text;
        }

        public DateTime Time { get; }
        public int Color { get; }
        public string Text { get; }
    }

    public sealed class LogBuffer
    {
        private readonly object _gate = new object();
        private readonly LogLine[] _lines = new LogLine[500];
        private int _next;
        private int _count;

        public event Action<LogLine>? Written;

        public void Add(int color, string text)
        {
            if (text == null)
                text = "";

            var line = new LogLine(DateTime.Now, color, text.Replace("\r", ""));
            lock (_gate)
            {
                _lines[_next] = line;
                _next = (_next + 1) % _lines.Length;
                if (_count < _lines.Length)
                    _count++;
            }

            var handler = Written;
            if (handler != null)
                handler(line);
        }

        public LogLine[] Recent()
        {
            lock (_gate)
            {
                var copy = new LogLine[_count];
                var start = (_next - _count + _lines.Length) % _lines.Length;
                for (var i = 0; i < _count; i++)
                    copy[i] = _lines[(start + i) % _lines.Length];
                return copy;
            }
        }
    }

    public sealed class ConsoleLink : IDisposable
    {
        private readonly LogBuffer _log;
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cancel = new CancellationTokenSource();
        private NetworkStream? _stream;
        private int _port;

        public ConsoleLink(LogBuffer log)
        {
            _log = log;
            _listener = new TcpListener(IPAddress.Loopback, 0);
        }

        public int Port
        {
            get { return _port; }
        }

        public bool Connected
        {
            get { return _stream != null; }
        }

        public event Action<byte>? Control;

        public void Start()
        {
            _listener.Start();
            _port = ((IPEndPoint)_listener.LocalEndpoint).Port;
            Task.Run(AcceptLoop);
        }

        public void Send(string command)
        {
            var stream = _stream;
            if (stream == null)
                throw new InvalidOperationException("Oyun konsolu henüz bağlanmadı.");

            var payload = Encoding.UTF8.GetBytes(command);
            var buffer = new byte[4 + payload.Length];
            LengthPrefix.Write(buffer, payload.Length);
            Buffer.BlockCopy(payload, 0, buffer, 4, payload.Length);
            lock (stream)
                stream.Write(buffer, 0, buffer.Length);
        }

        public void Dispose()
        {
            _cancel.Cancel();
            try { _listener.Stop(); } catch { }
            try { _stream?.Dispose(); } catch { }
        }

        private async Task AcceptLoop()
        {
            try
            {
                while (!_cancel.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    client.NoDelay = true;
                    _stream = client.GetStream();
                    _log.Add(11, "Oyun konsolu bağlandı.");
                    await ReadLoop(_stream);
                    _log.Add(14, "Oyun konsolu kapandı.");
                    _stream = null;
                }
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException)
            {
            }
        }

        private async Task ReadLoop(NetworkStream stream)
        {
            var code = new byte[1];
            var length = new byte[4];
            while (!_cancel.IsCancellationRequested)
            {
                if (!await ReadExact(stream, code, 1))
                    return;

                if (code[0] < 16)
                {
                    if (!await ReadExact(stream, length, 4))
                        return;

                    var size = LengthPrefix.Read(length);
                    if (!ConsoleLimits.PayloadOk(size))
                    {
                        _log.Add(12, "Konsol paketi bozuk.");
                        return;
                    }

                    var payload = new byte[size];
                    if (size > 0 && !await ReadExact(stream, payload, size))
                        return;

                    _log.Add(code[0], Encoding.UTF8.GetString(payload));
                }
                else
                {
                    var control = Control;
                    if (control != null)
                        control(code[0]);
                }
            }
        }

        private static async Task<bool> ReadExact(NetworkStream stream, byte[] buffer, int count)
        {
            var offset = 0;
            while (offset < count)
            {
                var read = await stream.ReadAsync(buffer, offset, count - offset);
                if (read == 0)
                    return false;
                offset += read;
            }

            return true;
        }
    }
}
