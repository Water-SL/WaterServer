using Mono.Cecil;

namespace Water.Patch
{

    public sealed class PlayerJoinHook : IPatchHook
    {
        public string Name { get { return "Player join"; } }

        public void Apply(Mono.Cecil.ModuleDefinition module, PatchReport report)
        {
            IlEditor.Join(module, report);
        }
    }

}
