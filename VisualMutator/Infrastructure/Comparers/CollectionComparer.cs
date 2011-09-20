namespace VisualMutator.Infrastructure.Comparers
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
            if ((first == null) != (second == null))
            {
                return false;
            }

            if (!ReferenceEquals(first, second))
            {
                if (first.Count != second.Count)
                {
                    return false;
                }

                if ((first.Count != 0) && HaveMismatchedElement(first, second))
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(ICollection<T> enumerable)
        {
            return enumerable.OrderBy(x => x).Aggregate(17, (current, val) => current * 23 + val.GetHashCode());
        }

        private static bool HaveMismatchedElement(IEnumerable<T> first,
                                                  IEnumerable<T> second)
        {
            int firstCount;
            int secondCount;

            var firstElementCounts = GetElementCounts(first, out firstCount);
            var secondElementCounts = GetElementCounts(second, out secondCount);

            if (firstCount != secondCount)
            {
                return true;
            }

            foreach (var kvp in firstElementCounts)
            {
                firstCount = kvp.Value;
                secondElementCounts.TryGetValue(kvp.Key, out secondCount);

                if (firstCount != secondCount)
                {
                    return true;
                }
            }

            return false;
        }

        private static Dictionary<T, int> GetElementCounts(IEnumerable<T> enumerable,
                                                           out int nullCount)
        {
            var dictionary = new Dictionary<T, int>();
            nullCount = 0;

            foreach (T element in enumerable)
            {
                if (element == null)
                {
                    nullCount++;
                }
                else
                {
                    int num;
                    dictionary.TryGetValue(element, out num);
                    num++;
                    dictionary[element] = num;
                }
            }

            return dictionary;
        }
    }
}