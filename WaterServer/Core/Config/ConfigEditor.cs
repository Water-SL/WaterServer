using System;
using Water;

namespace Water.Host
{
    public static class ConfigEditor
    {
        public static void Apply(WaterApplication app, string key, string value)
        {
            var name = (key ?? "").ToLowerInvariant();
            if (name == "relayhz")
            {
                int hz;
                if (!int.TryParse(value, out hz))
                    throw new InvalidOperationException("relayhz sayi olmali.");
                HzLimit.Check(hz);
                app.Settings.RelayHz = hz;
            }
            else if (name == "restartoncrash")
            {
                app.Settings.RestartOnCrash = Truth.IsOn(value);
            }
            else if (name == "decouple")
            {
                app.Settings.DecoupleNetworkRate = Truth.IsOn(value);
            }
            else
            {
                throw new InvalidOperationException("Bilinmeyen ayar. relayhz, restartoncrash, decouple.");
            }

            app.SaveSettings();
            WaterChannels.ConfigChanged.Publish(new ConfigChange { Key = name, Value = value ?? "" });
            app.Log.Add(10, "Kaydedildi. " + name + " = " + value);
        }
    }
}
