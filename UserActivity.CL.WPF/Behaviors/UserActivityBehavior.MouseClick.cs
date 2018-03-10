using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UserActivity.CL.WPF.Entities;
using UserActivity.CL.WPF.Services;
using UserActivity.CL.WPF.Extensions;
using System.IO;

namespace UserActivity.CL.WPF.Behaviors
{
    public partial class UserActivityBehavior
    {
        private static void OnElementPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var region = GetRegionInVisualTree(e.OriginalSource ?? sender);
            if (region == null)
                return;

            var position = e.GetPosition(region);
            string regionName = GetRegionName(region);
            double regionWidth = region.ActualWidth;
            double regionHeight = region.ActualHeight;
            var ev = new EventInfo(regionName, position.X, position.Y, regionWidth, regionHeight, EventKind.Click)
            {
                CreateRegionImage = () => region.GetRegionJpgImage()
            };
            if (UserActivityService.Current != null)
            {
                UserActivityService.Current.RegisterEvent(ev);
            }
        }
    }
}
