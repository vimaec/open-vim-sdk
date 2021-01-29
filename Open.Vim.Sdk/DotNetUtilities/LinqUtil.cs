using System;
using System.Collections.Generic;
using System.Linq;

namespace Vim.DotNetUtilities
{
    public static class LinqUtil
    {
        public static T Maximize<T>(this IEnumerable<T> self, Func<T, T, bool> f)
            => self.Any() ? self.Skip(1).Aggregate(self.FirstOrDefault(), (a, b) => f(a, b) ? a : b) : default;

        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> items, Func<T, int> counter, int maxCount)
        {
            List<List<T>> batches = new List<List<T>>();
            List<T> currentBatch = new List<T>();
            int count = 0;
            foreach (var item in items)
            {
                currentBatch.Add(item);
                count += counter(item);

                if (count > maxCount)
                {
                    batches.Add(currentBatch);
                    currentBatch = new List<T>();
                    count = 0;
                }
            }

            if (currentBatch.Count > 0)
                batches.Add(currentBatch);

            return batches;
        }
    }
}
