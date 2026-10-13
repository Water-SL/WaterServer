using System;
using Water;

namespace Water.Host
{
    public sealed class FrameworksCommand : IHostCommand
    {
        public string Name { get { return "frameworks"; } }
        public string Summary { get { return "Derlenen hedef cerceveler"; } }

        public void Run(WaterApplication app, string line)
        {

            app.Log.Add(11, WaterProduct.SupportedFrameworks);

        }
    }
}
