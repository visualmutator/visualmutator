namespace VisualMutator
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableUtils
    {
         public static string MakeString<T>(this IEnumerable<T> enumerable, string delimiter)
         {
            if (enumerable.Any())
            {
                return "[" + enumerable.Select(a => a.ToString()).Aggregate((a, b) => a + delimiter + b) + "]";
            }
            else
            {
                return "[]";
            }
        }
        public static string MakeString<T>(this IEnumerable<T> enumerable)
        {
            return MakeString(enumerable, ", ");
        }
        public static string MakeString<T>(this IEnumerable<T> enumerable, char delimiter)
        {
            return MakeString(enumerable, delimiter.ToString());
        }
    }
}