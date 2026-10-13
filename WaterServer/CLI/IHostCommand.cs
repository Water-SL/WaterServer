using System;

namespace Water.Host
{
    public interface IHostCommand
    {
        string Name { get; }
        string Summary { get; }
        void Run(WaterApplication app, string line);
    }
}
