using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserActivity.CL.WPF.Entities
{
	public class ActivityInfo
	{
		public ActivityInfo()
			: this(null, default(double), default(double), default(double), default(double), ActivityKind.Unknown)
		{
		}

		public ActivityInfo(double x, double y, ActivityKind globalType)
			: this(null, x, y, default(double), default(double), globalType)
		{
		}

		public ActivityInfo(string regionName, double inRegionX, double inRegionY, double regionWidth, double regionHeight, ActivityKind globalType)
		{
			GlobalType = globalType;
			InRegionX = inRegionX;
			InRegionY = inRegionY;
			RegionName = regionName;
			RegionWidth = regionWidth;
			RegionHeight = regionHeight;
		}

		public ActivityKind GlobalType { get; set; }
		public double InRegionX { get; set; }
		public double InRegionY { get; set; }
		public string RegionName { get; set; }
		public double RegionWidth { get; set; }
		public double RegionHeight { get; set; }

		public Func<Image> CreateRegionImage { get; set; }
	}
}
