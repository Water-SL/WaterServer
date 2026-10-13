namespace Water.Host
{
    public sealed class CancelExitControl : IConsoleControl
    {
        public byte Code { get { return 22; } }

        public void Apply(GameSession session)
        {
            session.ApplyControl("cancel", 11, "Kapanis iptal.");
        }
    }
}
