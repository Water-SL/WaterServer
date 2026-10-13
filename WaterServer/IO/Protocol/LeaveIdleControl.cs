namespace Water.Host
{
    public sealed class LeaveIdleControl : IConsoleControl
    {
        public byte Code { get { return 18; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("idle-off", 8, "Sunucu idle moddan cikti.");
        }
    }
}
