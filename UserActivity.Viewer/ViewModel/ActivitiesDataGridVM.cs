using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.ViewModel.Items;
using UserActivity.Viewer.Extensions;

namespace UserActivity.Viewer.ViewModel
{
    public class ActivitiesDataGridVM : ComponentVM
    {
        public ActivitiesDataGridVM()
        {
            Activities = new ObservableCollection<Activity>();
        }

        public void Initialize(RegionImageItemVM regionImage, IEnumerable<Activity> activities)
        {
            Header =
                regionImage == null ? "Список Действий" :
                regionImage.DisplayName + " (Список Действий)";

            Activities.Clear();
            Activities.AddRange(activities);
        }

        public ObservableCollection<Activity> Activities
        {
            get;
            private set;
        }
    }
}
