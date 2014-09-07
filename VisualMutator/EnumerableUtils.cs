namespace VisualMutator
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableUtils
    {
         public static string MakeString<T>(this IEnumerable<T> enumerable, char delimiter)
         {
             return enumerable.Select(a => a.ToString()).Aggregate("", (a, b) => a + delimiter + b);
         }
        public static string MakeString<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(a => a.ToString()).Aggregate("", (a, b) => a + ", " + b);
        }
    }
}