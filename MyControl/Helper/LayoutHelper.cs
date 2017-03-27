using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using AmicaUI.Utils.Adorners;

namespace TestUI.Utils
{
    public class LayoutHelper
    {
        //使Popup不置顶
        public static readonly DependencyProperty TopIndexProperty = DependencyProperty.RegisterAttached("TopIndex", typeof(int), typeof(LayoutHelper), new FrameworkPropertyMetadata(0, TopIndexPropertyChangedCallback));
        public static int GetTopIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(TopIndexProperty);
        }
        public static void SetTopIndex(DependencyObject obj, int value)
        {
            obj.SetValue(TopIndexProperty, value);
        }

        static void TopIndexPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is Window)
            {
                Window obj = sender as Window;
                var hwndSource = (HwndSource)PresentationSource.FromVisual(obj);
                if (hwndSource != null)
                {
                    Win32.RECT rect;
                    if (Win32.GetWindowRect(hwndSource.Handle, out rect))
                    {
                        obj.Loaded += (s, args) =>
                        {
                            Win32.SetWindowPos(hwndSource.Handle, LayoutHelper.GetTopIndex(obj), rect.Left, rect.Top, (int)obj.Width, (int)obj.Height, 0);
                        };
                    }
                }
            }

            if (sender is Popup)
            {
                Popup obj = sender as Popup;
                obj.Opened += (s, args) =>
                {
                    var hwndSource = (HwndSource)PresentationSource.FromVisual(obj.Child);
                    if (hwndSource != null)
                    {
                        Win32.RECT rect;
                        if (Win32.GetWindowRect(hwndSource.Handle, out rect))
                        {
                            Win32.SetWindowPos(hwndSource.Handle, LayoutHelper.GetTopIndex(obj), rect.Left, rect.Top, (int)obj.Width, (int)obj.Height, 0);
                        }
                    }
                };
            }
        }

        //Highlight
        public static Action<bool, FrameworkElement> HighlightAction = (isHighlight, obj) =>
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(obj);
            if (layer != null)
            {
                if (isHighlight == true)
                    layer.Add(new HighlightAdorner(obj));
                else
                {
                    Adorner[] adornerArray = layer.GetAdorners(obj);
                    if (adornerArray != null)
                        Array.ForEach(adornerArray, adorner => layer.Remove(adorner));
                }
            }
        }; 

        public static readonly DependencyProperty IsHighlightProperty = DependencyProperty.RegisterAttached("IsHighlight", typeof(bool), typeof(LayoutHelper), new UIPropertyMetadata(false, IsHighlightPropertyChangedCallback));
        public static bool GetIsHighlight(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsHighlightProperty);
        }
        public static void SetIsHighlight(DependencyObject obj, bool value)
        {
            obj.SetValue(IsHighlightProperty, value);
        }
        static void IsHighlightPropertyChangedCallback(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (!(target is FrameworkElement))
                return;

            FrameworkElement obj = target as FrameworkElement;
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(obj);
            if (layer == null)
            {
                RoutedEventHandler handler = null;
                handler = (s, _) => { HighlightAction((bool)e.NewValue, obj); obj.Loaded -= handler; };
                obj.Loaded += handler;
            }
            else
                HighlightAction((bool)e.NewValue, obj);
        }
    }
}
