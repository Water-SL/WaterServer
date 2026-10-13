using System;
using System.IO;
using System.Net;

namespace Water.Host
{
    public static class PortRules
    {
        public static void Check(int gamePort, int webPort, string bindAddress, string gameDirectory)
        {
            if (string.IsNullOrWhiteSpace(gameDirectory) || !Directory.Exists(gameDirectory))
                throw new InvalidOperationException("Dedicated server klasoru bulunamadi.");
            if (!File.Exists(Path.Combine(gameDirectory, "SCPSL.exe")) && !File.Exists(Path.Combine(gameDirectory, "SCPSL.x86_64")))
                throw new InvalidOperationException("Bu klasorde SCPSL.exe yok.");
            if (gamePort < 1 || gamePort > 65535 || webPort < 1 || webPort > 65535)
                throw new InvalidOperationException("Port 1 ile 65535 arasinda olmali.");
            if (gamePort == webPort)
                throw new InvalidOperationException("Oyun portu ile panel portu farkli olmali.");
            if (!IPAddress.TryParse(bindAddress, out _))
                throw new InvalidOperationException("Panel adresi bir IP olmali.");
        }
    }
}
