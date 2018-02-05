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
using UserActivity.Viewer.Services;
using UserActivity.Viewer.View;
using UserActivity.Viewer.ViewModel.Items;

namespace UserActivity.Viewer.ViewModel
{
    /// <summary>
    /// Heat map view model.
    /// </summary>
    public class HeatMapVM : ComponentVM
    {
        const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Событий: {2}";

        double _heatMapOpacity;
        int _pointGradientRadius;
        int _pointOverlapRadius;
        string _loadedDataInfo;
        string _filteredDataInfo;
        DataImportService _import = DataImportService.Create();

        /// <summary>Ctor.</summary>
        public HeatMapVM()
        {
            Header = "Тепловая Карта";
            LoadedDataInfo = string.Format(DataStatusStringFormat, 0, 0, 0);
            FilteredDataInfo = string.Format(DataStatusStringFormat, 0, 0, 0);
            HeatMapOpacity = 0.75;
            PointGradientRadius = 40;
            PointOverlapRadius = 10;

            RegionSelector.SelectedItemChanged += OnSelectedRegionChanged;
            EventTypeSelector.Add(ActivityKind.Click, "Клики Мыши");
            EventTypeSelector.Add(ActivityKind.Movement, "Движения Мыши");
            EventTypeSelector.SelectedItemChanged += OnSelectedEventTypeChanged;
        }

        /// <summary>
        /// On selected region changed.
        /// </summary>
        private void OnSelectedRegionChanged(object sender, EventArgs e)
        {
            var region = RegionSelector.SelectedItem;
            if (region != null)
            {
                ((HeatMapView)View).SetRegion(region);
            }
            EventTypeSelector.SelectFirst();
        }

        /// <summary>
        /// On selected event type changed.
        /// </summary>
        private void OnSelectedEventTypeChanged(object sender, EventArgs e)
        {
            var region = RegionSelector.SelectedItem;
            var type = EventTypeSelector.SelectedItem?.Value;
            if ((region != null) && (type != ActivityKind.Unknown))
            {
                var activities = Files
                    .SelectMany(sg => sg.Sessions
                        .SelectMany(s => s.ActivityCollection
                            .Where(a => a.RegionName == region.RegionName
                                && a.ImageName == region.ImageName
                                && a.Kind == type)));
                ((HeatMapView)View).SetActivities(activities);

                int fileCount = Files
                    .Where(sg => sg.Sessions
                        .Any(s => s.ActivityCollection
                            .Any(a => a.RegionName == region.RegionName && a.ImageName == region.ImageName
                                && a.Kind == type)))
                    .Count();
                int sessionCount = Files
                    .Sum(sg => sg.Sessions
                        .Where(s => s.ActivityCollection
                            .Any(a => a.RegionName == region.RegionName && a.ImageName == region.ImageName
                                && a.Kind == type))
                        .Count());
                int eventCount = Files
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

        /// <summary>Import file command.</summary>
        public RelayCommand ImportFileCommand => new RelayCommand(ExecuteImportFile);

        /// <summary>All loaded data.</summary>
        private List<SessionGroup> Files { get; } = new List<SessionGroup>();

        /// <summary>Selected region.</summary>
        public SelectableCollection<RegionImageItemVM> RegionSelector { get; } =
            new SelectableCollection<RegionImageItemVM>();

        /// <summary>Selected event type.</summary>
        public SelectableCollection<CollectionItem<ActivityKind>> EventTypeSelector { get; }
            = new SelectableCollection<CollectionItem<ActivityKind>>();

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
            var groups = _import.ImportFile();
            Files.AddRange(groups);

            var newRegions = new List<RegionImageItemVM>();
            foreach (var region in Files.SelectMany(sg => sg.Sessions).SelectMany(s => s.RegionCollection))
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

            //var regions = Files.SelectMany(sg => sg.Sessions)
            //    .SelectMany(s => s.RegionCollection)
            //    .SelectMany(r => r.Images.Select(v => new { r, v }))
            //    .DistinctBy((r1, r2) => r1.r.Name == r2.r.Name && r1.v.Name == r2.v.Name);

            RegionSelector.Clear();
            RegionSelector.AddRange(newRegions.OrderBy(r => r.RegionName));
            RegionSelector.SelectFirst();

            int fileCount = Files.Count;
            int sessionCount = Files.Sum(sg => sg.Sessions.Count);
            int eventCount = Files.Sum(sg => sg.Sessions.Sum(a => a.ActivityCollection.Count));
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
