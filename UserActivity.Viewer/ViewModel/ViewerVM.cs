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

namespace UserActivity.Viewer.ViewModel
{
	public class ViewerVM : ViewModelBase
	{
		private const string DataStatusStringFormat = "Файлов: {0}, Сессий: {1}, Действий: {2}";

		private string _loadedDataStatusString;
		private string _filteredDataStatusString;

		public ViewerVM()
		{
			ImportDataFromFileCommand = new RelayCommand(ExecuteLoadDataFromFileCommand);
			OpenClickHeatMapTabCommand = new RelayCommand(ExecuteOpenClickHeatMapTabCommand, CanExecuteIfRegionSelected);
			OpenMovementHeatMapTabCommand = new RelayCommand(ExecuteOpenMovementHeatMapTabCommand, CanExecuteIfRegionSelected);
			OpenTestHeatMapTabCommand = new RelayCommand(ExecuteOpenTestHeatMapTabCommand);
			OpenActivitiesDataGridCommand = new RelayCommand(ExecuteOpenActivitiesDataGridCommand, CanExecuteIfRegionSelected);
			CloseComponentCommand = new RelayCommand<ComponentVM>(ExecuteCloseComponentCommand, CanExecuteIfComponentNotNull);

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

		public RelayCommand ImportDataFromFileCommand { get; private set; }
		public RelayCommand OpenClickHeatMapTabCommand { get; private set; }
		public RelayCommand OpenMovementHeatMapTabCommand { get; private set; }
		public RelayCommand OpenTestHeatMapTabCommand { get; private set; }
		public RelayCommand OpenActivitiesDataGridCommand { get; private set; }
		public RelayCommand<ComponentVM> CloseComponentCommand { get; private set; }

		private List<SessionGroup> SessionGroups = new List<SessionGroup>();

		public SelectableCollection<RegionImageItemVM> RegionSelector
		{
			get;
			private set;
		}

		public SelectableCollection<ComponentVM> Components
		{
			get;
			private set;
		}

		private bool CanExecuteIfRegionSelected()
		{
			bool canExecute = RegionSelector.SelectedItem != null;
			return canExecute;
		}

		private bool CanExecuteIfComponentNotNull(ComponentVM component)
		{
			bool canExecute = component != null;
			return canExecute;
		}

		private void ExecuteLoadDataFromFileCommand()
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

		private void ExecuteOpenClickHeatMapTabCommand()
		{
			var selectedRegion = RegionSelector.SelectedItem;
			if (selectedRegion != null)
			{
				ExecuteOpenHeatMapTabCommand(ActivityKind.Click, selectedRegion);
			}
		}

		private void ExecuteOpenMovementHeatMapTabCommand()
		{
			var selectedRegion = RegionSelector.SelectedItem;
			if (selectedRegion != null)
			{
				ExecuteOpenHeatMapTabCommand(ActivityKind.Movement, selectedRegion);
			}
		}

		private void ExecuteOpenTestHeatMapTabCommand()
		{
			ExecuteOpenHeatMapTabCommand(ActivityKind.Unknown, null);
		}

		private void ExecuteOpenHeatMapTabCommand(ActivityKind type, RegionImageItemVM region)
		{
			var vm = new HeatMapVM();
			vm.Initialize(type, region);
			vm.CloseCommand = CloseComponentCommand;
			var v = new HeatMapView();
			v.ViewModel = vm;
			vm.View = v;

			if (region != null)
			{
				v.SetRegion(region);
			}

			if (type != ActivityKind.Unknown)
			{
				var activities = SessionGroups
					.SelectMany(sg => sg.Sessions
						.SelectMany(s => s.ActivityCollection
							.Where(a => a.RegionName == region.RegionName
								&& a.ImageName == region.ImageName
								&& a.Kind == type)));
				v.SetActivities(activities);
			}

			Components.Add(vm);
			Components.SelectedItem = Components.Last();
		}

		private void ExecuteCloseComponentCommand(ComponentVM component)
		{
			if ((component != null) && Components.Contains(component))
			{
				CloseComponent(component);
			}
		}

		private void ExecuteOpenActivitiesDataGridCommand()
		{
			var vm = new ActivitiesDataGridVM();
			vm.CloseCommand = CloseComponentCommand;
			var v = new ActivitiesDataGridView();
			v.ViewModel = vm;
			vm.View = v;

			IEnumerable<Activity> activities = null;
			var selectedRegion = RegionSelector.SelectedItem;
			if (selectedRegion != null)
			{
				activities = SessionGroups.SelectMany(sg => sg.Sessions.SelectMany(s => s.ActivityCollection.Where(a => a.RegionName == selectedRegion.RegionName && a.ImageName == selectedRegion.ImageName)));
			}
			vm.Initialize(selectedRegion, activities);

			Components.Add(vm);
			Components.SelectedItem = Components.Last();
		}

		private void CloseComponent(ComponentVM component)
		{
			Components.Remove(component);
			component.Unload();
		}

		private string FormatDataStatusString(int sessionGroupsCount, int sessionsCount, int eventsCount)
		{
			string str = string.Format(DataStatusStringFormat, sessionGroupsCount, sessionsCount, eventsCount);
			return str;
		}
	}
}