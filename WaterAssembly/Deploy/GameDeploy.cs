using System;
using System.IO;
using Water;

namespace Water.Patch
{
    public sealed class DeployRequest
    {
        public string GameDirectory { get; set; } = "";
        public string WaterSlDll { get; set; } = "";
        public string WaterLibDll { get; set; } = "";
    }

    public sealed class DeployResult
    {
        public string Executable { get; set; } = "";
        public string DataDirectory { get; set; } = "";
        public PatchReport Report { get; set; } = new PatchReport();
        public bool UpToDate { get; set; }
    }

    public static class GameDeploy
    {
        public static DeployResult Deploy(DeployRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (!Directory.Exists(request.GameDirectory))
                throw new DirectoryNotFoundException("Game directory was not found.");
            if (!File.Exists(request.WaterSlDll) || !File.Exists(request.WaterLibDll))
                throw new FileNotFoundException("WaterSL.dll and WaterLib.dll (net472) must be built before deployment.");

            var sourceExe = ExecutableResolver.Resolve(request.GameDirectory);
            var sourceData = Path.Combine(request.GameDirectory, "SCPSL_Data");
            if (!Directory.Exists(sourceData))
                throw new DirectoryNotFoundException("SCPSL_Data was not found next to the dedicated server.");

            var result = new DeployResult();
            var targetExe = Path.Combine(request.GameDirectory, sourceExe.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? "WaterSL.exe" : "WaterSL.x86_64");
            var targetData = Path.Combine(request.GameDirectory, "WaterSL_Data");
            Directory.CreateDirectory(targetData);

            FileSync.CopyIfNewer(sourceExe, targetExe);
            DirectoryLink.LinkTree(sourceData, targetData);

            var managed = Path.Combine(targetData, "Managed");
            Directory.CreateDirectory(managed);
            var sourceManaged = Path.Combine(sourceData, "Managed");
            var sourceAssembly = Path.Combine(sourceManaged, "Assembly-CSharp.dll");
            var targetAssembly = Path.Combine(managed, "Assembly-CSharp.dll");
            var stampPath = Path.Combine(managed, "water.stamp");
            var stamp = StampBuilder.Build(sourceAssembly, request.WaterSlDll, request.WaterLibDll);

            if (!File.Exists(stampPath) || File.ReadAllText(stampPath) != stamp || !File.Exists(targetAssembly))
            {
                FileSync.CopyManaged(sourceManaged, managed);
                File.Copy(request.WaterLibDll, Path.Combine(managed, "WaterLib.dll"), true);
                File.Copy(request.WaterSlDll, Path.Combine(managed, "WaterSL.dll"), true);
                result.Report = AssemblyPatcher.Patch(targetAssembly);
                var scripting = Path.Combine(targetData, "ScriptingAssemblies.json");
                if (File.Exists(scripting))
                    ScriptingAssemblies.Ensure(scripting);
                File.WriteAllText(stampPath, stamp);
            }
            else
            {
                result.UpToDate = true;
                File.Copy(request.WaterLibDll, Path.Combine(managed, "WaterLib.dll"), true);
                File.Copy(request.WaterSlDll, Path.Combine(managed, "WaterSL.dll"), true);
            }

            result.Executable = targetExe;
            result.DataDirectory = targetData;
            WaterApi.Contribute("WaterAssembly", "Assembly yaması");
            return result;
        }

        public static string ResolveExecutable(string gameDirectory)
        {
            return ExecutableResolver.Resolve(gameDirectory);
        }

    }
}
