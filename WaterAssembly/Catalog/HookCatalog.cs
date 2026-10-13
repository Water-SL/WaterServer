namespace Water.Patch
{
    public enum HookKind
    {
        Call,
        Instance,
        BeforeReturn
    }

    public sealed class HookSite
    {
        public HookSite(string typeName, string methodName, int parameters, string hook, HookKind kind)
        {
            TypeName = typeName;
            MethodName = methodName;
            Parameters = parameters;
            Hook = hook;
            Kind = kind;
        }

        public string TypeName { get; }
        public string MethodName { get; }
        public int Parameters { get; }
        public string Hook { get; }
        public HookKind Kind { get; }

        public string Key
        {
            get { return TypeName + "." + MethodName; }
        }
    }

    public static class HookCatalog
    {
        public static readonly HookSite[] Sites =
        {
            new HookSite("ServerConsole", "HandlePlayerJoin", 2, "OnPlayerJoin", HookKind.Instance),
            new HookSite("ReferenceHub", "OnDestroy", 0, "OnPlayerLeave", HookKind.Instance),
            new HookSite("CustomNetworkManager", "OnServerDisconnect", 1, "OnServerDisconnect", HookKind.Call),
            new HookSite("CharacterClassManager", "UserCode_RpcRoundStarted", 0, "OnRoundStart", HookKind.Call),
            new HookSite("RoundSummary", "ForceEnd", 0, "OnRoundEnd", HookKind.Call)
        };
    }
}
