using System;

namespace Water.Patch
{

    public static class FileSync
    {
        public static void CopyIfNewer(string source, string destination)
        {
            if (!System.IO.File.Exists(destination) || Newer(source, destination))
                System.IO.File.Copy(source, destination, true);
        }

        public static void CopyManaged(string source, string destination)
        {
            foreach (var file in System.IO.Directory.GetFiles(source))
            {
                var name = System.IO.Path.GetFileName(file);
                if (name == "WaterSL.dll" || name == "WaterLib.dll" || name == "water.stamp")
                    continue;
                if (IsDatabaseJunk(name))
                    continue;

                var target = System.IO.Path.Combine(destination, name);
                if (!System.IO.File.Exists(target) || !Same(file, target))
                    System.IO.File.Copy(file, target, true);
            }
        }

        public static void ReplaceOwned(string source, string destination)
        {
            if (System.IO.File.Exists(destination))
            {
                var attributes = System.IO.File.GetAttributes(destination);
                if ((attributes & System.IO.FileAttributes.ReparsePoint) != 0)
                    System.IO.File.Delete(destination);
            }

            if (!System.IO.File.Exists(destination) || System.IO.File.GetLastWriteTimeUtc(source) > System.IO.File.GetLastWriteTimeUtc(destination))
                System.IO.File.Copy(source, destination, true);
        }

        private static bool IsDatabaseJunk(string name)
        {
            var extension = System.IO.Path.GetExtension(name);
            return extension == ".id0" || extension == ".id1" || extension == ".id2" || extension == ".nam" || extension == ".til" || extension == ".idb" || extension == ".i64";
        }

        private static bool Newer(string source, string destination)
        {
            return System.IO.File.GetLastWriteTimeUtc(source) > System.IO.File.GetLastWriteTimeUtc(destination)
                || new System.IO.FileInfo(source).Length != new System.IO.FileInfo(destination).Length;
        }

        private static bool Same(string source, string destination)
        {
            return System.IO.File.GetLastWriteTimeUtc(source) == System.IO.File.GetLastWriteTimeUtc(destination)
                && new System.IO.FileInfo(source).Length == new System.IO.FileInfo(destination).Length;
        }
    }

}
