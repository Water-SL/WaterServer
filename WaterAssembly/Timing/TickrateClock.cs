using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Water.Patch
{
    public static class TickrateClock
    {
        public static void Apply(ModuleDefinition module, PatchReport report)
        {
            var method = FindSetter(module);
            if (method == null)
            {
                report.Skipped.Add("ServerStatic.set_ServerTickrate was not found.");
                return;
            }

            foreach (var instruction in method.Body.Instructions)
            {
                var called = instruction.Operand as MethodReference;
                if (called != null && called.Name == "set_fixedDeltaTime")
                {
                    report.Skipped.Add("ServerStatic.set_ServerTickrate clock already set.");
                    return;
                }
            }

            Instruction frameRate = null;
            foreach (var instruction in method.Body.Instructions)
            {
                var called = instruction.Operand as MethodReference;
                if (called != null && called.Name == "set_targetFrameRate")
                    frameRate = instruction;
            }

            if (frameRate == null || frameRate.Next == null)
            {
                report.Skipped.Add("ServerStatic.set_ServerTickrate has no frame-rate write.");
                return;
            }

            var tickrate = FindTickrate(module);
            var vsync = ImportSetter(module, "UnityEngine.QualitySettings", "set_vSyncCount");
            var step = ImportSetter(module, "UnityEngine.Time", "set_fixedDeltaTime");
            var il = method.Body.GetILProcessor();
            var next = frameRate.Next;
            il.InsertBefore(next, il.Create(OpCodes.Ldc_I4_0));
            il.InsertBefore(next, il.Create(OpCodes.Call, vsync));
            il.InsertBefore(next, il.Create(OpCodes.Ldc_R4, 1f));
            il.InsertBefore(next, il.Create(OpCodes.Ldsfld, tickrate));
            il.InsertBefore(next, il.Create(OpCodes.Conv_R4));
            il.InsertBefore(next, il.Create(OpCodes.Div));
            il.InsertBefore(next, il.Create(OpCodes.Call, step));
            if (method.Body.MaxStackSize < 2)
                method.Body.MaxStackSize = 2;
            report.Applied.Add("ServerStatic.set_ServerTickrate");
        }

        private static MethodReference ImportSetter(ModuleDefinition module, string typeName, string methodName)
        {
            AssemblyNameReference reference = null;
            foreach (var candidate in module.AssemblyReferences)
            {
                if (candidate.Name == "UnityEngine.CoreModule")
                {
                    reference = candidate;
                    break;
                }
            }

            if (reference == null)
                throw new System.InvalidOperationException("UnityEngine.CoreModule is not referenced.");

            var resolved = module.AssemblyResolver.Resolve(reference);
            TypeDefinition type = null;
            foreach (var candidate in resolved.MainModule.Types)
            {
                if (candidate.FullName == typeName)
                {
                    type = candidate;
                    break;
                }
            }

            if (type == null)
                throw new System.InvalidOperationException(typeName + " was not found.");

            MethodDefinition method = null;
            foreach (var candidate in type.Methods)
            {
                if (candidate.Name == methodName && candidate.Parameters.Count == 1)
                {
                    method = candidate;
                    break;
                }
            }

            if (method == null)
                throw new System.InvalidOperationException(methodName + " was not found.");

            return module.ImportReference(method);
        }

        private static FieldDefinition FindTickrate(ModuleDefinition module)
        {
            foreach (var type in module.Types)
            {
                if (type.Name != "ServerStatic")
                    continue;
                foreach (var field in type.Fields)
                {
                    if (field.Name == "_serverTickrate")
                        return field;
                }
            }

            throw new System.InvalidOperationException("ServerStatic._serverTickrate was not found.");
        }

        private static MethodDefinition FindSetter(ModuleDefinition module)
        {
            foreach (var type in module.Types)
            {
                if (type.Name != "ServerStatic")
                    continue;
                foreach (var method in type.Methods)
                {
                    if (method.Name == "set_ServerTickrate" && method.HasBody)
                        return method;
                }
            }

            return null;
        }
    }
}
