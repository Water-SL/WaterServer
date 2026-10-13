namespace Water.Host
{
    public sealed class HeartbeatControl : IConsoleControl
    {
        public byte Code { get { return 23; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("beat", 0, "");
        }
    }
}
