using Mono.Cecil;

namespace Water.Patch
{

    public sealed class PlayerLeaveHook : IPatchHook
    {
        public string Name { get { return "Player leave"; } }

        public void Apply(Mono.Cecil.ModuleDefinition module, PatchReport report)
        {
            IlEditor.Self(module, "ReferenceHub", "OnDestroy", 0, "OnPlayerLeave", report);
        }
    }

}
