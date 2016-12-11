using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.Model
{
	public class HeatPointItem
	{
		public int X;
		public int Y;
		public byte Intensity;

		public HeatPointItem(int x, int y, byte intensity)
		{
			X = x;
			Y = y;
			Intensity = intensity;
		}
	}
}
