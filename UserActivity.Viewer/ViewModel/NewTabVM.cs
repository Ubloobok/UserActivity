using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.ViewModel
{
    public class NewTabVM : ComponentVM
    {
        public NewTabVM()
        {
            Header = "+";
        }

        public RelayCommand OpenHeatMapTabCommand { get; set; }
        public RelayCommand OpenEventListTabCommand { get; set; }
        public RelayCommand OpenSequentialPatternTabCommand { get; set; }
    }
}
