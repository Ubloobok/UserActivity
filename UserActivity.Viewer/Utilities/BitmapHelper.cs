using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UserActivity.Viewer.Utilities
{
	[DebuggerDisplay("# {Alpha} {Red} {Green} {Blue}")]
	[StructLayout(LayoutKind.Sequential)]
	public struct PixelColor
	{
		public byte Blue;
		public byte Green;
		public byte Red;
		public byte Alpha;
	}

	public class BitmapHelper
	{

		public PixelColor[,] GetPixels(BitmapSource source)
		{
			var height = source.PixelHeight;
			var width = source.PixelWidth;
			PixelColor[,] pixels = new PixelColor[width, height];
			var pixelBytes = new byte[height * width * 4];

			int stride = source.PixelWidth * ((source.Format.BitsPerPixel + 7) / 8);
			int offset = 0;

			source.CopyPixels(pixelBytes, stride, 0);
			int y0 = offset / width;
			int x0 = offset - width * y0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					pixels[x + x0, y + y0] = new PixelColor
					{
						Blue = pixelBytes[(y * width + x) * 4 + 0],
						Green = pixelBytes[(y * width + x) * 4 + 1],
						Red = pixelBytes[(y * width + x) * 4 + 2],
						Alpha = pixelBytes[(y * width + x) * 4 + 3],
					};
				}
			}
			return pixels;
		}
	}
}
