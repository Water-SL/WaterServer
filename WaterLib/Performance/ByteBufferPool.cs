using System;

namespace Water
{
    public sealed class ByteBufferPool
    {
        private readonly int _size;
        private readonly byte[][] _slots;
        private readonly bool[] _used;

        public ByteBufferPool(int slots, int size)
        {
            if (slots < 1)
                slots = 1;
            if (size < 64)
                size = 64;
            _size = size;
            _slots = new byte[slots][];
            _used = new bool[slots];
            for (var i = 0; i < slots; i++)
                _slots[i] = new byte[size];
        }

        public int Size
        {
            get { return _size; }
        }

        public byte[] Rent()
        {
            lock (_used)
            {
                for (var i = 0; i < _slots.Length; i++)
                {
                    if (_used[i])
                        continue;
                    _used[i] = true;
                    return _slots[i];
                }
            }

            return new byte[_size];
        }

        public void Return(byte[] buffer)
        {
            if (buffer == null)
                return;

            lock (_used)
            {
                for (var i = 0; i < _slots.Length; i++)
                {
                    if (!ReferenceEquals(_slots[i], buffer))
                        continue;
                    _used[i] = false;
                    return;
                }
            }
        }
    }
}
