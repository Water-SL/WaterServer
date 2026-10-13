using Mono.Cecil;

namespace Water.Patch
{
    public static class HookPipeline
    {
        private static readonly IPatchHook[] Hooks =
        {
            new PlayerJoinHook(),
            new PlayerLeaveHook(),
            new ServerDisconnectHook(),
            new RoundStartHook(),
            new RoundEndHook()
        };

        public static void Apply(ModuleDefinition module, PatchReport report)
        {
            for (var i = 0; i < Hooks.Length; i++)
                Hooks[i].Apply(module, report);
        }

        public static int Count { get { return Hooks.Length; } }
    }
}
