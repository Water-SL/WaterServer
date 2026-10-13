using System;
using System.IO;

namespace Water.Patch
{
    public static class ScriptingAssemblies
    {
        public static void Ensure(string path)
        {
            var json = File.ReadAllText(path);
            if (json.IndexOf("\"WaterSL.dll\"]", StringComparison.Ordinal) >= 0)
                return;

            var marker = "\"],\"types\"";
            var index = json.IndexOf(marker, StringComparison.Ordinal);
            if (index < 0)
                throw new InvalidDataException("ScriptingAssemblies.json has an unexpected shape.");

            json = json.Insert(index + 1, ",\"WaterLib.dll\",\"WaterSL.dll\"");
            var end = json.LastIndexOf("]}", StringComparison.Ordinal);
            if (end < 0)
                throw new InvalidDataException("ScriptingAssemblies.json is missing its type list.");

            json = json.Insert(end, ",16,16");
            File.WriteAllText(path, json);
        }
    }
}
