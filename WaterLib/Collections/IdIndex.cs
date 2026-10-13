using System;

namespace Water
{

    public sealed class IdIndex
    {
        private int[] _ids = new int[32];
        private int[] _slots = new int[32];
        private int _count;

        public void Clear()
        {
            _count = 0;
        }

        public void Add(int id, int slot)
        {
            ArrayGrow.Ensure(ref _ids, _count + 1);
            ArrayGrow.Ensure(ref _slots, _count + 1);
            _ids[_count] = id;
            _slots[_count] = slot;
            _count++;
        }

        public int Find(int id)
        {
            for (var i = 0; i < _count; i++)
            {
                if (_ids[i] == id)
                    return _slots[i];
            }

            return -1;
        }
    }

}
