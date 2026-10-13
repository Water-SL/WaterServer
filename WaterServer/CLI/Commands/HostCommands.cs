namespace Water.Host
{
    public static class HostCommands
    {
        public static void Register(WaterApplication app)
        {
            CommandCatalog.Register(app);
        }
    }
}
