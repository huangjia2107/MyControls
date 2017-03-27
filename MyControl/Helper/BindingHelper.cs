using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AmicaUI.Models;
using AmicaPCEDotNetWrap.CLR;
using AmicaUI.UIControl;

namespace AmicaUI.Utils
{
    public class BindingHelper
    {
        //AttachedProperty
        //DynamicResource
        public static readonly DependencyProperty ResourceKeyProperty = DependencyProperty.RegisterAttached("ResourceKey", typeof(string), typeof(BindingHelper), new UIPropertyMetadata(ResourceKeyPropertyChangedCallback));
        public static string GetResourceKey(DependencyObject obj)
        {
            return (string)obj.GetValue(ResourceKeyProperty);
        }
        public static void SetResourceKey(DependencyObject obj, string value)
        {
            obj.SetValue(ResourceKeyProperty, value);
        }
        static void ResourceKeyPropertyChangedCallback(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            UpdateResourceReference(target as FrameworkElement);
        }

        public static readonly DependencyProperty DPProperty = DependencyProperty.RegisterAttached("DP", typeof(DependencyProperty), typeof(BindingHelper), new UIPropertyMetadata(DPPropertyChangedCallback));
        public static DependencyProperty GetDP(DependencyObject obj)
        {
            return (DependencyProperty)obj.GetValue(DPProperty);
        }
        public static void SetDP(DependencyObject obj, DependencyProperty value)
        {
            obj.SetValue(DPProperty, value);
        }
        static void DPPropertyChangedCallback(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            UpdateResourceReference(target as FrameworkElement);
        }

        private static void UpdateResourceReference(FrameworkElement element)
        {
            string resourceKey = GetResourceKey(element);
            DependencyProperty dp = GetDP(element);

            if (string.IsNullOrEmpty(resourceKey) || dp == DependencyProperty.UnsetValue || dp == null)
                return;

            element.SetResourceReference(dp, resourceKey);
        }
    }
}
