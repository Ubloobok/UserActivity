using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.Implements;
using UserActivity.Viewer.View;
using UserActivity.Viewer.ViewModel.Items;
using UserActivity.Viewer.Extensions;
using UserActivity.CL.WPF.Services;
using System.Windows;

namespace UserActivity.Viewer.ViewModel
{
    public class ViewerVM : ViewModelBase
    {
        private const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Действий: {2}";

        private string _loadedDataStatusString;
        private string _filteredDataStatusString;

        public ViewerVM()
        {
            RegionSelector = new SelectableCollection<RegionImageItemVM>();
            RegionSelector.SelectedItemChanged += OnRegionSelectorSelectedItemChanged;
            Components = new SelectableCollection<ComponentVM>();

            DataStatusString = FormatDataStatusString(0, 0, 0);
            FilteredDataStatusString = FormatDataStatusString(0, 0, 0);
        }

        private void OnRegionSelectorSelectedItemChanged(object sender, EventArgs e)
        {
            var selectedRegion = RegionSelector.SelectedItem;
            if (selectedRegion != null)
            {
                int sessionGroupCount = SessionGroups.Where(sg => sg.Sessions.Any(s => s.ActivityCollection.Any(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName))).Count();
                int sessionCount = SessionGroups.Sum(sg => sg.Sessions.Where(s => s.ActivityCollection.Any(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName)).Count());
                int eventCount = SessionGroups.Sum(sg => sg.Sessions.Sum(s => s.ActivityCollection.Where(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName).Count()));
                FilteredDataStatusString = FormatDataStatusString(sessionGroupCount, sessionCount, eventCount);
            }

            OpenClickHeatMapTabCommand.RaiseCanExecuteChanged();
            OpenMovementHeatMapTabCommand.RaiseCanExecuteChanged();
            OpenActivitiesDataGridCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Sets or gets the all data status string property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string DataStatusString
        {
            get { return _loadedDataStatusString; }
            set
            {
                if (_loadedDataStatusString == value)
                {
                    return;
                }
                _loadedDataStatusString = value;
                RaisePropertyChanged(() => DataStatusString);
            }
        }

        /// <summary>
        /// Sets or gets the filtered data status string property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string FilteredDataStatusString
        {
            get { return _filteredDataStatusString; }
            set
            {
                if (_filteredDataStatusString == value)
                {
                    return;
                }
                _filteredDataStatusString = value;
                RaisePropertyChanged(() => FilteredDataStatusString);
            }
        }

        public RelayCommand ImportDataFromFileCommand => new RelayCommand(ExecuteImportDataFromFile);
        public RelayCommand OpenClickHeatMapTabCommand => new RelayCommand(ExecuteOpenClickHeatMapTab, IsRegionSelected);
        public RelayCommand OpenMovementHeatMapTabCommand => new RelayCommand(ExecuteOpenMovementHeatMapTab, IsRegionSelected);
        public RelayCommand OpenTestHeatMapTabCommand => new RelayCommand(ExecuteOpenTestHeatMapTab);
        public RelayCommand OpenActivitiesDataGridCommand => new RelayCommand(ExecuteOpenActivitiesDataGrid, IsRegionSelected);
        public RelayCommand OpenSequentialPatternCommand => new RelayCommand(ExecuteOpenSequentialPattern);
        public RelayCommand<ComponentVM> CloseComponentCommand => new RelayCommand<ComponentVM>(ExecuteCloseComponent);

        private List<SessionGroup> SessionGroups => new List<SessionGroup>();
        public SelectableCollection<RegionImageItemVM> RegionSelector
        {
            get;
            private set;
        }
        public SelectableCollection<ComponentVM> Components { get; private set; }

        private bool IsRegionSelected()
        {
            bool canExecute = RegionSelector.SelectedItem != null;
            return canExecute;
        }

        private bool IsComponentNotNull(ComponentVM component)
        {
            bool canExecute = component != null;
            return canExecute;
        }

        private void ExecuteImportDataFromFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("UAD-файлы (*.{0})|*.{0}", XmlUserActivityDataContext.UadFileExtension),
                Multiselect = true,
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                var serializer = new XmlSerializer(typeof(SessionGroup));
                foreach (var stream in openFileDialog.OpenFiles())
                {
                    using (stream)
                    {
                        SessionGroup sessionGroup = XmlUserActivityDataContext.LoadSessionGroup(stream);
                        SessionGroups.Add(sessionGroup);
                    }
                }

                var newRegions = new List<RegionImageItemVM>();
                foreach (var region in SessionGroups.SelectMany(sg => sg.Sessions).SelectMany(s => s.RegionCollection))
                {
                    foreach (var image in region.Images)
                    {
                        if (newRegions.FirstOrDefault(r => r.RegionName == region.Name && r.ImageName == image.Name) == null)
                        {
                            var newRegion = new RegionImageItemVM() { RegionName = region.Name, Image = image };
                            newRegions.Add(newRegion);
                        }
                    }
                }
                RegionSelector.Clear();
                RegionSelector.AddRange(newRegions.OrderBy(r => r.RegionName));
                RegionSelector.SelectedItem = RegionSelector.FirstOrDefault();

                int sessionGroupCount = SessionGroups.Count;
                int sessionCount = SessionGroups.Sum(sg => sg.Sessions.Count);
                int eventCount = SessionGroups.Sum(sg => sg.Sessions.Sum(a => a.ActivityCollection.Count));
                DataStatusString = FormatDataStatusString(sessionGroupCount, sessionCount, eventCount);
            }
        }

        private TViewModel CreateComponent<TViewModel, TView>()
            where TViewModel : ComponentVM, new()
            where TView : FrameworkElement, new()
        {
            var vm = new TViewModel();
            var v = new TView();
            vm.CloseCommand = CloseComponentCommand;
            v.DataContext = vm;
            vm.View = v;
            return vm;
        }

        private void OpenComponent(ComponentVM vm)
        {
            Components.Add(vm);
            Components.SelectedItem = Components.Last();
        }

        private void ExecuteOpenClickHeatMapTab()
        {
            OpenHeatMapTab(ActivityKind.Click, RegionSelector.SelectedItem);
        }

        private void ExecuteOpenMovementHeatMapTab()
        {
            OpenHeatMapTab(ActivityKind.Movement, RegionSelector.SelectedItem);
        }

        private void ExecuteOpenTestHeatMapTab()
        {
            OpenHeatMapTab(ActivityKind.Unknown, null);
        }

        private void OpenHeatMapTab(ActivityKind type, RegionImageItemVM region)
        {
            var vm = CreateComponent<HeatMapVM, HeatMapView>();

            vm.Initialize(type, region);
            if (region != null)
            {
                ((HeatMapView)vm.View).SetRegion(region);
            }
            if (type != ActivityKind.Unknown)
            {
                var activities = SessionGroups
                    .SelectMany(sg => sg.Sessions
                        .SelectMany(s => s.ActivityCollection
                            .Where(a => a.RegionName == region.RegionName
                                && a.ImageName == region.ImageName
                                && a.Kind == type)));
                ((HeatMapView)vm.View).SetActivities(activities);
            }

            OpenComponent(vm);
        }

        private void ExecuteOpenSequentialPattern()
        {
            var vm = CreateComponent<SequentialPatternVM, SequentialPatternView>();

            var activities = SessionGroups.SelectMany(sg => sg.Sessions.SelectMany(s => s.ActivityCollection));
            vm.Initialize(activities);

            OpenComponent(vm);
        }

        private void ExecuteOpenActivitiesDataGrid()
        {
            var vm = CreateComponent<ActivitiesDataGridVM, ActivitiesDataGridView>();

            var region = RegionSelector.SelectedItem;
            var activities = SessionGroups
                .SelectMany(sg => sg.Sessions
                    .SelectMany(s => s.ActivityCollection
                        .Where(a => a.RegionName == region.RegionName
                            && a.ImageName == region.ImageName)));
            vm.Initialize(region, activities);

            OpenComponent(vm);
        }

        private void ExecuteCloseComponent(ComponentVM component)
        {
            if ((component != null) && Components.Contains(component))
            {
                Components.Remove(component);
                component.Unload();
            }
        }
        
        private string FormatDataStatusString(int sessionGroupsCount, int sessionsCount, int eventsCount)
        {
            string str = string.Format(DataStatusStringFormat, sessionGroupsCount, sessionsCount, eventsCount);
            return str;
        }
    }
}