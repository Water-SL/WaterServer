namespace Water.Host
{
    public sealed class StartCommand : IHostCommand
    {
        public string Name { get { return "start"; } }
        public string Summary { get { return "WaterSL sürecini aç"; } }
        public void Run(WaterApplication app, string line) { app.Game.Start(); }
    }

    public sealed class StopCommand : IHostCommand
    {
        public string Name { get { return "stop"; } }
        public string Summary { get { return "Oyun sürecini kapat"; } }
        public void Run(WaterApplication app, string line) { app.Game.Stop(); }
    }

    public sealed class RestartCommand : IHostCommand
    {
        public string Name { get { return "restart"; } }
        public string Summary { get { return "Oyun sürecini yeniden başlat"; } }
        public void Run(WaterApplication app, string line) { app.Game.Restart(); }
    }
}
