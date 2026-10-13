using Water;

namespace Water.Patch
{

    public static class StampBuilder
    {
        public static string Build(string assemblyPath, string waterSl, string waterLib)
        {
            var builder = new System.Text.StringBuilder();
            builder.Append(Water.WaterProduct.Version);
            builder.Append('|');
            builder.Append(System.IO.File.GetLastWriteTimeUtc(assemblyPath).Ticks);
            builder.Append('|');
            builder.Append(new System.IO.FileInfo(assemblyPath).Length);
            builder.Append('|');
            builder.Append(System.IO.File.GetLastWriteTimeUtc(waterSl).Ticks);
            builder.Append('|');
            builder.Append(System.IO.File.GetLastWriteTimeUtc(waterLib).Ticks);
            builder.Append("|livehooks1");
            return builder.ToString();
        }
    }

}
