namespace Water.Host
{
    public interface IConsoleControl
    {
        byte Code { get; }
        void Apply(GameSession session);
    }
}
