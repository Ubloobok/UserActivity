using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserActivity.Viewer.Implements;

namespace System.Linq
{
    /// <summary>
    /// LINQ and collections extensions.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Add item to collection.
        /// </summary>
        public static T AddItem<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return item;
        }

        /// <summary>
        /// Add range of items to collection.
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            if (range != null)
            {
                foreach (var item in range)
                {
                    collection.Add(item);
                }
            }
        }

        /// <summary>
        /// Add item to selectable collection.
        /// </summary>
        public static void Add<T>(this SelectableCollection<CollectionItem<T>> source, T value, string displayValue)
        {
            var item = new CollectionItem<T>() { Value = value, DisplayValue = displayValue };
            source.Add(item);
        }

        /// <summary>
        /// Distinct by delegate.
        /// </summary>
        public static IEnumerable<T> DistinctBy<T>(this IEnumerable<T> source, Func<T, T, bool> equals)
        {
            var comparer = new DelegateEqualityComparer<T>(equals);
            return source.Distinct(comparer);
        }
    }
}
