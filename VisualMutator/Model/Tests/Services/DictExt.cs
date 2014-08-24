namespace VisualMutator.Model.Tests.Services
{
    using System.Collections.Generic;

    public static class DictExt
    {
        public static V GetOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V val;
            return dict.TryGetValue(key, out val) ? val : defaultValue;
        }
    }
}