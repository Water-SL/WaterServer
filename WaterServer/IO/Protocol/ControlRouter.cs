namespace Water.Host
{
    public static class ControlRouter
    {
        private static readonly IConsoleControl[] Handlers =
        {
            new RoundRestartControl(),
            new EnterIdleControl(),
            new LeaveIdleControl(),
            new ExitRequestControl(),
            new ExitConfirmControl(),
            new CancelExitControl(),
            new HeartbeatControl()
        };

        public static void Dispatch(GameSession session, byte code)
        {
            for (var i = 0; i < Handlers.Length; i++)
            {
                if (Handlers[i].Code == code)
                {
                    Handlers[i].Apply(session);
                    return;
                }
            }
        }
    }
}
