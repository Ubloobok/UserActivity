using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.View;
using UserActivity.Viewer.ViewModel.Items;

namespace UserActivity.Viewer.ViewModel
{
	public class HeatMapVM : ComponentVM
	{
		private double _heatMapOpacity;
		private int _pointGradientRadius;
		private int _pointOverlapRadius;

		public HeatMapVM()
		{
		}

		public void Initialize(ActivityKind activityType, RegionImageItemVM regionImage)
		{
			ActivityType = activityType;
			RegionImage = regionImage;
			Header =
				regionImage == null ? "Тепловая Карта" :
				regionImage.DisplayName + (
				activityType == ActivityKind.Click ? " (Клики)" :
				activityType == ActivityKind.Movement ? " (Движения)" :
				"Тепловая Карта");
			HeatMapOpacity = 0.75;
			PointGradientRadius = 40;
			PointOverlapRadius = 10;
		}

		/// <summary>
		/// Gets or sets the Activity Type property.
		/// </summary>
		public ActivityKind ActivityType { get; private set; }

		/// <summary>
		/// Gets or sets current region and image.
		/// </summary>
		public RegionImageItemVM RegionImage { get; set; }

		/// <summary>
		/// Gets or sets the heatmap opacity property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public double HeatMapOpacity
		{
			get { return _heatMapOpacity; }
			set
			{
				if (_heatMapOpacity == value)
				{
					return;
				}
				_heatMapOpacity = value;
				RaisePropertyChanged(() => HeatMapOpacity);
				
			}
		}

		/// <summary>
		/// Gets or sets the point display radius property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public int PointGradientRadius
		{
			get { return _pointGradientRadius; }
			set
			{
				if (_pointGradientRadius == value)
				{
					return;
				}
				_pointGradientRadius = value;
				RaisePropertyChanged(() => PointGradientRadius);
				OnVisualValueChanged();
				PointOverlapRadius = value / 4;
			}
		}

		/// <summary>
		/// Gets or sets the point group radius property.
		/// Changes to that property's value raise the PropertyChanged event. 
		/// </summary>
		public int PointOverlapRadius
		{
			get { return _pointOverlapRadius; }
			set
			{
				if (_pointOverlapRadius == value)
				{
					return;
				}
				_pointOverlapRadius = value;
				RaisePropertyChanged(() => PointOverlapRadius);
				OnVisualValueChanged();
			}
		}

		/// <summary>
		/// Raises view refresh.
		/// </summary>
		private void OnVisualValueChanged()
		{
			if ((View != null) && (View is HeatMapView))
			{
				((HeatMapView)View).Refresh();
			}
		}

		/// <summary>
		/// Unloads current component and all resources.
		/// </summary>
		public override void Unload()
		{
			base.Unload();
			if ((View != null) && (View is HeatMapView))
			{
				((HeatMapView)View).Unload();
			}
		}
	}
}
