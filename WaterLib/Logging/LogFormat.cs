namespace Water
{
    public static class LogFormat
    {
        public static string Line(LogRecord record)
        {
            return record.Time.ToString("HH:mm:ss") + " " + record.Level + " " + record.Message;
        }
    }
}
