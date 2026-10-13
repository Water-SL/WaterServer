using Mono.Cecil;

namespace Water.Patch
{

    public interface IPatchHook
    {
        string Name { get; }
        void Apply(Mono.Cecil.ModuleDefinition module, PatchReport report);
    }

}
