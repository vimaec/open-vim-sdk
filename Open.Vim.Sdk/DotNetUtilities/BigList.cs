using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Vim.DotNetUtilities
{
    // Useful for creating really big buffers of data. 
    public class BigList<T>
    {
        public const int MaxSize = 1000 * 1000;
        private List<List<T>> _lists = new List<List<T>> { new List<T>() };
        public T this[long n]
        {
            get => _lists[(int)(n / MaxSize)][(int)(n % MaxSize)];
            set => _lists[(int)(n / MaxSize)][(int)(n % MaxSize)] = value;
        }

        public long Count
            => _lists.Count - 1 * MaxSize + _curList.Count;

        private List<T> _curList
            => _lists[_lists.Count - 1];

        public void Add(T x)
        {
            if (_curList.Count >= MaxSize)
                _lists.Add(new List<T>());
            _curList.Add(x);
            Debug.Assert(_lists.Sum(list => list.Count) == Count);
        }
    }
}
