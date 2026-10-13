using System;

namespace Water
{

    public sealed class Gauge
    {
        private int _value;

        public int Value { get { return System.Threading.Volatile.Read(ref _value); } }

        public void Set(int value)
        {
            System.Threading.Volatile.Write(ref _value, value);
        }
    }

}
