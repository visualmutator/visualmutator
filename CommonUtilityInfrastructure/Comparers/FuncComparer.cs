namespace VisualMutator.Infrastructure.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FuncComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;

        private readonly Func<T,int> _getHashCode;

        public FuncComparer(Func<T, T, bool> comparer, Func<T,int> getHashCode)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _comparer = comparer;
            _getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
}