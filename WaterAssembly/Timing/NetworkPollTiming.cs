using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Water.Patch
{
    public static class NetworkPollTiming
    {
        public static void Apply(ModuleDefinition module, PatchReport report)
        {
            Tighten(module, "LiteNetLib4MirrorTransport", "PollingLoop", 30, report);
            Tighten(module, "LiteNetLib4MirrorServer", "ConnectionRequestLoop", 30, report);
            Tighten(module, "FileConsole", "Prompt", 25, report);
            Tighten(module, "TcpConsole", "Send", 25, report);
            Tighten(module, "TcpConsole", "Receive", 20, report);
            Tighten(module, "QueryServer", "HandleUsers", 15, report);
        }

        private static void Tighten(ModuleDefinition module, string typeName, string methodName, int fromMilliseconds, PatchReport report)
        {
            var method = Find(module, typeName, methodName);
            if (method == null)
            {
                report.Skipped.Add(typeName + "." + methodName + " was not found.");
                return;
            }

            var changed = false;
            Instruction previous = null;
            foreach (var instruction in method.Body.Instructions)
            {
                var called = instruction.Operand as MethodReference;
                if (previous != null && called != null && called.Name == "Sleep" && called.DeclaringType != null && called.DeclaringType.Name == "Thread")
                {
                    if (Read(previous) == fromMilliseconds)
                    {
                        previous.Operand = (sbyte)1;
                        changed = true;
                    }
                }

                previous = instruction;
            }

            if (changed)
                report.Applied.Add(typeName + "." + methodName);
            else
                report.Skipped.Add(typeName + "." + methodName + " sleep already tightened.");
        }

        private static int Read(Instruction instruction)
        {
            if (instruction.OpCode != OpCodes.Ldc_I4_S)
                return -1;
            if (instruction.Operand is sbyte signed)
                return signed;
            if (instruction.Operand is int value)
                return value;
            return -1;
        }

        private static MethodDefinition Find(ModuleDefinition module, string typeName, string methodName)
        {
            foreach (var type in module.Types)
            {
                if (type.Name != typeName)
                    continue;
                foreach (var method in type.Methods)
                {
                    if (method.Name == methodName && method.HasBody)
                        return method;
                }
            }

            return null;
        }
    }
}
