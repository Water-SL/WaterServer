namespace Water.Host
{
    public sealed class RoundRestartControl : IConsoleControl
    {
        public byte Code { get { return 16; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("round", 11, "Round yeniden basliyor.");
        }
    }
}
