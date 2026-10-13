namespace Water.Host
{
    public sealed class PortCommand : IHostCommand
    {
        public string Name { get { return "port"; } }
        public string Summary { get { return "Game port"; } }

        public void Run(WaterApplication app, string line)
        {
            app.Log.Add(11, "game " + app.Settings.GamePort);
        }
    }
}
