using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Water;

namespace Water.SL
{
    internal static class GameBridge
    {
        private static readonly HashSet<int> Announced = new HashSet<int>();

        private static Assembly _game;
        private static Type _hubType;
        private static FieldInfo _nickField;
        private static MemberInfo _nickValue;
        private static MemberInfo _playerId;
        private static MethodInfo _addLog;
        private static MethodInfo _sendConsole;
        private static FieldInfo _consoleField;

        public static void Initialize()
        {
            WaterApi.Contribute("WaterSL", "Oyun çalışma zamanı");
            _game = FindAssembly("Assembly-CSharp");
            if (_game == null)
                throw new InvalidOperationException("Assembly-CSharp is not loaded.");

            _hubType = _game.GetType("ReferenceHub", false);
            _nickField = _hubType?.GetField("nicknameSync", BindingFlags.Public | BindingFlags.Instance);
            _consoleField = _hubType?.GetField("gameConsoleTransmission", BindingFlags.Public | BindingFlags.Instance);

            var serverConsole = _game.GetType("ServerConsole", false);
            _addLog = serverConsole?.GetMethod("AddLog", BindingFlags.Public | BindingFlags.Static);
            TryLog("Water armed.");
            PluginHost.Load(TryLog);
        }

        public static void AnnounceJoin(object hub)
        {
            int id;
            string name;
            TryReadIdentity(hub, out id, out name);
            if (id != 0 && !Announced.Add(id))
                return;

            var transmission = _consoleField?.GetValue(hub);
            if (transmission != null)
            {
                if (_sendConsole == null)
                    _sendConsole = transmission.GetType().GetMethod("SendToClient", BindingFlags.Public | BindingFlags.Instance);

                if (_sendConsole != null)
                    _sendConsole.Invoke(transmission, new object[] { WaterProduct.JoinMessage, "blue" });
                else
                    TryLog("GameConsoleTransmission.SendToClient was not found.");
            }

            TryLog(WaterProduct.JoinMessage);
            PlayerEvents.RaiseJoined(new PlayerJoinInfo
            {
                Id = id,
                Name = name ?? "",
                Hub = hub
            });
        }

        public static void AnnounceLeave(object hub)
        {
            int id;
            string name;
            TryReadIdentity(hub, out id, out name);
            if (id != 0)
                Announced.Remove(id);

            PlayerEvents.RaiseLeft(new PlayerJoinInfo
            {
                Id = id,
                Name = name ?? "",
                Hub = hub
            });
        }

        public static void NoteRound(bool started)
        {
            var info = new RoundInfo { Phase = started ? "start" : "end" };
            if (started)
                RoundEvents.RaiseStarted(info);
            else
                RoundEvents.RaiseEnded(info);

            TryLog(started ? "Water round started." : "Water round ended.");
        }

        public static void NoteDisconnect()
        {
            WaterLog.Write(LogLevel.Debug, "server disconnect");
        }

        public static void TryLog(string message)
        {
            try
            {
                if (_addLog == null && _game != null)
                {
                    var serverConsole = _game.GetType("ServerConsole", false);
                    _addLog = serverConsole?.GetMethod("AddLog", BindingFlags.Public | BindingFlags.Static);
                }

                if (_addLog != null)
                    _addLog.Invoke(null, new object[] { message, ConsoleColor.Blue, false });
            }
            catch
            {
            }

            WaterApi.WriteLog(message);
        }

        private static void TryReadIdentity(object hub, out int id, out string name)
        {
            id = 0;
            name = "";
            if (_playerId == null && _hubType != null)
                _playerId = FindMember(_hubType, "PlayerId") ?? FindMember(_hubType, "playerId");

            if (_playerId != null)
                id = ReadInt(_playerId, hub);

            if (id == 0)
                id = RuntimeHelpers.GetHashCode(hub);

            var sync = _nickField?.GetValue(hub);
            if (sync == null)
                return;

            if (_nickValue == null)
            {
                var type = sync.GetType();
                _nickValue = FindMember(type, "DisplayName")
                    ?? FindMember(type, "_displayName")
                    ?? FindMember(type, "Network_displayName")
                    ?? FindMember(type, "MyNick");
            }

            if (_nickValue == null)
                return;

            var value = ReadObject(_nickValue, sync) as string;
            if (!string.IsNullOrEmpty(value))
                name = value;
        }

        private static Assembly FindAssembly(string assemblyName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (var i = 0; i < assemblies.Length; i++)
            {
                if (string.Equals(assemblies[i].GetName().Name, assemblyName, StringComparison.Ordinal))
                    return assemblies[i];
            }

            return null;
        }

        private static MemberInfo FindMember(Type type, string memberName)
        {
            if (type == null)
                return null;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            var property = type.GetProperty(memberName, flags);
            if (property != null)
                return property;

            return type.GetField(memberName, flags);
        }

        private static object ReadObject(MemberInfo member, object target)
        {
            var property = member as PropertyInfo;
            if (property != null)
                return property.GetValue(target, null);

            return ((FieldInfo)member).GetValue(target);
        }

        private static int ReadInt(MemberInfo member, object target)
        {
            var value = ReadObject(member, target);
            if (value == null)
                return 0;
            if (value is int)
                return (int)value;
            if (value is byte)
                return (byte)value;
            if (value is ushort)
                return (ushort)value;

            var type = value.GetType();
            var field = type.GetField("Value") ?? type.GetField("NetId") ?? type.GetField("_value");
            if (field != null && field.FieldType == typeof(int))
                return (int)field.GetValue(value);

            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType == typeof(int))
                    return (int)fields[i].GetValue(value);
            }

            return 0;
        }
    }
}
