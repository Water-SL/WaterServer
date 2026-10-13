namespace Water.Host
{
    public static class Locale
    {
        public static bool Turkish { get; private set; }

        public static void Use(string language)
        {
            Turkish = language == "tr";
        }

        public static string Get(string key)
        {
            string en;
            string tr;
            switch (key)
            {
                case "lang.ask":
                    en = "Language:  1 English   2 Turkce";
                    tr = "Dil:  1 English   2 Turkce";
                    break;
                case "folder.ask":
                    en = "Dedicated server folder";
                    tr = "Dedicated server klasoru";
                    break;
                case "port.ask":
                    en = "Port";
                    tr = "Port";
                    break;
                case "starting":
                    en = "Starting WaterSL.";
                    tr = "WaterSL baslatiliyor.";
                    break;
                case "ready":
                    en = "Server is up. Type tps for the real server TPS. help for commands. exit to quit.";
                    tr = "Sunucu acik. Gercek TPS icin tps yaz. Komutlar icin help. Cikis icin exit.";
                    break;
                case "installed":
                    en = "WaterSL is already installed.";
                    tr = "WaterSL zaten kurulu.";
                    break;
                case "missing":
                    en = "WaterSL is not installed yet. Folder is required once.";
                    tr = "WaterSL henuz kurulu degil. Klasor bir kez sorulur.";
                    break;
                case "status":
                    en = "running";
                    tr = "acik";
                    break;
                case "stopped":
                    en = "stopped";
                    tr = "kapali";
                    break;
                case "tps.hint":
                    en = "Real TPS is the game command: tps   (round of 1 / frame time, over the configured tickrate)";
                    tr = "Gercek TPS oyun komutu: tps   (1 / kare suresi, ayarli tickrate uzerinden)";
                    break;
                default:
                    return key;
            }

            return Turkish ? tr : en;
        }
    }
}
