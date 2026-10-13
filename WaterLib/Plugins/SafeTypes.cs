using System;

namespace Water
{

    public static class SafeTypes
    {
        public static Type[] Of(System.Reflection.Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                var found = ex.Types ?? Array.Empty<Type>();
                var count = 0;
                for (var i = 0; i < found.Length; i++)
                    if (found[i] != null)
                        count++;
                var clean = new Type[count];
                var index = 0;
                for (var i = 0; i < found.Length; i++)
                    if (found[i] != null)
                        clean[index++] = found[i]!;
                return clean;
            }
        }
    }

}
