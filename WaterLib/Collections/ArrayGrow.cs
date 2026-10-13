using System;

namespace Water
{

    public static class ArrayGrow
    {
        public static void Ensure<T>(ref T[] array, int needed)
        {
            if (array != null && array.Length >= needed)
                return;

            var size = array == null || array.Length == 0 ? 8 : array.Length;
            while (size < needed)
                size *= 2;

            Array.Resize(ref array, size);
        }
    }

}
