using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vim.DotNetUtilities
{
    /// <summary>
    /// Stores values in an internal list, and supports look-up using multiple keys.
    /// </summary>
    public interface IMultiKeyDictionary<TKey1, TKey2, TValue> : IEnumerable<(TKey1, TKey2, TValue)>
    {
        int Count { get; }
        (TKey1, TKey2, TValue) this[TKey1 key] { get; }
        (TKey1, TKey2, TValue) this[TKey2 key] { get; }
        (TKey1, TKey2, TValue) At(int element);
        bool ContainsKey(TKey1 key);
        bool ContainsKey(TKey2 key);
        int IndexOf(TKey1 key);
        int IndexOf(TKey2 key);
        int Add(TKey1 key1, TKey2 key2, TValue value);
    }

    /// <summary>
    /// Stores values in an internal list, and supports look-up using multiple keys.
    /// </summary>
    public class MultiKeyDictionary<TKey1, TKey2, TValue> 
        : IMultiKeyDictionary<TKey1, TKey2, TValue>
    {
        public int Add(TKey1 key1, TKey2 key2, TValue value)
        {
            var n = _values.Count;
            _values.Add(value);
            _keys1.Add(key1, n);
            _keys2.Add(key2, n);
            return n;
        }

        private readonly IList<TValue> _values = new List<TValue>();
        private readonly BiDictionary<TKey1, int> _keys1 = new BiDictionary<TKey1,int>();
        private readonly BiDictionary<TKey2, int> _keys2 = new BiDictionary<TKey2, int>();

        public bool ContainsKey(TKey1 key)
            => _keys1.ContainsKey(key);

        public bool ContainsKey(TKey2 key)
            => _keys2.ContainsKey(key);

        public int IndexOf(TKey1 key)
            => _keys1.ContainsKey(key) ? _keys1[key] : -1;

        public int IndexOf(TKey2 key)
            => _keys2.ContainsKey(key) ? _keys2[key] : -1;

        public (TKey1, TKey2, TValue) At(int n)
            => (_keys1.KeyFromValue(n), _keys2.KeyFromValue(n), _values[n]);

        public (TKey1, TKey2, TValue) this[TKey1 key] 
            => At(IndexOf(key));

        public (TKey1, TKey2, TValue) this[TKey2 key]
            => At(IndexOf(key));

        public int Count
            => _values.Count;

        public IEnumerable<(TKey1, TKey2, TValue)> KeysAndValues()
            => Enumerable.Range(0, _values.Count).Select(At);

        public IEnumerator<(TKey1, TKey2, TValue)> GetEnumerator()
            => KeysAndValues().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => KeysAndValues().GetEnumerator();
    }
}
