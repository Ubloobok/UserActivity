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
using System.Windows.Controls.Primitives;

namespace UserActivity.CL.WPF.Behaviors
{
    public partial class UserActivityBehavior
    {
        public static bool GetIsEnabled(DependencyObject obj) =>
            (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value) =>
            obj.SetValue(IsEnabledProperty, value);

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(UserActivityBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

        public static string GetRegionName(DependencyObject obj) =>
            (string)obj.GetValue(RegionNameProperty);

        public static void SetRegionName(DependencyObject obj, string value) =>
            obj.SetValue(RegionNameProperty, value);

        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(UserActivityBehavior), null);

        public static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            bool isEnabled = (bool)e.NewValue;

            var uiElement = sender as UIElement;
            if (uiElement != null)
            {
                uiElement.PreviewMouseDown -= OnElementPreviewMouseDown;
                uiElement.PreviewMouseMove -= OnPreviewMouseMove;
                if (isEnabled)
                {
                    uiElement.PreviewMouseDown += OnElementPreviewMouseDown;
                    uiElement.PreviewMouseMove += OnPreviewMouseMove;
                }
            }
        }

        public static FrameworkElement GetRegionInVisualTree(object source)
        {
            var element = source as DependencyObject;
            DependencyObject region = null;
            foreach (var currentElement in element.GetAscendantsAndSelf())
            {
                object value = currentElement.GetValue(RegionNameProperty);
                string regionName = value as string;
                if (regionName != null)
                {
                    region = currentElement;
                    break;
                }
            }
            return region as FrameworkElement;
        }
    }
}
