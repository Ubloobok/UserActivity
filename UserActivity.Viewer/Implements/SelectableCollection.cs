using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UserActivity.Viewer.Implements
{
    /// <summary>
    /// Collection item.
    /// </summary>
    public class CollectionItem
    {
        /// <summary>
        /// Create new collection item.
        /// </summary>
        public static CollectionItem<T> New<T>(T value, string displayValue) =>
            new CollectionItem<T>() { Value = value, DisplayValue = displayValue };
    }

    /// <summary>
    /// Collection item.
    /// </summary>
    public class CollectionItem<T>
    {
        /// <summary>Display value.</summary>
        public string DisplayValue { get; set; }
        /// <summary>Value.</summary>
        public T Value { get; set; }
    }

    /// <summary>
    /// Collection for selectors controls.
    /// </summary>
    public class SelectableCollection<T> : ObservableCollection<T>
        where T : class
    {
        private T _selectedItem;

        /// <summary>Ctor.</summary>
        public SelectableCollection()
        {
        }

        /// <summary>Ctor.</summary>
        public SelectableCollection(params T[] items)
            : base(items ?? Enumerable.Empty<T>())
        {
        }

        /// <summary>Selected item.</summary>
        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// SelectedItem property was changed.
        /// </summary>
        public event EventHandler SelectedItemChanged;

        /// <summary>
        /// Select first element of the collection.
        /// </summary>
        public void SelectFirst() => SelectedItem = this.FirstOrDefault();
    }
}
