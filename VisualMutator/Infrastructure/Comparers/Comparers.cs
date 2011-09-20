namespace VisualMutator.Infrastructure.Comparers
{
    using System;

    public static class Comparers
    {
        public static FuncComparer<T> Func<T>(Func<T, T, bool> comparer)
        {
            return new FuncComparer<T>(comparer);
        }
        public static KeyComparer<T> Key<T>(Func<T, object> keyExtractor)
        {
            return new KeyComparer<T>(keyExtractor);
        }
    }
}