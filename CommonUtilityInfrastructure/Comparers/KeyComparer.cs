namespace CommonUtilityInfrastructure.Comparers
{
    using System;
    using System.Collections.Generic;

    public class KeyComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> _keyExtractor;

        public KeyComparer(Func<T, object> keyExtractor)
        {
            _keyExtractor = keyExtractor;
        }

        public bool Equals(T x, T y)
        {
            return _keyExtractor(x).Equals(_keyExtractor(y));
        }

        public int GetHashCode(T obj)
        {
            return _keyExtractor(obj).GetHashCode();
        }
    }

}