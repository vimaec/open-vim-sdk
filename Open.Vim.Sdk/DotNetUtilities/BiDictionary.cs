using System.Collections;
using System.Collections.Generic;

// https://stackoverflow.com/questions/268321/bidirectional-1-to-1-dictionary-in-c-sharp
// https://stackoverflow.com/questions/255341/getting-multiple-keys-of-specified-value-of-a-generic-dictionary
namespace Vim.DotNetUtilities
{
    /// <summary>
    /// A Dictionary where values are unique and can be used to look up keys. 
    /// </summary>
    public interface IBiDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        TKey KeyFromValue(TValue value);
        bool TryGetKey(TValue value, out TKey key);
        bool ContainsValue(TValue value);
        bool RemoveByValue(TValue value);
    }

    /// <summary>
    /// A Dictionary where values are unique and can be used to look up keys. 
    /// </summary>
    public class BiDictionary<TKey, TValue> : IBiDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _d1 
            = new Dictionary<TKey, TValue>();
        
        private readonly Dictionary<TValue, TKey> _d2 
            = new Dictionary<TValue, TKey>();

        public void Add(TKey key, TValue value)
        {
            _d1.Add(key, value);
            _d2.Add(value, key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
            => Add(item.Key, item.Value);

        public void Clear()
        {
            _d1.Clear();
            _d2.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
            => ContainsKey(item.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kv in this)
                array[arrayIndex++] = kv;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
            => Remove(item.Key);

        public int Count
            => _d1.Count;

        public bool IsReadOnly
            => false;

        public bool ContainsKey(TKey key)
            => _d1.ContainsKey(key);

        public bool ContainsValue(TValue value)
            => _d2.ContainsKey(value);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            => GetEnumerator();

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
            => _d1.GetEnumerator();

        public bool Remove(TKey key)
        {
            if (!ContainsKey(key))
                return false;
            _d2.Remove(_d1[key]);
            _d1.Remove(key);
            return true;
        }

        public bool RemoveByValue(TValue value)
        {
            if (!ContainsValue(value))
                return false;
            return Remove(KeyFromValue(value));
        }

        public bool TryGetValue(TKey key, out TValue value)
            => _d1.TryGetValue(key, out value);

        public bool TryGetKey(TValue value, out TKey key)
            => _d2.TryGetValue(value, out key);

        public TValue this[TKey key]
        {
            get => _d1[key];
            set
            {
                _d1[key] = value;
                _d2[value] = key;
            }
        }

        public ICollection<TKey> Keys
            => _d1.Keys;

        public ICollection<TValue> Values
            => _d1.Values;

        public TKey KeyFromValue(TValue value)
            => _d2[value];

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
