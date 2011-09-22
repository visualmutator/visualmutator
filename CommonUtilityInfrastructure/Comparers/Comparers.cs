namespace VisualMutator.Infrastructure.Comparers
{
    using System;

    public static class Comparers
    {
       
        public static KeyComparer<T> Key<T>(Func<T, object> keyExtractor)
        {
            return new KeyComparer<T>(keyExtractor);
        }
        public static CollectionComparer<T> Collection<T>() where T : class, IComparable<T>
        {
            return new CollectionComparer<T>();
        }
    }
}