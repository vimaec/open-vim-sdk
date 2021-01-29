using System;
using System.Collections.Generic;
using System.Linq;

namespace Vim.DotNetUtilities
{
    public class DictionaryOfLists<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {
        public DictionaryOfLists()
        { }

        public DictionaryOfLists(IEnumerable<IGrouping<TKey, TValue>> groups)
        {
            foreach (var grp in groups)
                Add(grp.Key, grp.ToList());
        }

        public void Add(TKey k, TValue v)
        {
            if (!ContainsKey(k))
                Add(k, new List<TValue>());
            this[k].Add(v);
        }

        public IEnumerable<TValue> AllValues
            => Values.SelectMany(xs => xs);

        public List<TValue> GetOrDefault(TKey k) {
            if (!ContainsKey(k))
                return new List<TValue>();
            return this[k];
        }
    }

    public static class DictionaryOfListExtensions
    {
        public static DictionaryOfLists<K, V> ToDictionaryOfLists<K, V>(this IEnumerable<V> self, Func<V, K> keySelector)
        {
            var r = new DictionaryOfLists<K, V>();
            foreach (var x in self)
                r.Add(keySelector(x), x);
            return r;
        }

        public static (List<(TKey key, List<TValue> values)> shared, List<(TKey key, TValue value)> unique) SplitByCount<TKey, TValue>(this DictionaryOfLists<TKey, TValue> items)
        {
            var shared = new List<(TKey, List<TValue>)>();
            var unique = new List<(TKey, TValue)>();

            foreach (var item in items)
            {
                if (item.Value.Count > 1)
                    shared.Add((item.Key, item.Value));
                else
                    unique.Add((item.Key, item.Value[0]));
            }
            return (shared, unique);
        }
    }
}
