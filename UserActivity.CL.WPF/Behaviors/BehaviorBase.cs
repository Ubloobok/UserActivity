using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UserActivity.CL.WPF.Extensions;

namespace UserActivity.CL.WPF.Behaviors
{
	public class BehaviorBase
	{
		public static DependencyObject GetRegionInVisualTree(DependencyObject element, DependencyProperty regionNameProperty)
		{
			DependencyObject region = null;
			foreach (var currentElement in element.GetAscendantsAndSelf())
			{
				object value = currentElement.GetValue(regionNameProperty);
				string regionName = value == null ? null : (string)value;
				if (regionName != null)
				{
					region = currentElement;
					break;
				}
			}
			return region;
		}
	}
}
