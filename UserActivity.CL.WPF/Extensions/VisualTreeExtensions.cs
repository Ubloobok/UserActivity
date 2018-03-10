using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UserActivity.CL.WPF.Entities;
using RegionImage = UserActivity.CL.WPF.Entities.Variation;

namespace UserActivity.CL.WPF.Extensions
{
    public static class VisualTreeExtensions
    {
        public static IEnumerable<DependencyObject> GetAscendantsAndSelf(this DependencyObject element)
        {
            var elements = new List<DependencyObject>();

            DependencyObject currentElement = element as DependencyObject;
            while (currentElement != null)
            {
                elements.Add(currentElement);
                var property = currentElement.GetType().GetProperty("Parent");
                if (property != null)
                {
                    var parentElement = property.GetValue(currentElement, null) as DependencyObject;
                    currentElement = parentElement ?? VisualTreeHelper.GetParent(currentElement);
                }
                else
                {
                    currentElement = VisualTreeHelper.GetParent(currentElement);
                }
            }

            return elements;
        }

        public static RegionImage GetRegionJpgImage(this UIElement source, double dpiX = 96, double dpiY = 96)
        {
            double actualWidth = source.RenderSize.Width;
            double actualHeight = source.RenderSize.Height;

            double renderWidth = actualWidth * (dpiX / 96.0d);
            double renderHeight = actualHeight * (dpiY / 96.0d);

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, dpiX, dpiY, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            byte[] _imageArray;

            using (MemoryStream outputStream = new MemoryStream())
            {
                jpgEncoder.Save(outputStream);
                _imageArray = outputStream.ToArray();
            }

            var image = new RegionImage()
            {
                Data = _imageArray,
                ImageType = ImageType.RawJpg,
                ImageDpiX = dpiX,
                ImageDpiY = dpiY,
                Width = actualWidth,
                Height = actualHeight,
            };
            return image;
        }
    }
}
