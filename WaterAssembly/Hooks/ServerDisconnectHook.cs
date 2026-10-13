using Mono.Cecil;

namespace Water.Patch
{

    public sealed class ServerDisconnectHook : IPatchHook
    {
        public string Name { get { return "Server disconnect"; } }

        public void Apply(Mono.Cecil.ModuleDefinition module, PatchReport report)
        {
            IlEditor.Call(module, "CustomNetworkManager", "OnServerDisconnect", 1, "OnServerDisconnect", report);
        }
    }

}
