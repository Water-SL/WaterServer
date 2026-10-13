using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Water.Host
{
    public static class WaterPaths
    {
        public static string Root
        {
            get
            {
                var home = Environment.GetEnvironmentVariable("WATER_HOME");
                if (!string.IsNullOrWhiteSpace(home))
                    return home;

                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Water");
            }
        }

        public static string ConfigFile
        {
            get { return Path.Combine(Root, "water.json"); }
        }

        public static string PluginDirectory
        {
            get { return Path.Combine(Root, "plugins"); }
        }
    }

    public sealed class WaterSettings
    {
        public string GameDirectory { get; set; } = "";
        public int GamePort { get; set; } = 7777;
        public int WebPort { get; set; } = 7788;
        public string BindAddress { get; set; } = "127.0.0.1";
        public string PasswordHash { get; set; } = "";
        public string AuthKey { get; set; } = "";
        public string Language { get; set; } = "";
        public int RelayHz { get; set; } = 64;
        public bool DecoupleNetworkRate { get; set; } = true;
        public int TxBuffer { get; set; } = 25000;
        public int RxBuffer { get; set; } = 200000;
        public bool RestartOnCrash { get; set; } = true;
        public bool Configured { get; set; }

        [JsonIgnore]
        public bool IsReady
        {
            get { return Configured && !string.IsNullOrEmpty(GameDirectory) && GamePort > 0; }
        }
    }

    public sealed class SettingsStore
    {
        private readonly object _gate = new object();
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public WaterSettings Load()
        {
            lock (_gate)
            {
                if (!File.Exists(WaterPaths.ConfigFile))
                    return new WaterSettings();

                var settings = JsonSerializer.Deserialize<WaterSettings>(File.ReadAllText(WaterPaths.ConfigFile), Json);
                return settings ?? new WaterSettings();
            }
        }

        public void Save(WaterSettings settings)
        {
            lock (_gate)
            {
                Directory.CreateDirectory(WaterPaths.Root);
                Directory.CreateDirectory(WaterPaths.PluginDirectory);
                File.WriteAllText(WaterPaths.ConfigFile, JsonSerializer.Serialize(settings, Json));
            }
        }
    }
}
