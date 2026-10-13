using System;
using System.IO;
using System.Reflection;
using Water;

namespace Water.SL
{
    internal static class PluginHost
    {
        public static void Load(Action<string> log)
        {
            var directory = Environment.GetEnvironmentVariable("WATER_PLUGINS");
            if (string.IsNullOrEmpty(directory))
                directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Water", "plugins");

            var files = PluginScan.Dlls(directory);
            for (var i = 0; i < files.Length; i++)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(files[i]);
                    var types = SafeTypes.Of(assembly);
                    for (var t = 0; t < types.Length; t++)
                    {
                        var type = types[t];
                        if (type == null || type.IsAbstract || type.IsInterface || !typeof(IWaterPlugin).IsAssignableFrom(type))
                            continue;

                        var plugin = (IWaterPlugin)Activator.CreateInstance(type);
                        plugin.Enable();
                        if (log != null)
                            log("Water plugin: " + plugin.Name);
                        WaterApi.Contribute(plugin.Name, "Oyun eklentisi");
                    }
                }
                catch (Exception ex)
                {
                    if (log != null)
                        log("Water plugin failed: " + Path.GetFileName(files[i]) + " (" + ex.GetType().Name + ")");
                }
            }
        }

    }
}
