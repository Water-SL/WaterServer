using System;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Water;
using Water.Patch;

namespace Water.Tests
{
    public static class Program
    {
        public static int Main()
        {
            var failed = 0;
            failed += Run("angle", Angle);
            failed += Run("midpoint", Midpoint);
            failed += Run("extrapolate", Extrapolate);
            failed += Run("codec", Codec);
            failed += Run("password", Password);
            failed += Run("bus", Bus);
            failed += Run("scripting", Scripting);
            failed += Run("patch", Patch);
            failed += Run("log", LogRing);
            failed += Run("average", Average);
            failed += Run("commands", Commands);
            failed += Run("plugin", PluginApi);
            failed += Run("boot", BootTests.Run);
            if (failed == 0)
            {
                Console.WriteLine("Water tests passed.");
                return 0;
            }

            Console.WriteLine(failed + " test(s) failed.");
            return 1;
        }

        private static int Run(string name, Action test)
        {
            try
            {
                test();
                Console.WriteLine("  ok  " + name);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("  FAIL " + name + ": " + ex.Message);
                return 1;
            }
        }

        private static void Angle()
        {
            var value = Motion.Repeat(Motion.LerpAngle(350f, 10f, 0.5f), 360f);
            if (value > 0.01f && Math.Abs(value - 360f) > 0.01f)
                throw new Exception("wrapped angle was " + value);
        }

        private static void Midpoint()
        {
            var previous = View(0, 0, 0f);
            var latest = View(100, 10, 10f);
            var frame = new RelayFrame();
            RelayComposer.Compose(previous, latest, 50, frame);
            if (Math.Abs(frame.Players[0].X - 5f) > 0.001f)
                throw new Exception("midpoint x " + frame.Players[0].X);
            if (frame.Extrapolating)
                throw new Exception("midpoint should not extrapolate");
        }

        private static void Extrapolate()
        {
            var previous = View(0, 0, 0f);
            var latest = View(100, 10, 10f);
            var frame = new RelayFrame();
            RelayComposer.Compose(previous, latest, 200, frame);
            if (!frame.Extrapolating)
                throw new Exception("expected extrapolation");
            if (Math.Abs(frame.Alpha - 1.35f) > 0.001f)
                throw new Exception("alpha " + frame.Alpha);
            if (Math.Abs(frame.Players[0].X - 13.5f) > 0.001f)
                throw new Exception("x " + frame.Players[0].X);
        }

        private static void Codec()
        {
            var frame = new RelayFrame { Tick = 42, Timestamp = 99, Alpha = 0.25f, Extrapolating = true };
            frame.Players[0] = new PlayerSample { Id = 7, X = 1.5f, Y = 2f, Z = -3f, Yaw = 12f, Pitch = 4f, Held = 11, Name = "Vatan" };
            frame.PlayerCount = 1;
            frame.Toys[0] = new ToySample { Id = 3, X = 8f, Y = 1f, Z = 2f, Yaw = 90f };
            frame.ToyCount = 1;
            frame.Items[0] = new ItemSample { Id = 5, Type = 4, X = 1f, Y = 2f, Z = 3f, Owner = 7 };
            frame.ItemCount = 1;

            var buffer = new byte[512];
            var written = FrameCodec.Encode(frame, buffer, 4);
            if (written <= 0)
                throw new Exception("encode failed");

            var decoded = new RelayFrame();
            if (!FrameCodec.TryDecode(buffer, 4, written, decoded))
                throw new Exception("decode failed");
            if (decoded.Tick != 42 || decoded.PlayerCount != 1 || decoded.Players[0].Name != "Vatan")
                throw new Exception("roundtrip mismatch");
            if (Math.Abs(decoded.Players[0].X - 1.5f) > 0.001f || decoded.Toys[0].Id != 3 || decoded.Items[0].Owner != 7)
                throw new Exception("sample mismatch");
            if (!decoded.Extrapolating)
                throw new Exception("flag lost");
        }

        private static void Password()
        {
            var stored = PasswordHasher.Hash("correct horse");
            if (!PasswordHasher.Verify("correct horse", stored))
                throw new Exception("valid password rejected");
            if (PasswordHasher.Verify("wrong horse", stored))
                throw new Exception("invalid password accepted");
            if (PasswordHasher.CreateToken().Length != 32)
                throw new Exception("token length");
        }

        private static void Bus()
        {
            var bus = new SnapshotBus();
            var slot = bus.BeginWrite();
            slot.Tick = 4;
            slot.Timestamp = 10;
            slot.AddPlayer(new PlayerSample { Id = 1, X = 2f });
            bus.Publish(slot);
            var read = bus.Read();
            if (read == null || read.Tick != 4 || read.PlayerCount != 1)
                throw new Exception("bus read");
            bus.EndRead();
        }

        private static void Scripting()
        {
            var path = Path.Combine(Path.GetTempPath(), "water-scripting-" + Guid.NewGuid().ToString("N") + ".json");
            try
            {
                File.WriteAllText(path, "{\"names\":[\"LabApi.dll\"],\"types\":[16]}");
                ScriptingAssemblies.Ensure(path);
                var text = File.ReadAllText(path);
                if (text.IndexOf("\"WaterSL.dll\"]", StringComparison.Ordinal) < 0 || text.IndexOf("WaterLib.dll", StringComparison.Ordinal) < 0)
                    throw new Exception(text);
                ScriptingAssemblies.Ensure(path);
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static void Patch()
        {
            var source = @"C:\Program Files (x86)\Steam\steamapps\common\SCP Secret Laboratory Dedicated Server\SCPSL_Data\Managed\Assembly-CSharp.dll";
            if (!File.Exists(source))
            {
                Console.WriteLine("    skip patch, game assembly missing");
                return;
            }

            var directory = Path.Combine(Path.GetTempPath(), "water-patch-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            var copy = Path.Combine(directory, "Assembly-CSharp.dll");
            try
            {
                File.Copy(source, copy);
                var report = AssemblyPatcher.Patch(copy, Path.GetDirectoryName(source));
                Expect(report, "ServerConsole.HandlePlayerJoin");
                Expect(report, "LiteNetLib4MirrorTransport.PollingLoop");
                Expect(report, "LiteNetLib4MirrorServer.ConnectionRequestLoop");
                Expect(report, "FileConsole.Prompt");
                Expect(report, "TcpConsole.Send");
                Expect(report, "TcpConsole.Receive");
                Expect(report, "QueryServer.HandleUsers");
                Expect(report, "ServerStatic.set_ServerTickrate");
                Expect(report, "ReferenceHub.OnDestroy");
                Expect(report, "CharacterClassManager.UserCode_RpcRoundStarted");
                Expect(report, "RoundSummary.ForceEnd");
                Expect(report, "CustomNetworkManager.OnServerDisconnect");
                if (!AssemblyPatcher.IsPatched(copy))
                    throw new Exception("reference missing");

                AssertHook(copy, "ServerConsole", "HandlePlayerJoin", "OnPlayerJoin");
                var second = AssemblyPatcher.Patch(copy, Path.GetDirectoryName(source));
                if (second.Applied.Count != 0)
                    throw new Exception("second patch rewrote " + string.Join(",", second.Applied));
            }
            finally
            {
                try { Directory.Delete(directory, true); } catch { }
            }
        }

        private static void Expect(PatchReport report, string name)
        {
            if (!report.Applied.Contains(name))
                throw new Exception("missing hook " + name + " applied=[" + string.Join(", ", report.Applied) + "] skipped=[" + string.Join(", ", report.Skipped) + "]");
        }

        private static void AssertHook(string path, string typeName, string methodName, string hook)
        {
            using (var assembly = AssemblyDefinition.ReadAssembly(path))
            {
                foreach (var type in assembly.MainModule.Types)
                {
                    if (type.Name != typeName)
                        continue;

                    foreach (var method in type.Methods)
                    {
                        if (method.Name != methodName || !method.HasBody)
                            continue;

                        foreach (var instruction in method.Body.Instructions)
                        {
                            var call = instruction.Operand as MethodReference;
                            if (call != null && call.Name == hook)
                                return;
                        }
                    }
                }
            }

            throw new Exception(typeName + "." + methodName + " has no " + hook);
        }

        private static void LogRing()
        {
            WaterLog.Minimum = LogLevel.Debug;
            WaterLog.Write(LogLevel.Info, "water-test-log");
            var recent = WaterLog.Recent();
            var found = false;
            for (var i = 0; i < recent.Length; i++)
            {
                if (recent[i].Message == "water-test-log")
                    found = true;
            }

            if (!found)
                throw new Exception("log missing");
        }

        private static void Average()
        {
            var meter = new MovingAverage(0.5);
            meter.Push(10);
            var value = meter.Push(20);
            if (Math.Abs(value - 15) > 0.001)
                throw new Exception("average " + value);
        }

        private static void PluginApi()
        {
            var seen = "";
            Action<PlayerJoinInfo> join = player => seen = player.Name;
            PlayerEvents.Joined += join;
            try
            {
                PlayerEvents.RaiseJoined(new PlayerJoinInfo { Id = 9, Name = "Ada" });
                if (seen != "Ada")
                    throw new Exception("join event");
                PlayerJoinInfo found;
                if (!WaterPlayers.TryGet(9, out found) || found.Name != "Ada")
                    throw new Exception("player list");

                var plugin = new SamplePlugin();
                plugin.Enable();
                if (!CommandBus.TryRun("water-sample"))
                    throw new Exception("plugin command");
                plugin.Disable();
            }
            finally
            {
                PlayerEvents.Joined -= join;
                PlayerEvents.RaiseLeft(new PlayerJoinInfo { Id = 9, Name = "Ada" });
            }
        }

        private sealed class SamplePlugin : WaterPlugin
        {
            public override string Name { get { return "Sample"; } }

            protected override void OnEnabled()
            {
                RegisterCommand("water-sample", "test", _ => { });
            }
        }

        private static void Commands()
        {
            var hit = false;
            CommandBus.Register("watertest", "test", _ => hit = true);
            if (!CommandBus.TryRun("watertest now"))
                throw new Exception("command missed");
            if (!hit)
                throw new Exception("handler missed");
            if (!SessionFrames.HasToken(System.Text.Encoding.ASCII.GetBytes("abc-payload"), "abc"))
                throw new Exception("token prefix");
            if (HookCatalog.Sites.Length != 5)
                throw new Exception("hook catalog " + HookCatalog.Sites.Length);
        }

        private static SnapshotView View(long time, long tick, float x)
        {
            return new SnapshotView
            {
                Timestamp = time,
                Tick = tick,
                Players = new[] { new PlayerSample { Id = 1, X = x, Name = "a" } },
                PlayerCount = 1,
                Toys = Array.Empty<ToySample>(),
                Items = Array.Empty<ItemSample>()
            };
        }
    }
}
