using System;

namespace Water
{

    public sealed class Counter
    {
        private long _value;

        public long Value { get { return System.Threading.Interlocked.Read(ref _value); } }

        public void Add(long amount)
        {
            System.Threading.Interlocked.Add(ref _value, amount);
        }

        public void Reset()
        {
            System.Threading.Interlocked.Exchange(ref _value, 0);
        }
    }

}
