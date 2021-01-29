using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Vim.DotNetUtilities
{
    // Useful for creating really big buffers of data. 
    public class BigArray<T>
    {
        public const int MaxSize = 1000 * 1000;
        private List<T[]> _lists = new List<T[]>();

        public T this[long n]
        {
            get => _lists[(int)(n / MaxSize)][(int)(n % MaxSize)];
            set => _lists[(int)(n / MaxSize)][(int)(n % MaxSize)] = value;
        }

        public long Count { get; }

        public BigArray(long n)
        {
            Count = n;
            while (n >= MaxSize)
            {
                _lists.Add(new T[MaxSize]);
                n -= MaxSize;
            }
            if (n > 0)
                _lists.Add(new T[n]);
            Debug.Assert(_lists.Sum(x => x.Length) == n);
        }
    }
}
