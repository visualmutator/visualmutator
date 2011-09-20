namespace VisualMutator.Infrastructure.Comparers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FuncComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparer;

        public FuncComparer(Func<T, T, bool> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            _comparer = comparer;
        }

        public bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            throw new NotSupportedException();
        }
    }
}