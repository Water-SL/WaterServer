using System;
using Water;

namespace Water.Host
{
    public sealed class ModulesCommand : IHostCommand
    {
        public string Name { get { return "modules"; } }
        public string Summary { get { return "Yuklu moduller"; } }

        public void Run(WaterApplication app, string line)
        {

            new PluginsCommand().Run(app, line);

        }
    }
}
