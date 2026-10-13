namespace Water.Host
{
    public sealed class ExitRequestControl : IConsoleControl
    {
        public byte Code { get { return 20; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("exit", 14, "Kapanis istendi.");
        }
    }
}
