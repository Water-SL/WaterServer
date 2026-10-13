namespace Water.Host
{
    public sealed class ExitConfirmControl : IConsoleControl
    {
        public byte Code { get { return 21; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("exit", 14, "Kapanis onaylandi.");
        }
    }
}
