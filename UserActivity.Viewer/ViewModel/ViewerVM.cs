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
        /// <summary>Ctor.</summary>
        public ViewerVM()
        {
            var newTab = CreateComponent<NewTabVM, NewTabView>();
            newTab.OpenEventListTabCommand = OpenActivitiesDataGridCommand;
            newTab.OpenHeatMapTabCommand = OpenHeatMapTabCommand;
            newTab.OpenSequentialPatternTabCommand = OpenSequentialPatternCommand;
            OpenComponent(newTab);
        }

        public RelayCommand OpenActivitiesDataGridCommand => new RelayCommand(ExecuteOpenActivitiesDataGrid);
        public RelayCommand OpenHeatMapTabCommand => new RelayCommand(ExecuteOpenHeatMapTab);
        public RelayCommand OpenSequentialPatternCommand => new RelayCommand(ExecuteOpenSequentialPattern);
        public RelayCommand<ComponentVM> CloseComponentCommand => new RelayCommand<ComponentVM>(ExecuteCloseComponent);

        private List<SessionGroup> SessionGroups => new List<SessionGroup>();
        public SelectableCollection<RegionImageItemVM> RegionSelector { get; private set; }
        public SelectableCollection<ComponentVM> Components { get; } = new SelectableCollection<ComponentVM>();

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
                foreach (var region in SessionGroups.SelectMany(sg => sg.Sessions).SelectMany(s => s.Regions))
                {
                    foreach (var image in region.Variations)
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
        }

        /// <summary>
        /// Create new component, fill default commands and view properties.
        /// </summary>
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

        /// <summary>
        /// Open initialized component, add to the components collection.
        /// </summary>
        private void OpenComponent(ComponentVM vm)
        {
            var index = Math.Max(0, Components.Count - 1);
            Components.Insert(index, vm);
            Components.SelectedItem = Components[index];
        }

        private void ExecuteOpenHeatMapTab()
        {
            var vm = CreateComponent<HeatMapVM, HeatMapView>();
            OpenComponent(vm);
        }

        private void ExecuteOpenSequentialPattern()
        {
            var vm = CreateComponent<SequentialPatternVM, SequentialPatternView>();
            OpenComponent(vm);
        }

        private void ExecuteOpenActivitiesDataGrid()
        {
            var vm = CreateComponent<EventListVM, EventListView>();
            OpenComponent(vm);
        }

        /// <summary>
        /// Close component, remove from collection and unload.
        /// </summary>
        private void ExecuteCloseComponent(ComponentVM component)
        {
            if ((component != null) && Components.Contains(component))
            {
                Components.Remove(component);
                component.Unload();
            }
        }
    }
}