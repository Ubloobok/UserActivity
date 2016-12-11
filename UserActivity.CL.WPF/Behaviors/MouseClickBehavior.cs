using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UserActivity.CL.WPF.Entities;
using UserActivity.CL.WPF.Services;
using UserActivity.CL.WPF.Extensions;
using System.IO;

namespace UserActivity.CL.WPF.Behaviors
{
	public class MouseClickBehavior :BehaviorBase
	{
		public static bool GetIsEnabled(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsEnabledProperty);
		}

		public static void SetIsEnabled(DependencyObject obj, bool value)
		{
			obj.SetValue(IsEnabledProperty, value);
		}

		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MouseClickBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

		public static string GetRegionName(DependencyObject obj)
		{
			return (string)obj.GetValue(RegionNameProperty);
		}

		public static void SetRegionName(DependencyObject obj, string value)
		{
			obj.SetValue(RegionNameProperty, value);
		}

		public static readonly DependencyProperty RegionNameProperty =
			DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(MouseClickBehavior), new PropertyMetadata(null));

		public static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			bool isEnabled = (bool)e.NewValue;
			var uiElement = sender as UIElement;
			if (uiElement != null)
			{
				uiElement.PreviewMouseDown -= OnElementPreviewMouseDown;
				if (isEnabled)
				{
					uiElement.PreviewMouseDown += OnElementPreviewMouseDown;
				}
			}
		}

		private static void OnElementPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			var region = GetRegionInVisualTree((e.OriginalSource ?? sender) as DependencyObject, MouseClickBehavior.RegionNameProperty) as FrameworkElement;
			var position = e.GetPosition(region);

			if (region != null)
			{
				string regionName = MouseClickBehavior.GetRegionName(region);
				double regionWidth = region.ActualWidth;
				double regionHeight = region.ActualHeight;
				var activityInfo = new ActivityInfo(regionName, position.X, position.Y, regionWidth, regionHeight, ActivityKind.Click)
				{
					CreateRegionImage = () => region.GetRegionJpgImage(300, 300)
				};
				if (UserActivityService.Current != null)
				{
					UserActivityService.Current.RegisterActivity(activityInfo);
				}
			}
		}
	}
}
