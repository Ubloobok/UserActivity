using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UserActivity.CL.WPF.Entities;
using UserActivity.Viewer.Effects;
using UserActivity.Viewer.Model;
using UserActivity.Viewer.ViewModel;
using UserActivity.Viewer.ViewModel.Items;

namespace UserActivity.Viewer.View
{
	/// <summary>
	/// Interaction logic for ClickMapTabView.xaml
	/// </summary>
	public partial class HeatMapView : UserControl
	{
		private List<HeatPointItem> _heatPoints;
		private RenderTargetBitmap _intensityMap;
		private Rectangle _clearRectangle;

		public HeatMapView()
		{
			InitializeComponent();

			AddativeBlendClear cleareffect = new AddativeBlendClear();
			cleareffect.ClearColor = Color.FromArgb(0x01, 0xFF, 0xFF, 0xFF);

			_heatPoints = new List<HeatPointItem>();
			_intensityMap = new RenderTargetBitmap(1200, 1200, 96, 96, PixelFormats.Pbgra32);
			//m_intensityMap = new RenderTargetBitmap((int)BackgroundImage.ActualWidth, (int)BackgroundImage.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			ClearIntensityMap();

			Size sz = new Size(_intensityMap.PixelWidth, _intensityMap.PixelHeight);

			// Create the clear rectangle, we need this to render a fade pass.
			_clearRectangle = new Rectangle();
			_clearRectangle.Fill = new ImageBrush(_intensityMap);
			_clearRectangle.Effect = cleareffect;
			_clearRectangle.Measure(sz);
			_clearRectangle.Arrange(new Rect(sz));

			// Connect the intensity map containing our heat to our image.
			HeatImage.Source = _intensityMap;
			Loaded += OnLoaded;
			PalleteSelector.SelectionChanged += OnPalleteSelectorSelectionChanged;
		}

		public HeatMapVM ViewModel
		{
			get;
			set;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			int newWidth = BackgroundImage.ActualWidth == 0 ? 1360 : (int)BackgroundImage.ActualWidth;
			int newHeight = BackgroundImage.ActualHeight == 0 ? 780 : (int)BackgroundImage.ActualHeight;

			if (((newWidth != 0) && (newHeight != 0))
				&& ((_intensityMap.PixelWidth != newWidth)
					|| (_intensityMap.PixelHeight != newHeight)))
			{
				_intensityMap = new RenderTargetBitmap(newWidth, newHeight, 96, 96, PixelFormats.Pbgra32);
				HeatImage.Source = _intensityMap;
			}

			ClearIntensityMap();
			RenderIntensityMap(_intensityMap, _heatPoints, true);

			if (PalleteSelector.SelectedIndex < 0)
			{
				PalleteSelector.SelectedIndex = 1;
				PalleteSelector.SelectedIndex = 0;
				PalleteSelector.InvalidateVisual();
			}
		}

		public void SetActivities(IEnumerable<Activity> activities)
		{
			foreach (var activity in activities)
			{
				_heatPoints.Add(new HeatPointItem((int)activity.InRegionX, (int)activity.InRegionY, 255));
			}
			ClearIntensityMap();
			RenderIntensityMap(_intensityMap, _heatPoints, true);
		}

		public void SetRegion(RegionImageItemVM region)
		{
			if ((region != null) && (region.Image != null))
			{
				var image = region.Image;
				ImageSource imageSource = null;

				if ((image.Type == ImageType.FileBmp) || (image.Type == ImageType.FileJpg) || (image.Type == ImageType.FilePng))
				{
					var bitmapImage = new BitmapImage();

					bitmapImage.BeginInit();
					bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					string filepath = System.IO.Path.Combine(Environment.CurrentDirectory, "Assets", "Images", image.Source);
					bitmapImage.UriSource = new Uri(filepath, UriKind.Absolute);
					bitmapImage.StreamSource = null;
					bitmapImage.EndInit();

					imageSource = bitmapImage;
				}
				else if ((image.Type == ImageType.RawBmp) || (image.Type == ImageType.RawJpg) || (image.Type == ImageType.RawPng))
				{
					var bitmapImage = new BitmapImage();
					using (var mem = new MemoryStream(region.Image.Data))
					{
						mem.Position = 0;
						bitmapImage.BeginInit();
						bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.UriSource = null;
						bitmapImage.StreamSource = mem;
						bitmapImage.EndInit();
					}
					imageSource = bitmapImage;
				}
				BackgroundImage.Source = imageSource;
			}
		}

		private void OnPalleteSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PalleteSelector.SelectedItem != null)
			{
				var pallete = ((PalleteSelector.SelectedItem as ComboBoxItem).Content as Rectangle).Fill;
				var visualPallete = new VisualBrush(new Rectangle() { Width = 256, Height = 1, Fill = pallete });
				HeatEffect.Palette = visualPallete;
			}
		}

		private void ClearHeatClick(object sender, RoutedEventArgs e)
		{
			_heatPoints.Clear();
			ClearIntensityMap();
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			if (!(Keyboard.Modifiers == ModifierKeys.Control))
			{
				return;
			}

			var p = e.GetPosition(HeatImage);
			int x = (int)(p.X);
			int y = (int)(p.Y);
			byte intensity = 255;

			_heatPoints.Add(new HeatPointItem(x, y, intensity));

			ClearIntensityMap();
			RenderIntensityMap(_intensityMap, _heatPoints, true);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			if (!(Keyboard.Modifiers == ModifierKeys.Alt))
			{
				return;
			}

			var p = e.GetPosition(HeatImage);
			int x = (int)(p.X);
			int y = (int)(p.Y);
			byte intensity = 255;

			_heatPoints.Add(new HeatPointItem(x, y, intensity));

			ClearIntensityMap();
			RenderIntensityMap(_intensityMap, _heatPoints, true);
		}

		private void RenderIntensityMap(RenderTargetBitmap map, List<HeatPointItem> points, bool addHeat)
		{
			RadialGradientBrush radialBrush = new RadialGradientBrush();

			int displayRadius = ViewModel.PointGradientRadius;
			int groupRadius = ViewModel.PointOverlapRadius;

			int maxNearestCount = points.Count == 0 ? 0 : points
				.Max(point => points
					.Where(p => (Math.Abs(point.X - p.X) <= groupRadius) && (Math.Abs(point.Y - p.Y) <= groupRadius))
					.Count());
			float rawIntensity = 1f / maxNearestCount;
			float intensity = rawIntensity;

			foreach (HeatPointItem point in points)
			{
				DrawingVisual dv = new DrawingVisual();
				using (DrawingContext ctx = dv.RenderOpen())
				{
					radialBrush.GradientStops.Clear();
					if (addHeat)
					{
						radialBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(intensity, 0, 0, 0), 0.0));
						radialBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(0f, 0, 0, 0), 1.0));
					}
					else
					{
						radialBrush.GradientStops.Add(new GradientStop(Color.FromArgb(point.Intensity, 0xFF, 0xFF, 0xFF), 0.0));
						radialBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0xFF, 0xFF, 0xFF), 1.0));
					}

					ctx.DrawRectangle(radialBrush, null, new Rect(point.X - displayRadius, point.Y - displayRadius, displayRadius * 2, displayRadius * 2));
				}
				map.Render(dv);
			}
		}

		private void ClearIntensityMap()
		{
			DrawingVisual dv = new DrawingVisual();
			using (DrawingContext ctx = dv.RenderOpen())
			{
				ctx.DrawRectangle(Brushes.White, null, new Rect(0, 0, _intensityMap.PixelWidth, _intensityMap.PixelHeight));
			}
			_intensityMap.Render(dv);
		}

		public void Refresh()
		{
			ClearIntensityMap();
			RenderIntensityMap(_intensityMap, _heatPoints, true);
		}

		public void Unload()
		{
			ViewModel = null;
			_intensityMap.Clear();
			_intensityMap = null;
			HeatImage.Source = null;
			HeatImage = null;
			BackgroundImage.Source = null;
			BackgroundImage = null;
		}
	}
}
