using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.ViewModel.Items;
using UserActivity.Viewer.Extensions;
using GalaSoft.MvvmLight.Command;
using UserActivity.Viewer.Implements;
using UserActivity.Viewer.Services;

namespace UserActivity.Viewer.ViewModel
{
    /// <summary>
    /// Event list view model.
    /// </summary>
    public class EventListVM : ComponentVM
    {
        const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Событий: {2}";
        string _loadedDataInfo;
        string _filteredDataInfo;
        DataImportService _import = DataImportService.Create();

        /// <summary>Ctor.</summary>
        public EventListVM()
        {
            LoadedDataInfo = string.Format(DataStatusStringFormat, 0, 0, 0);
            FilteredDataInfo = string.Format(DataStatusStringFormat, 0, 0, 0);

            EventTypeSelector.Add(ActivityKind.Unknown, "Любой");
            EventTypeSelector.Add(ActivityKind.Click, "Клики Мыши");
            EventTypeSelector.Add(ActivityKind.Movement, "Движения Мыши");
            EventTypeSelector.SelectedItemChanged += OnSelectedEventTypeChanged;
        }

        /// <summary>Import file command.</summary>
        public RelayCommand ImportFileCommand => new RelayCommand(ExecuteImportFile);

        /// <summary>All loaded data.</summary>
        private List<SessionGroup> Files { get; set; } = new List<SessionGroup>();

        /// <summary>All loaded events.</summary>
        public ObservableCollection<Activity> Events { get; private set; } = new ObservableCollection<Activity>();

        /// <summary>Selected region.</summary>
        public SelectableCollection<RegionImageItemVM> RegionSelector { get; }
            = new SelectableCollection<RegionImageItemVM>();

        /// <summary>Selected event type.</summary>
        public SelectableCollection<CollectionItem<ActivityKind>> EventTypeSelector { get; }
            = new SelectableCollection<CollectionItem<ActivityKind>>();

        /// <summary>All data status string property.</summary>
        public string LoadedDataInfo
        {
            get { return _loadedDataInfo; }
            set { Set(ref _loadedDataInfo, value); }
        }

        /// <summary>Filtered data status string property./summary>
        public string FilteredDataInfo
        {
            get { return _filteredDataInfo; }
            set { Set(ref _filteredDataInfo, value); }
        }

        /// <summary>
        /// Import UAD-file(s), load available regions.
        /// </summary>
        private void ExecuteImportFile()
        {
            var groups = _import.ImportFile();
            Files.AddRange(groups);

            int fileCount = Files.Count;
            int sessionCount = Files.Sum(sg => sg.Sessions.Count);
            int eventCount = Files.Sum(sg => sg.Sessions.Sum(a => a.ActivityCollection.Count));
            LoadedDataInfo = string.Format(DataStatusStringFormat, fileCount, sessionCount, eventCount);

            EventTypeSelector.SelectedItem = EventTypeSelector.First();
        }

        /// <summary>
        /// On selected event type changed.
        /// </summary>
        private void OnSelectedEventTypeChanged(object sender, EventArgs e)
        {
            var type = EventTypeSelector.SelectedItem?.Value;
            var events = Files
                .SelectMany(g => g.Sessions
                    .SelectMany(s => s.ActivityCollection
                        .Where(a => type == ActivityKind.Unknown || a.Kind == type)));

            Events.Clear();
            Events.AddRange(events);

            var activities = Files
                .SelectMany(sg => sg.Sessions
                    .SelectMany(s => s.ActivityCollection
                        .Where(a => type == ActivityKind.Unknown || a.Kind == type)));

            int fileCount = Files
                .Where(sg => sg.Sessions
                    .Any(s => s.ActivityCollection
                        .Any(a => type == ActivityKind.Unknown || a.Kind == type)))
                .Count();
            int sessionCount = Files
                .Sum(sg => sg.Sessions
                    .Where(s => s.ActivityCollection
                        .Any(a => type == ActivityKind.Unknown || a.Kind == type))
                    .Count());
            int eventCount = Files
                .Sum(sg => sg.Sessions
                    .Sum(s => s.ActivityCollection
                        .Where(a => type == ActivityKind.Unknown || a.Kind == type)
                        .Count()));
            FilteredDataInfo = string.Format(DataStatusStringFormat, fileCount, sessionCount, eventCount);
        }

        /// <summary>
        /// Unload component and all resources.
        /// </summary>
        public override void Unload()
        {
            base.Unload();
            Files.Clear();
            Files = null;
            Events.Clear();
            Events = null;
        }
    }
}
