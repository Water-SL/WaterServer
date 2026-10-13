using Mono.Cecil;

namespace Water.Patch
{

    public sealed class RoundStartHook : IPatchHook
    {
        public string Name { get { return "Round start"; } }

        public void Apply(Mono.Cecil.ModuleDefinition module, PatchReport report)
        {
            IlEditor.Call(module, "CharacterClassManager", "UserCode_RpcRoundStarted", 0, "OnRoundStart", report);
        }
    }

}
