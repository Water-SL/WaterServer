using Mono.Cecil;

namespace Water.Patch
{

    public sealed class RoundEndHook : IPatchHook
    {
        public string Name { get { return "Round end"; } }

        public void Apply(Mono.Cecil.ModuleDefinition module, PatchReport report)
        {
            IlEditor.Call(module, "RoundSummary", "ForceEnd", 0, "OnRoundEnd", report);
        }
    }

}
