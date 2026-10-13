using System.IO;
using Mono.Cecil;

namespace Water.Patch
{
    public sealed class PatchReport
    {
        public System.Collections.Generic.List<string> Applied { get; } = new System.Collections.Generic.List<string>();
        public System.Collections.Generic.List<string> Skipped { get; } = new System.Collections.Generic.List<string>();

        public bool Changed
        {
            get { return Applied.Count > 0; }
        }
    }

    public static class AssemblyPatcher
    {
        public const string HookAssembly = "WaterSL";
        public const string HookNamespace = "Water.SL";
        public const string HookType = "WaterHooks";

        public static PatchReport Patch(string assemblyPath, string referenceDirectory = null)
        {
            if (string.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
                throw new FileNotFoundException("Assembly-CSharp was not found.", assemblyPath);

            var resolver = new DefaultAssemblyResolver();
            var ownDirectory = Path.GetDirectoryName(assemblyPath);
            if (!string.IsNullOrEmpty(ownDirectory))
                resolver.AddSearchDirectory(ownDirectory);
            if (!string.IsNullOrEmpty(referenceDirectory))
                resolver.AddSearchDirectory(referenceDirectory);

            var report = new PatchReport();
            using (var assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters
            {
                ReadWrite = false,
                InMemory = true,
                ReadSymbols = false,
                AssemblyResolver = resolver
            }))
            {
                var module = assembly.MainModule;
                IlEditor.EnsureReference(module);
                HookPipeline.Apply(module, report);
                DedicatedCull.Apply(module, report);
                NetworkPollTiming.Apply(module, report);
                TickrateClock.Apply(module, report);

                if (report.Applied.Count == 0)
                    return report;

                var temp = assemblyPath + ".water.tmp";
                assembly.Write(temp);
                File.Copy(temp, assemblyPath, true);
                File.Delete(temp);
            }

            return report;
        }

        public static bool IsPatched(string assemblyPath)
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(assemblyPath))
            {
                foreach (var reference in assembly.MainModule.AssemblyReferences)
                {
                    if (reference.Name == HookAssembly)
                        return true;
                }
            }

            return false;
        }
    }
}
