using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.CL.WPF.Entities
{
	public class WindowInfo
	{
		public WindowInfo()
			: this(default(double), default(double), default(double), default(double))
		{
		}

		public WindowInfo(double width, double height)
			: this(default(double), default(double), width, height)
		{
		}

		public WindowInfo(double x, double y, double width, double height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
	}
}
