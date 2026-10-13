using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Water.Patch
{
    public static class IlEditor
    {
        public static void EnsureReference(ModuleDefinition module)
        {
            foreach (var reference in module.AssemblyReferences)
            {
                if (reference.Name == AssemblyPatcher.HookAssembly)
                    return;
            }

            module.AssemblyReferences.Add(new AssemblyNameReference(AssemblyPatcher.HookAssembly, new Version(1, 0, 0, 0)));
        }

        public static void Call(ModuleDefinition module, string typeName, string methodName, int parameters, string hookName, PatchReport report)
        {
            var method = FindMethod(module, typeName, methodName, parameters);
            if (method == null)
            {
                report.Skipped.Add(typeName + "." + methodName + " was not found.");
                return;
            }

            if (HasHook(method))
            {
                report.Skipped.Add(typeName + "." + methodName + " already patched.");
                return;
            }

            var hook = Import(module, hookName);
            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions[0];
            il.InsertBefore(first, il.Create(OpCodes.Call, hook));
            method.Body.MaxStackSize = Math.Max(method.Body.MaxStackSize, 2);
            report.Applied.Add(typeName + "." + methodName);
        }

        public static void Self(ModuleDefinition module, string typeName, string methodName, int parameters, string hookName, PatchReport report)
        {
            var method = FindMethod(module, typeName, methodName, parameters);
            if (method == null)
            {
                report.Skipped.Add(typeName + "." + methodName + " was not found.");
                return;
            }

            if (HasHook(method))
            {
                report.Skipped.Add(typeName + "." + methodName + " already patched.");
                return;
            }

            var hook = Import(module, hookName, module.TypeSystem.Object);
            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions[0];
            var call = il.Create(OpCodes.Call, hook);
            var self = il.Create(OpCodes.Ldarg_0);
            il.InsertBefore(first, call);
            il.InsertBefore(call, self);
            method.Body.MaxStackSize = Math.Max(method.Body.MaxStackSize, 2);
            report.Applied.Add(typeName + "." + methodName);
        }

        public static void Join(ModuleDefinition module, PatchReport report)
        {
            var method = FindMethod(module, "ServerConsole", "HandlePlayerJoin", 2);
            if (method == null)
            {
                report.Skipped.Add("ServerConsole.HandlePlayerJoin was not found.");
                return;
            }

            if (HasHook(method))
            {
                report.Skipped.Add("ServerConsole.HandlePlayerJoin already patched.");
                return;
            }

            var hook = Import(module, "OnPlayerJoin", module.TypeSystem.Object, module.TypeSystem.Int32);
            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions[0];
            var call = il.Create(OpCodes.Call, hook);
            var mode = il.Create(OpCodes.Ldarg_1);
            var hub = il.Create(OpCodes.Ldarg_0);
            il.InsertBefore(first, call);
            il.InsertBefore(call, mode);
            il.InsertBefore(mode, hub);
            method.Body.MaxStackSize = Math.Max(method.Body.MaxStackSize, 4);
            report.Applied.Add("ServerConsole.HandlePlayerJoin");
        }

        public static void PositionPass(ModuleDefinition module, PatchReport report)
        {
            var method = FindMethod(module, "FpcServerPositionDistributor", "LateUpdate", 0);
            if (method == null)
            {
                report.Skipped.Add("FpcServerPositionDistributor.LateUpdate was not found.");
                return;
            }

            if (HasHook(method))
            {
                report.Skipped.Add("FpcServerPositionDistributor.LateUpdate already patched.");
                return;
            }

            var begin = Import(module, "BeginPositionPass");
            var end = Import(module, "EndPositionPass");
            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions[0];
            il.InsertBefore(first, il.Create(OpCodes.Call, begin));

            var returns = method.Body.Instructions.Where(instruction => instruction.OpCode == OpCodes.Ret).ToArray();
            for (var i = 0; i < returns.Length; i++)
                il.InsertBefore(returns[i], il.Create(OpCodes.Call, end));

            method.Body.MaxStackSize = Math.Max(method.Body.MaxStackSize, 2);
            report.Applied.Add("FpcServerPositionDistributor.LateUpdate");
        }

        public static void Send(ModuleDefinition module, PatchReport report)
        {
            var method = FindMethod(module, "LiteNetLib4MirrorServer", "Send", 6);
            if (method == null)
            {
                report.Skipped.Add("LiteNetLib4MirrorServer.Send was not found.");
                return;
            }

            if (HasHook(method))
            {
                report.Skipped.Add("LiteNetLib4MirrorServer.Send already patched.");
                return;
            }

            var parameters = method.Parameters;
            var hook = Import(
                module,
                "CaptureOutgoing",
                module.TypeSystem.Int32,
                module.TypeSystem.Int32,
                new ArrayType(module.TypeSystem.Byte),
                module.TypeSystem.Int32,
                module.TypeSystem.Int32,
                module.TypeSystem.Byte);

            var il = method.Body.GetILProcessor();
            var first = method.Body.Instructions[0];
            var call = il.Create(OpCodes.Call, hook);
            il.InsertBefore(first, call);
            Instruction previous = call;
            for (var i = parameters.Count - 1; i >= 0; i--)
            {
                var load = il.Create(OpCodes.Ldarg, parameters[i]);
                il.InsertBefore(previous, load);
                previous = load;
            }

            method.Body.MaxStackSize = Math.Max(method.Body.MaxStackSize, 8);
            report.Applied.Add("LiteNetLib4MirrorServer.Send");
        }

        public static MethodDefinition FindMethod(ModuleDefinition module, string typeName, string methodName, int parameterCount)
        {
            foreach (var type in module.Types)
            {
                if (!TypeMatches(type, typeName))
                    continue;

                foreach (var method in type.Methods)
                {
                    if (method.Name == methodName && method.HasBody && method.Parameters.Count == parameterCount)
                        return method;
                }
            }

            return null;
        }

        public static bool TypeMatches(TypeDefinition type, string typeName)
        {
            if (type.Name == typeName)
                return true;

            var index = type.FullName.LastIndexOf('.');
            var shortName = index < 0 ? type.FullName : type.FullName.Substring(index + 1);
            return shortName == typeName;
        }

        public static bool HasHook(MethodDefinition method)
        {
            if (method == null || !method.HasBody)
                return false;

            foreach (var instruction in method.Body.Instructions)
            {
                var reference = instruction.Operand as MethodReference;
                if (reference != null && reference.DeclaringType != null && reference.DeclaringType.Name == AssemblyPatcher.HookType)
                    return true;
            }

            return false;
        }

        public static MethodReference Import(ModuleDefinition module, string name, params TypeReference[] parameters)
        {
            AssemblyNameReference assembly = null;
            foreach (var reference in module.AssemblyReferences)
            {
                if (reference.Name == AssemblyPatcher.HookAssembly)
                {
                    assembly = reference;
                    break;
                }
            }

            if (assembly == null)
                throw new InvalidOperationException("WaterSL reference is missing.");

            var type = new TypeReference(AssemblyPatcher.HookNamespace, AssemblyPatcher.HookType, module, assembly);
            var method = new MethodReference(name, module.TypeSystem.Void, type)
            {
                HasThis = false
            };

            for (var i = 0; i < parameters.Length; i++)
                method.Parameters.Add(new ParameterDefinition(parameters[i]));

            return module.ImportReference(method);
        }
    }
}
