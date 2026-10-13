using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Water;

namespace Water.Host
{
    public sealed class WaterApplication : IDisposable
    {
        private readonly SettingsStore _store;
        private readonly List<IWaterPlugin> _plugins = new List<IWaterPlugin>();

        public WaterApplication(WaterSettings settings, SettingsStore store)
        {
            Settings = settings;
            _store = store;
            Log = new LogBuffer();
            Game = new GameSession(Settings, Log);
            WaterApi.Contribute("WaterLib", "Shared API");
            WaterApi.Contribute("WaterServer", "Process and console");
            LoadPlugins();
            HostCommands.Register(this);
        }

        public WaterSettings Settings { get; private set; }
        public LogBuffer Log { get; }
        public GameSession Game { get; }

        public void SaveSettings()
        {
            _store.Save(Settings);
        }

        public void Dispatch(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var line = text.Trim();
            Log.Add(13, "› " + line);
            if (CommandBus.TryRun(line))
                return;

            Game.Command(line);
        }

        public void Dispose()
        {
            for (var i = 0; i < _plugins.Count; i++)
            {
                try { _plugins[i].Disable(); } catch { }
            }

            Game.Dispose();
        }

        private void LoadPlugins()
        {
            if (!Directory.Exists(WaterPaths.PluginDirectory))
                return;

            var files = Directory.GetFiles(WaterPaths.PluginDirectory, "*.dll");
            for (var i = 0; i < files.Length; i++)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(files[i]);
                    Type[] types;
                    try { types = assembly.GetTypes(); }
                    catch (ReflectionTypeLoadException ex) { types = (ex.Types?.Where(t2 => t2 != null).Select(t2 => t2!) ?? Enumerable.Empty<Type>()).ToArray(); }

                    for (var t = 0; t < types.Length; t++)
                    {
                        var type = types[t];
                        if (type == null || type.IsAbstract || !typeof(IWaterPlugin).IsAssignableFrom(type))
                            continue;

                        var plugin = (IWaterPlugin)Activator.CreateInstance(type)!;
                        plugin.Enable();
                        _plugins.Add(plugin);
                        Log.Add(10, "Plugin: " + plugin.Name);
                    }
                }
                catch (Exception ex)
                {
                    Log.Add(14, "Plugin failed: " + Path.GetFileName(files[i]) + " (" + ex.GetType().Name + ")");
                }
            }
        }

        public static string SuggestedGame()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "SCP Secret Laboratory Dedicated Server");
            return Directory.Exists(path) ? path : "";
        }
    }
}
