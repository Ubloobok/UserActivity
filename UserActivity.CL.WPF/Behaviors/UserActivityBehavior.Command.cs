using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UserActivity.CL.WPF.Entities;
using UserActivity.CL.WPF.Services;
using UserActivity.CL.WPF.Extensions;
using System.Windows.Input;

namespace UserActivity.CL.WPF.Behaviors
{
    public partial class UserActivityBehavior
    {
        public static string GetCommandName(DependencyObject obj) =>
            (string)obj.GetValue(CommandNameProperty);

        public static void SetCommandName(DependencyObject obj, string value) =>
            obj.SetValue(CommandNameProperty, value);

        public static readonly DependencyProperty CommandNameProperty =
            DependencyProperty.RegisterAttached("CommandName", typeof(string), typeof(UserActivityBehavior), new PropertyMetadata(null, OnCommandNameChanged));

        public static void OnCommandNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var buttonBase = sender as ButtonBase;
            if (buttonBase != null)
            {
                buttonBase.Click -= OnButtonBaseClick;
                if (e.NewValue != null)
                {
                    buttonBase.Click += OnButtonBaseClick;
                }
            }
        }

        private static void OnButtonBaseClick(object sender, RoutedEventArgs e)
        {
            var region = GetRegionInVisualTree(e.OriginalSource ?? sender);
            if ((region == null) || !GetIsEnabled(region))
                return;

            var button = sender as ButtonBase;
            var position = button.TransformToAncestor(region).Transform(new Point(0, 0));
            position = new Point(position.X + button.ActualWidth / 2, position.Y + button.ActualHeight / 2);

            string regionName = GetRegionName(region);
            double regionWidth = region.ActualWidth;
            double regionHeight = region.ActualHeight;
            string commandName = GetCommandName(button);
            var ev = new EventInfo(regionName, position.X, position.Y, regionWidth, regionHeight, EventKind.Command, commandName)
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
