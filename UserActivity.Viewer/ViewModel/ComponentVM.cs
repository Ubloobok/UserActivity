using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.ViewModel
{
	public class ComponentVM : ViewModelBase
	{
		private string _header;
		private object _view;

		/// <summary>
		/// Sets or gets the header property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public string Header
		{
			get { return _header; }
			set
			{
				if (_header == value)
				{
					return;
				}
				_header = value;
				RaisePropertyChanged(() => Header);
			}
		}

		/// <summary>
		/// Sets or gets the view property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public object View
		{
			get { return _view; }
			set
			{
				if (_view == value)
				{
					return;
				}
				_view = value;
				RaisePropertyChanged(() => View);
			}
		}

		/// <summary>
		/// Gets or sets close command;
		/// </summary>
		public RelayCommand<ComponentVM> CloseCommand { get; set; }

		/// <summary>
		/// Unloads current component and all resources.
		/// </summary>
		public virtual void Unload()
		{
		}
	}
}
