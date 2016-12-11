using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.CL.WPF.Entities
{
	public class ScreenInfo
	{
		public ScreenInfo()
		{
		}

		public ScreenInfo(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public double Width { get; set; }
		public double Height { get; set; }
	}
}
