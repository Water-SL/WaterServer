namespace Water.Host
{
    public sealed class EnterIdleControl : IConsoleControl
    {
        public byte Code { get { return 17; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("idle-on", 8, "Sunucu idle moda girdi.");
        }
    }
}
