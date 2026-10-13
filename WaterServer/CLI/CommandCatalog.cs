using Water;

namespace Water.Host
{
    public static class CommandCatalog
    {
        public static void Register(WaterApplication app)
        {
            var commands = new IHostCommand[]
            {
                new StatusCommand(),
                new PlayersCommand(),
                new PluginsCommand(),
                new VersionCommand(),
                new FrameworksCommand(),
                new LogsCommand(),
                new PortCommand(),
                new ModulesCommand(),
                new SayCommand(),
                new IdleCommand(),
                new MetricsCommand(),
                new RolesCommand(),
                new ItemsCommand(),
                new ZonesCommand(),
                new TeamsCommand(),
                new HooksCommand(),
                new ConfigCommand(),
                new HelpCommand(),
                new StartCommand(),
                new StopCommand(),
                new RestartCommand()
            };

            for (var i = 0; i < commands.Length; i++)
            {
                var command = commands[i];
                CommandBus.Register(command.Name, command.Summary, line => command.Run(app, line));
            }
        }
    }
}
