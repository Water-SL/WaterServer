using System;

namespace Water
{
    public static class RoundEvents
    {
        public static event Action<RoundInfo>? Started;

        public static event Action<RoundInfo>? Ended;

        public static void RaiseStarted(RoundInfo round)
        {
            if (round == null)
                round = new RoundInfo();

            WaterApi.RoundStarted.Publish(round);
            var handler = Started;
            if (handler != null)
                handler(round);
        }

        public static void RaiseEnded(RoundInfo round)
        {
            if (round == null)
                round = new RoundInfo();

            WaterApi.RoundEnded.Publish(round);
            var handler = Ended;
            if (handler != null)
                handler(round);
        }
    }
}
