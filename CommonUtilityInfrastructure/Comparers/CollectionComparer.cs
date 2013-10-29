namespace CommonUtilityInfrastructure.Comparers
{
    #region Usings

    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class CollectionComparer<T> : IEqualityComparer<ICollection<T>>
        where T : class
    {

        public bool Equals(ICollection<T> first, ICollection<T> second)
        {
            return first.SequenceEqual(second);
        }

        public int GetHashCode(ICollection<T> collection)
        {
            return collection.Aggregate(17, (one, two) => 11 * one.GetHashCode() ^ two.GetHashCode());
        }

    }
}