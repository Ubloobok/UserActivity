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
        const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Событий: {2}";
        string _loadedDataStatusString;
        string _filteredDataStatusString;

        /// <summary>Ctor.</summary>
        public ActivitiesDataGridVM()
        {
            Activities = new ObservableCollection<Activity>();
            DataStatusString = FormatDataStatusString(0, 0, 0);
            FilteredDataStatusString = FormatDataStatusString(0, 0, 0);
        }

        /// <summary>All data status string property.</summary>
        public string DataStatusString
        {
            get { return _loadedDataStatusString; }
            set { Set(ref _loadedDataStatusString, value); }
        }

        /// <summary>Filtered data status string property./summary>
        public string FilteredDataStatusString
        {
            get { return _filteredDataStatusString; }
            set { Set(ref _filteredDataStatusString, value); }
        }

        public void Initialize(RegionImageItemVM regionImage, IEnumerable<Activity> activities)
        {
            Header =
                regionImage == null ? "Список Действий" :
                regionImage.DisplayName + " (Список Действий)";

            Activities.Clear();
            Activities.AddRange(activities);
        }

        /// <summary>Loaded and filtered events.</summary>
        public ObservableCollection<Activity> Activities { get; private set; }

        private string FormatDataStatusString(int sessionGroupsCount, int sessionsCount, int eventsCount)
        {
            //var selectedRegion = RegionSelector.SelectedItem;
            //if (selectedRegion != null)
            //{
            //    int sessionGroupCount = SessionGroups.Where(sg => sg.Sessions.Any(s => s.ActivityCollection.Any(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName))).Count();
            //    int sessionCount = SessionGroups.Sum(sg => sg.Sessions.Where(s => s.ActivityCollection.Any(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName)).Count());
            //    int eventCount = SessionGroups.Sum(sg => sg.Sessions.Sum(s => s.ActivityCollection.Where(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName).Count()));
            //    FilteredDataStatusString = FormatDataStatusString(sessionGroupCount, sessionCount, eventCount);
            //}

            //int sessionGroupCount = SessionGroups.Count;
            //int sessionCount = SessionGroups.Sum(sg => sg.Sessions.Count);
            //int eventCount = SessionGroups.Sum(sg => sg.Sessions.Sum(a => a.ActivityCollection.Count));
            //DataStatusString = FormatDataStatusString(sessionGroupCount, sessionCount, eventCount);

            string str = string.Format(DataStatusStringFormat, sessionGroupsCount, sessionsCount, eventsCount);
            return str;
        }
    }
}
