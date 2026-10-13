using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Water.Patch
{
    public static class DedicatedCull
    {
        private static readonly string[] ExactTypes =
        {
            "LiquidWobble",
            "ExplosionCameraShake",
            "CameraShakeController",
            "CrosshairController",
            "FirearmCrosshairBase",
            "FogController",
            "SpectatorAttachmentSelector",
            "SpectatorSelectorFirearmButton",
            "ViewmodelCamera",
            "AnimatedViewmodelBase",
            "HideHUDController",
            "MainMenuCamera",
            "HumeShieldBarController",
            "Scp106Minimap",
            "InventoryDrawersController",
            "WeaponShootAnimation",
            "FakeSoundScope",
            "GlowInTheDarkSubcontroller",
            "DoorAnimateAlarmsExtension",
            "AnimatorConditionalOverride",
            "KeycodeField",
            "FlipbookPlayer",
            "RadialMenuBase",
            "DisruptorScopeTrackerExtension",
            "AttachmentPresetSelector"
        };

        private static readonly string[] NameParts =
        {
            "Viewmodel",
            "Hud",
            "GUI",
            "Gui",
            "Minimap",
            "Crosshair",
            "Tooltip",
            "Menu"
        };

        public static void Apply(ModuleDefinition module, PatchReport report)
        {
            var guard = FindDedicatedGetter(module);
            if (guard == null)
            {
                report.Skipped.Add("ServerStatic.IsDedicated was not found.");
                return;
            }

            var imported = module.ImportReference(guard);
            foreach (var type in module.Types)
                Walk(type, imported, report);
        }

        private static void Walk(TypeDefinition type, MethodReference guard, PatchReport report)
        {
            if (IsClientVisual(type))
            {
                foreach (var method in type.Methods)
                {
                    if (!IsFrameMethod(method) || TouchesServer(method))
                        continue;

                    Cull(method, guard, report);
                }
            }

            foreach (var nested in type.NestedTypes)
                Walk(nested, guard, report);
        }

        private static bool IsClientVisual(TypeDefinition type)
        {
            for (var i = 0; i < ExactTypes.Length; i++)
            {
                if (type.Name == ExactTypes[i])
                    return true;
            }

            for (var i = 0; i < NameParts.Length; i++)
            {
                if (type.Name.IndexOf(NameParts[i], System.StringComparison.Ordinal) >= 0)
                    return true;
            }

            return false;
        }

        private static bool IsFrameMethod(MethodDefinition method)
        {
            if (method == null || !method.HasBody || method.IsAbstract)
                return false;
            if (method.ReturnType.FullName != "System.Void")
                return false;
            if (method.Body.Instructions.Count < 4)
                return false;

            return method.Name == "Update" || method.Name == "LateUpdate" || method.Name == "FixedUpdate";
        }

        private static bool TouchesServer(MethodDefinition method)
        {
            foreach (var instruction in method.Body.Instructions)
            {
                var called = instruction.Operand as MethodReference;
                if (called == null || called.DeclaringType == null)
                    continue;

                var owner = called.DeclaringType.FullName;
                if (owner.IndexOf("NetworkServer", System.StringComparison.Ordinal) >= 0)
                    return true;
                if (owner.IndexOf("ServerConsole", System.StringComparison.Ordinal) >= 0)
                    return true;
                if (called.Name == "get_isLocalPlayer" || called.Name == "get_isServer" || called.Name == "get_IsDedicated")
                    return true;
            }

            return false;
        }

        private static void Cull(MethodDefinition method, MethodReference guard, PatchReport report)
        {
            var key = method.DeclaringType.Name + "." + method.Name;
            var first = method.Body.Instructions[0];
            var existing = first.Operand as MethodReference;
            if (first.OpCode == OpCodes.Call && existing != null && existing.Name == "get_IsDedicated")
            {
                report.Skipped.Add(key + " already culled.");
                return;
            }

            var il = method.Body.GetILProcessor();
            var ret = il.Create(OpCodes.Ret);
            var branch = il.Create(OpCodes.Brfalse, first);
            var call = il.Create(OpCodes.Call, guard);
            il.InsertBefore(first, ret);
            il.InsertBefore(ret, branch);
            il.InsertBefore(branch, call);
            if (method.Body.MaxStackSize < 1)
                method.Body.MaxStackSize = 1;
            report.Applied.Add(key);
        }

        private static MethodDefinition FindDedicatedGetter(ModuleDefinition module)
        {
            foreach (var type in module.Types)
            {
                if (type.Name != "ServerStatic")
                    continue;

                foreach (var method in type.Methods)
                {
                    if (method.Name == "get_IsDedicated" && method.IsStatic)
                        return method;
                }
            }

            return null;
        }
    }
}
