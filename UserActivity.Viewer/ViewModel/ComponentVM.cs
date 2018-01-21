using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.ViewModel
{
    /// <summary>
    /// Base class for components.
    /// </summary>
	public class ComponentVM : ViewModelBase
    {
        private string _header;
        private object _view;

        /// <summary>Header for tab.</summary>
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
                RaisePropertyChanged();
            }
        }

        /// <summary>View for view model.</summary>
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
                RaisePropertyChanged();
            }
        }

        /// <summary>Close command.</summary>
        public RelayCommand<ComponentVM> CloseCommand { get; set; }

        /// <summary>
        /// Unloads current component and all resources.
        /// </summary>
        public virtual void Unload()
        {
        }
    }
}
