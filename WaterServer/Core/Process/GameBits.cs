using System;
using System.IO;

namespace Water.Host
{
    public static class GameBitsLocator
    {
        public static (string WaterSl, string WaterLib) Locate()
        {
            var beside = Path.Combine(AppContext.BaseDirectory, "game");
            var sl = Path.Combine(beside, "WaterSL.dll");
            var lib = Path.Combine(beside, "WaterLib.dll");
            if (File.Exists(sl) && File.Exists(lib))
                return (sl, lib);

            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            for (var i = 0; i < 7 && dir != null; i++, dir = dir.Parent)
            {
                foreach (var configuration in new[] { "Release", "Debug" })
                {
                    sl = Path.Combine(dir.FullName, "WaterSL", "bin", configuration, "net472", "WaterSL.dll");
                    lib = Path.Combine(dir.FullName, "WaterLib", "bin", configuration, "net472", "WaterLib.dll");
                    if (File.Exists(sl) && File.Exists(lib))
                        return (sl, lib);
                }
            }

            throw new FileNotFoundException("net472 WaterSL.dll bulunamadi. Once cozumu derleyin.");
        }
    }
}
