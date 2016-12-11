using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UserActivity.Viewer.Implements
{
	public class CollectionItem<T>
	{
		public string DisplayValue { get; set; }
		public T Value { get; set; }
	}

	public class SelectableCollection<T> : ObservableCollection<T>
		where T : class
	{
		private T _selectedItem;

		public SelectableCollection()
		{
		}

		public SelectableCollection(params T[] items)
			: base(items ?? Enumerable.Empty<T>())
		{
		}

		/// <summary>
		/// Gets or sets selected item.
		/// </summary>
		public T SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				_selectedItem = value;
				OnPropertyChanged(new PropertyChangedEventArgs("SelectedItem"));
				var selectedItemChanged = SelectedItemChanged;
				if (selectedItemChanged != null)
				{
					selectedItemChanged(this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler SelectedItemChanged;
	}
}
