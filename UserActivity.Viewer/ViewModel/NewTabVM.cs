using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.ViewModel
{
    /// <summary>
    /// New tab view model.
    /// </summary>
    public class NewTabVM : ComponentVM
    {
        /// <summary>Ctor.</summary>
        public NewTabVM()
        {
            Header = "+";
        }

        /// <summary>Open heat map.</summary>
        public RelayCommand OpenHeatMapTabCommand { get; set; }

        /// <summary>Open event list.</summary>
        public RelayCommand OpenEventListTabCommand { get; set; }

        /// <summary>Open sequential pattern.</summary>
        public RelayCommand OpenSequentialPatternTabCommand { get; set; }
    }
}
