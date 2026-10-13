namespace Water.Host
{
    public sealed class PlayersCommand : IHostCommand
    {
        public string Name { get { return "players"; } }
        public string Summary { get { return "Where the player list lives"; } }

        public void Run(WaterApplication app, string line)
        {
            app.Log.Add(11, "The player list is the game console. Lines typed here are sent to the game.");
        }
    }
}
