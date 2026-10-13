using System;
using System.Runtime.CompilerServices;
using Water;

namespace Water.SL
{
    public static class WaterHooks
    {
        public static readonly bool Armed;

        static WaterHooks()
        {
            try
            {
                var session = Environment.GetEnvironmentVariable("WATER_SESSION");
                Armed = !string.IsNullOrEmpty(session) && session.Length == WaterProduct.TokenLength;
                if (!Armed)
                    return;

                GameBridge.Initialize();
            }
            catch (Exception ex)
            {
                Armed = false;
                GameBridge.TryLog("Water did not arm: " + ex.GetType().Name);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnPlayerJoin(object hub, int mode)
        {
            if (!Armed || hub == null || mode != 1)
                return;

            try
            {
                GameBridge.AnnounceJoin(hub);
            }
            catch
            {
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnPlayerLeave(object hub)
        {
            if (!Armed || hub == null)
                return;

            try
            {
                GameBridge.AnnounceLeave(hub);
            }
            catch
            {
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnServerDisconnect()
        {
            if (!Armed)
                return;

            GameBridge.NoteDisconnect();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnRoundStart()
        {
            if (!Armed)
                return;

            GameBridge.NoteRound(true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void OnRoundEnd()
        {
            if (!Armed)
                return;

            GameBridge.NoteRound(false);
        }
    }
}
