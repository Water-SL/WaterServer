namespace Water
{
    public static class Backoff
    {
        public static int Milliseconds(int attempt, int first, int cap)
        {
            if (attempt < 0)
                attempt = 0;
            var delay = first;
            for (var i = 0; i < attempt && delay < cap; i++)
                delay *= 2;
            return delay > cap ? cap : delay;
        }
    }
}
