using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UserActivity.CL.WPF.Entities;
using UserActivity.CL.WPF.Services;
using UserActivity.Viewer.Implements;
using UserActivity.Viewer.View;
using UserActivity.Viewer.ViewModel.Items;

namespace UserActivity.Viewer.ViewModel
{
    public class HeatMapVM : ComponentVM
    {
        const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Событий: {2}";

        double _heatMapOpacity;
        int _pointGradientRadius;
        int _pointOverlapRadius;
        string _loadedDataInfo;
        string _filteredDataInfo;

        /// <summary>Ctor.</summary>
        public HeatMapVM()
        {
            Header = "Тепловая Карта";
            HeatMapOpacity = 0.75;
            PointGradientRadius = 40;
            PointOverlapRadius = 10;

            RegionSelector.SelectedItemChanged += OnSelectedRegionChanged;
            EventTypeSelector.Add(ActivityKind.Click, "Клики Мыши");
            EventTypeSelector.Add(ActivityKind.Movement, "Движения Мыши");
            EventTypeSelector.SelectedItemChanged += OnSelectedEventTypeChanged;
        }

        private void OnSelectedRegionChanged(object sender, EventArgs e)
        {
            var region = RegionSelector.SelectedItem;
            if (region != null)
            {
                ((HeatMapView)View).SetRegion(region);
            }
            EventTypeSelector.SelectedItem = EventTypeSelector.First();
        }

        private void OnSelectedEventTypeChanged(object sender, EventArgs e)
        {
            var region = RegionSelector.SelectedItem;
            var type = EventTypeSelector.SelectedItem?.Value;
            if ((region != null) && (type != ActivityKind.Unknown))
            {
                var activities = SessionGroups
                    .SelectMany(sg => sg.Sessions
                        .SelectMany(s => s.ActivityCollection
                            .Where(a => a.RegionName == region.RegionName
                                && a.ImageName == region.ImageName
                                && a.Kind == type)));
                ((HeatMapView)View).SetActivities(activities);

                int fileCount = SessionGroups
                    .Where(sg => sg.Sessions
                        .Any(s => s.ActivityCollection
                            .Any(a => a.RegionName == region.RegionName && a.ImageName == region.ImageName
                                && a.Kind == type)))
                    .Count();
                int sessionCount = SessionGroups
                    .Sum(sg => sg.Sessions
                        .Where(s => s.ActivityCollection
                            .Any(a => a.RegionName == region.RegionName && a.ImageName == region.ImageName
                                && a.Kind == type))
                        .Count());
                int eventCount = SessionGroups
                    .Sum(sg => sg.Sessions
                        .Sum(s => s.ActivityCollection
                            .Where(a => a.RegionName == region.RegionName && a.ImageName == region.ImageName
                                && a.Kind == type)
                            .Count()));
                FilteredDataInfo = string.Format(DataStatusStringFormat, fileCount, sessionCount, eventCount);
            }
            else
            {
                FilteredDataInfo = string.Format(DataStatusStringFormat, 0, 0, 0);
            }
        }

        public RelayCommand ImportFileCommand => new RelayCommand(ExecuteImportFile);

        private List<SessionGroup> SessionGroups { get; } = new List<SessionGroup>();

        public SelectableCollection<RegionImageItemVM> RegionSelector { get; } = new SelectableCollection<RegionImageItemVM>();

        public SelectableCollection<CollectionItem<ActivityKind>> EventTypeSelector { get; } = new SelectableCollection<CollectionItem<ActivityKind>>();

        /// <summary>Heatmap opacity.</summary>
        public double HeatMapOpacity
        {
            get { return _heatMapOpacity; }
            set { Set(ref _heatMapOpacity, value); OnVisualValueChanged(); }
        }

        /// <summary>Point gradient radius for display.</summary>
        public int PointGradientRadius
        {
            get { return _pointGradientRadius; }
            set { Set(ref _pointGradientRadius, value); OnVisualValueChanged(); }
        }

        /// <summary>Point overlap radius for display.</summary>
        public int PointOverlapRadius
        {
            get { return _pointOverlapRadius; }
            set { Set(ref _pointOverlapRadius, value); OnVisualValueChanged(); }
        }

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
            var openFileDialog = new OpenFileDialog()
            {
                Filter = string.Format("UAD-файлы (*.{0})|*.{0}", XmlUserActivityDataContext.UadFileExtension),
                Multiselect = true,
            };
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                foreach (var stream in openFileDialog.OpenFiles())
                {
                    using (stream)
                    {
                        var sessionGroup = XmlUserActivityDataContext.LoadSessionGroup(stream);
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
            }

            int fileCount = SessionGroups.Count;
            int sessionCount = SessionGroups.Sum(sg => sg.Sessions.Count);
            int eventCount = SessionGroups.Sum(sg => sg.Sessions.Sum(a => a.ActivityCollection.Count));
            LoadedDataInfo = string.Format(DataStatusStringFormat, fileCount, sessionCount, eventCount);
        }

        /// <summary>
        /// Raises view refresh.
        /// </summary>
        private void OnVisualValueChanged()
        {
            (View as HeatMapView)?.Refresh();
        }

        /// <summary>
        /// Unload current component and all resources.
        /// </summary>
        public override void Unload()
        {
            base.Unload();
            (View as HeatMapView)?.Unload();
        }
    }
}
