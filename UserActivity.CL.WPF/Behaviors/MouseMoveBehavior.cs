using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UserActivity.CL.WPF.Entities;
using UserActivity.CL.WPF.Extensions;
using UserActivity.CL.WPF.Services;

namespace UserActivity.CL.WPF.Behaviors
{
	public class MouseMoveBehavior : BehaviorBase
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
			DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MouseMoveBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

		public static string GetRegionName(DependencyObject obj)
		{
			return (string)obj.GetValue(RegionNameProperty);
		}

		public static void SetRegionName(DependencyObject obj, string value)
		{
			obj.SetValue(RegionNameProperty, value);
		}

		public static readonly DependencyProperty RegionNameProperty =
			DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(MouseMoveBehavior), new PropertyMetadata(null));

		public static DateTime GetLastMoveDateTime(DependencyObject obj)
		{
			return (DateTime)obj.GetValue(LastMoveDateTimeProperty);
		}

		public static void SetLastMoveDateTime(DependencyObject obj, DateTime value)
		{
			obj.SetValue(LastMoveDateTimeProperty, value);
		}

		public static readonly DependencyProperty LastMoveDateTimeProperty =
			DependencyProperty.RegisterAttached("LastMoveDateTime", typeof(DateTime), typeof(MouseMoveBehavior));

		public static Point GetLastMovePosition(DependencyObject obj)
		{
			return (Point)obj.GetValue(LastMovePositionProperty);
		}

		public static void SetLastMovePosition(DependencyObject obj, Point value)
		{
			obj.SetValue(LastMovePositionProperty, value);
		}

		public static readonly DependencyProperty LastMovePositionProperty =
			DependencyProperty.RegisterAttached("LastMovePosition", typeof(Point), typeof(MouseMoveBehavior));

		public static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			bool isEnabled = (bool)e.NewValue;
			var uiElement = sender as UIElement;
			if (uiElement != null)
			{
				uiElement.PreviewMouseMove -= OnPreviewMouseMove;
				if (isEnabled)
				{
					uiElement.PreviewMouseMove += OnPreviewMouseMove;
				}
			}
		}

		private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
		{
			var region = GetRegionInVisualTree((e.OriginalSource ?? sender) as DependencyObject, MouseMoveBehavior.RegionNameProperty) as FrameworkElement;

			if (region != null)
			{
				var prevDate = GetLastMoveDateTime(region);
				var prevPoint = GetLastMovePosition(region);

				var newDate = DateTime.Now;
				var newPoint = e.GetPosition(region);

				if (((Math.Abs(newPoint.X - prevPoint.X) > 5) || (Math.Abs(newPoint.Y - prevPoint.Y) > 5))
					&& ((newDate - prevDate).Milliseconds > 50))
				{
					string regionName = MouseMoveBehavior.GetRegionName(region);
					double regionWidth = region.ActualWidth;
					double regionHeight = region.ActualHeight;
					var activityInfo = new ActivityInfo(regionName, newPoint.X, newPoint.Y, regionWidth, regionHeight, ActivityKind.Movement)
					{
						CreateRegionImage = () => region.GetRegionJpgImage(300, 300)
					};
					if (UserActivityService.Current != null)
					{
						UserActivityService.Current.RegisterActivity(activityInfo);
					}

					SetLastMoveDateTime(region, newDate);
					SetLastMovePosition(region, newPoint);
				}
			}
		}
	}
}
