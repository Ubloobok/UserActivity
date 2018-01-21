using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UserActivity.Viewer.Implements;

namespace System.Linq
{
    public static class CollectionExtensions
    {
        public static T AddItem<T>(this ICollection<T> collection, T item)
        {
            collection.Add(item);
            return item;
        }

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
        public static void Add<T>(this SelectableCollection<CollectionItem<T>> collection, T value, string displayValue)
        {
            var item = new CollectionItem<T>() { Value = value, DisplayValue = displayValue };
            collection.Add(item);
        }
    }
}
