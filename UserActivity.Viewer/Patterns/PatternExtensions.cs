using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.Patterns
{
    public static class PatternExtensions
    {
        public static int[] DistinctMany(this IEnumerable<int[]> arrays)
        {
            int[] result = arrays
                .SelectMany(s => s)
                .Distinct()
                .OrderBy(c => c)
                .ToArray();
            return result;
        }

        public static int[][] Variations(this int[] items, int minLen, int maxLen)
        {
            List<int[]> variations = new List<int[]>();
            for (int len = minLen; len <= maxLen; len++)
            {
                var temp = Enumerable
                    .Range(0, len)
                    .Select(_ => items)
                    .CartesianProduct()
                    .Select(_ => _.ToArray())
                    .ToArray();
                variations.AddRange(temp);
            }
            return variations.ToArray();
        }

        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
                emptyProduct,
                (accumulator, sequence) =>
                    from accseq in accumulator
                    from item in sequence
                    select accseq.Concat(new[] { item })
                );
        }

        private static bool IsSubArrayEqual(int[] array, int[] subarray, int startIndex)
        {
            for (int i = 0; i < subarray.Length; i++)
            {
                if (array[startIndex++] != subarray[i]) return false;
            }
            return true;
        }

        public static int IndexOf(this int[] array, int[] subarray)
        {
            int max = 1 + array.Length - subarray.Length;
            for (int i = 0; i < max; i++)
            {
                if (IsSubArrayEqual(array, subarray, i))
                {
                    return i;
                }
            }
            return -1;
        }

        public static int Count(this int[] array, int[] subarray)
        {
            int max = array.Length - subarray.Length + 1;
            int count = 0;
            for (int i = 0; i < max; i++)
            {
                if (IsSubArrayEqual(array, subarray, i))
                {
                    count++;
                    i += subarray.Length - 1;
                }
            }
            return count;
        }
    }
}
