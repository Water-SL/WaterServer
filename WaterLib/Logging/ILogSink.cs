namespace Water
{
    public interface ILogSink
    {
        void Write(LogRecord record);
    }
}
