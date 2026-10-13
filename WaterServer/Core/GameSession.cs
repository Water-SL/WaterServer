using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Water;
using Water.Patch;

namespace Water.Host
{
    public sealed class GameSession : IDisposable
    {
        private readonly WaterSettings _settings;
        private readonly LogBuffer _log;
        private readonly object _gate = new object();
        private ConsoleLink? _console;
        private Process? _process;
        private string _token = "";
        private bool _stopRequested;
        private bool _idle;
        private readonly RestartBudget _restarts = new RestartBudget(4, TimeSpan.FromMinutes(8));
        private long _heartbeat = Stopwatch.GetTimestamp();
        private bool _sawHeartbeat;
        private Timer? _watch;

        public GameSession(WaterSettings settings, LogBuffer log)
        {
            _settings = settings;
            _log = log;
        }

        public bool Running
        {
            get { return _process != null && !_process.HasExited; }
        }

        public int Pid
        {
            get { return Running ? _process!.Id : 0; }
        }

        public bool Idle
        {
            get { return _idle; }
        }

        public string Executable { get; private set; } = "";

        public void Start()
        {
            lock (_gate)
            {
                if (Running)
                    throw new InvalidOperationException("Sunucu zaten çalışıyor.");

                _stopRequested = false;
                var bits = GameBits.Locate();
                var deployed = GameDeploy.Deploy(new DeployRequest
                {
                    GameDirectory = _settings.GameDirectory,
                    WaterSlDll = bits.WaterSl,
                    WaterLibDll = bits.WaterLib
                });

                Executable = deployed.Executable;
                if (deployed.UpToDate)
                    _log.Add(11, "WaterSL güncel, yama yeniden yazılmadı.");
                else
                {
                    _log.Add(10, "WaterSL hazır. Yama: " + string.Join(", ", deployed.Report.Applied));
                    if (deployed.Report.Skipped.Count > 0)
                        _log.Add(14, "Atlanan kancalar: " + string.Join(", ", deployed.Report.Skipped));
                }

                _console?.Dispose();
                _console = new ConsoleLink(_log);
                _console.Control += OnControl;
                _console.Start();

                _token = PasswordHasher.CreateToken();
                _sawHeartbeat = false;
                _heartbeat = Stopwatch.GetTimestamp();

                var start = LaunchPlan.Create(_settings, deployed.Executable, _console.Port, _token);
                _process = Process.Start(start) ?? throw new InvalidOperationException("WaterSL başlamadı.");
                _process.EnableRaisingEvents = true;
                _process.Exited += OnExited;
                OutputPump.Attach(_process, _log);
                _log.Add(10, "WaterSL başladı. PID " + _process.Id + ", oyun portu " + _settings.GamePort + ".");
                _watch = new Timer(WatchHeartbeat, null, 5000, 5000);
            }
        }

        public void Stop()
        {
            lock (_gate)
            {
                _stopRequested = true;
                SendOrKill("exit");
            }
        }

        public void Restart()
        {
            Stop();
            for (var i = 0; i < 40 && Running; i++)
                Thread.Sleep(100);
            Start();
        }

        public void Command(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var console = _console;
            if (console == null || !console.Connected)
                throw new InvalidOperationException("Oyun konsolu hazır değil.");

            console.Send(text.Trim());
        }

        public void Dispose()
        {
            _stopRequested = true;
            _watch?.Dispose();
            try
            {
                if (_process != null && !_process.HasExited)
                    _process.Kill();
            }
            catch { }

            _console?.Dispose();
            _process?.Dispose();
        }

        private void SendOrKill(string command)
        {
            StopSequence.Request(
                () =>
                {
                    if (_console == null || !_console.Connected)
                        throw new InvalidOperationException("Konsol yok.");
                    _console.Send(command);
                },
                () =>
                {
                    if (_process != null && !_process.HasExited)
                        _process.Kill();
                });
        }

        public void ApplyControl(string kind, int color, string message)
        {
            if (kind == "idle-on")
                _idle = true;
            else if (kind == "idle-off")
                _idle = false;
            else if (kind == "exit")
                _stopRequested = true;
            else if (kind == "cancel")
                _stopRequested = false;
            else if (kind == "beat")
            {
                _sawHeartbeat = true;
                _heartbeat = Stopwatch.GetTimestamp();
                WaterChannels.Heartbeats.Publish(new HeartbeatInfo());
            }

            if (!string.IsNullOrEmpty(message))
                _log.Add(color, message);
        }

        private void OnControl(byte code)
        {
            ControlRouter.Dispatch(this, code);
        }

        private void OnExited(object? sender, EventArgs args)
        {
            _watch?.Dispose();
            _watch = null;
            var code = _process?.ExitCode ?? -1;
            _log.Add(14, "WaterSL kapandı. Çıkış kodu " + code + ".");

            if (_stopRequested || !_settings.RestartOnCrash)
                return;

            if (!_restarts.TryTake())
            {
                _log.Add(12, "Sekiz dakika içinde dört yeniden başlatma aşıldı. Otomatik dönüş durdu.");
                return;
            }

            _log.Add(14, "Çökme sonrası yeniden başlatılıyor.");
            try { Start(); }
            catch (Exception ex) { _log.Add(12, ex.Message); }
        }

        private void WatchHeartbeat(object? state)
        {
            if (!Running)
                return;

            var silent = MonoTime.SecondsSince(_heartbeat);
            var uptime = _process == null ? 0 : (DateTime.UtcNow - _process.StartTime.ToUniversalTime()).TotalSeconds;
            if (HeartbeatRule.Waiting(_sawHeartbeat, uptime))
                _log.Add(14, "Henüz heartbeat yok. Sessiz çökme izlemesi beklemede.");

            if (HeartbeatRule.ShouldRestart(_sawHeartbeat, silent, uptime))
            {
                _log.Add(12, "Heartbeat kesildi. Sunucu yeniden başlatılıyor.");
                _stopRequested = false;
                try
                {
                    if (_process != null && !_process.HasExited)
                        _process.Kill();
                }
                catch
                {
                }
            }
        }
    }

    public static class GameBits
    {
        public static (string WaterSl, string WaterLib) Locate()
        {
            return GameBitsLocator.Locate();
        }
    }
}
