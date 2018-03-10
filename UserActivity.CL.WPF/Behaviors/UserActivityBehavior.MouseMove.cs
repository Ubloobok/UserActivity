using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UserActivity.CL.WPF.Entities;
using UserActivity.CL.WPF.Extensions;
using UserActivity.CL.WPF.Services;

namespace UserActivity.CL.WPF.Behaviors
{
    public partial class UserActivityBehavior
    {
        public static DateTime GetLastMoveDateTime(DependencyObject obj) =>
            (DateTime)obj.GetValue(LastMoveDateTimeProperty);

        public static void SetLastMoveDateTime(DependencyObject obj, DateTime value) =>
            obj.SetValue(LastMoveDateTimeProperty, value);

        public static readonly DependencyProperty LastMoveDateTimeProperty =
            DependencyProperty.RegisterAttached("LastMoveDateTime", typeof(DateTime), typeof(UserActivityBehavior));

        public static Point GetLastMovePosition(DependencyObject obj) =>
            (Point)obj.GetValue(LastMovePositionProperty);

        public static void SetLastMovePosition(DependencyObject obj, Point value) =>
            obj.SetValue(LastMovePositionProperty, value);

        public static readonly DependencyProperty LastMovePositionProperty =
            DependencyProperty.RegisterAttached("LastMovePosition", typeof(Point), typeof(UserActivityBehavior));

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            var region = GetRegionInVisualTree(e.OriginalSource ?? sender);
            if (region == null)
                return;

            var prevDate = GetLastMoveDateTime(region);
            var prevPoint = GetLastMovePosition(region);

            var newDate = DateTime.Now;
            var newPoint = e.GetPosition(region);

            if (((Math.Abs(newPoint.X - prevPoint.X) > 5) || (Math.Abs(newPoint.Y - prevPoint.Y) > 5))
                && ((newDate - prevDate).Milliseconds > 50))
            {
                string regionName = GetRegionName(region);
                double regionWidth = region.ActualWidth;
                double regionHeight = region.ActualHeight;
                var ev = new EventInfo(regionName, newPoint.X, newPoint.Y, regionWidth, regionHeight, EventKind.Movement)
                {
                    CreateRegionImage = () => region.GetRegionJpgImage()
                };
                if (UserActivityService.Current != null)
                {
                    UserActivityService.Current.RegisterEvent(ev);
                }

                SetLastMoveDateTime(region, newDate);
                SetLastMovePosition(region, newPoint);
            }
        }
    }
}
