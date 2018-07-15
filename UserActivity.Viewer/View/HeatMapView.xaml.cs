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
        private byte _defaultIntensity = 255;

        public HeatMapView()
        {
            InitializeComponent();

            AddativeBlendClear cleareffect = new AddativeBlendClear();
            cleareffect.ClearColor = Color.FromScRgb(1, 0, 0, 0);

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

        public HeatMapVM ViewModel => DataContext as HeatMapVM;

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

        public void SetEvents(IEnumerable<Event> events)
        {
            _heatPoints.Clear();
            foreach (var activity in events)
            {
                _heatPoints.Add(new HeatPointItem((int)activity.InRegionX, (int)activity.InRegionY, _defaultIntensity));
            }
            Refresh();
        }

        public void SetRegion(RegionImageItemVM region)
        {
            if ((region != null) && (region.Image != null))
            {
                var image = region.Image;
                ImageSource imageSource = null;

                if ((image.ImageType == ImageType.FileBmp) || (image.ImageType == ImageType.FileJpg) || (image.ImageType == ImageType.FilePng))
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
                else if ((image.ImageType == ImageType.RawBmp) || (image.ImageType == ImageType.RawJpg) || (image.ImageType == ImageType.RawPng))
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
            Refresh();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                var p = e.GetPosition(HeatImage);
                int x = (int)(p.X);
                int y = (int)(p.Y);

                _heatPoints.Add(new HeatPointItem(x, y, _defaultIntensity));
                Refresh();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                var p = e.GetPosition(HeatImage);
                int x = (int)(p.X);
                int y = (int)(p.Y);

                _heatPoints.Add(new HeatPointItem(x, y, _defaultIntensity));
                Refresh();
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                var p = e.GetPosition(HeatImage);
                int x = (int)(p.X);
                int y = (int)(p.Y);

                int bitsPerPixel = 32;
                int bytesPerPixel = bitsPerPixel / 8;

                int stride = _intensityMap.PixelWidth * bytesPerPixel;
                byte[] data = new byte[_intensityMap.PixelHeight * stride];
                _intensityMap.CopyPixels(data, stride, 0);

                // First three bytes have equal value.
                int position = y * stride + x * bytesPerPixel;
                byte intensity = data[position];
                CurIntentsity.Text = $"X: {x}  Y: {y}  I: {intensity}";
            }
        }

        private void OnHeatmapMethodChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void RenderIntensityMap(RenderTargetBitmap map, List<HeatPointItem> points, bool addHeat)
        {
            bool isOld = ((ComboBoxItem)HeatmapMethod.SelectedValue).Content.ToString() == "Старый";

            int displayRadius = ViewModel.PointGradientRadius;
            int groupRadius = ViewModel.PointOverlapRadius;

            int maxNearestCount = points.Count == 0 ? 0 : points
                .Max(point => points
                    .Where(p => (Math.Abs(point.X - p.X) <= groupRadius) && (Math.Abs(point.Y - p.Y) <= groupRadius))
                    .Count());
            float startIntensity = (1f / maxNearestCount);

            if (isOld)
            {
                RadialGradientBrush radialBrush = new RadialGradientBrush();

                foreach (HeatPointItem point in points)
                {
                    DrawingVisual dv = new DrawingVisual();
                    using (DrawingContext ctx = dv.RenderOpen())
                    {
                        radialBrush.GradientStops.Clear();
                        //if (addHeat)
                        //{
                            radialBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(startIntensity, 0, 0, 0), 0.0));
                            radialBrush.GradientStops.Add(new GradientStop(Color.FromScRgb(0f, 0, 0, 0), 1.0));
                        //}
                        //else
                        //{
                        //    radialBrush.GradientStops.Add(new GradientStop(Color.FromArgb(intensity, 0, 0, 0), 0.0));
                        //    radialBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
                        //}

                        ctx.DrawRectangle(radialBrush, null, new Rect(point.X - displayRadius, point.Y - displayRadius, displayRadius * 2, displayRadius * 2));
                    }
                    map.Render(dv);
                }

                HeatImage.Source = map;
            }
            else
            {
                int width = (int)HeatImage.ActualWidth;
                int height = (int)HeatImage.ActualHeight;
                // variables declaration
                WriteableBitmap img = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                uint[] pixels = new uint[width * height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var intensity = 0.0d;
                        for (int i = 0; i < points.Count; i++)
                        {
                            var point = points[i];
                            var distance = Math.Sqrt(Math.Pow(x - point.X, 2) + Math.Pow(y - point.Y, 2));
                            if (distance < displayRadius)
                            {
                                intensity += 1.0d - distance / displayRadius;
                            }
                        }
                        intensity = intensity == 0 ? 0 : intensity / maxNearestCount;

                        int pixel = width * y + x;

                        int color =
                            intensity == 0 ? 255 :
                            intensity >= 1 ? 0 :
                            Convert.ToInt32((1 - intensity) * 255);

                        int red = color;
                        int green = color;
                        int blue = color;

                        int alpha =
                            intensity == 0 ? 255 :
                            intensity >= 1 ? 0 :
                            Convert.ToInt32((1 - intensity) * 255);

                        pixels[pixel] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                    }
                }

                img.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
                HeatImage.Source = img;  // image1 is a WPF Image in my XAML
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
            if (_intensityMap != null)
            {
                ClearIntensityMap();
                RenderIntensityMap(_intensityMap, _heatPoints, true);
            }
        }

        public void Unload()
        {
            DataContext = null;
            _intensityMap.Clear();
            _intensityMap = null;
            HeatImage.Source = null;
            HeatImage = null;
            BackgroundImage.Source = null;
            BackgroundImage = null;
        }

        private void OnButtonAddDataClick(object sender, RoutedEventArgs e)
        {
            var points =
                from pointString in InputData.Text?.Split(';')
                let pointParts = pointString.Split(',')
                select new HeatPointItem(int.Parse(pointParts[0]), int.Parse(pointParts[1]), _defaultIntensity);

            _heatPoints.AddRange(points);
            Refresh();
        }
    }
}
