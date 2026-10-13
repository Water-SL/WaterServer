using System;

namespace Water
{

    public sealed class RestartBudget
    {
        private readonly int _max;
        private readonly TimeSpan _window;
        private int _used;
        private DateTime _opened;

        public RestartBudget(int max, TimeSpan window)
        {
            _max = max < 1 ? 1 : max;
            _window = window;
            _opened = DateTime.UtcNow;
        }

        public bool TryTake()
        {
            if (DateTime.UtcNow - _opened > _window)
            {
                _opened = DateTime.UtcNow;
                _used = 0;
            }

            if (_used >= _max)
                return false;

            _used++;
            return true;
        }
    }

}
