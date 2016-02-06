using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyControl.Resources.Controls
{
    public class UserButton : Button
    {
        private static readonly Type _typeofSelf = typeof(UserButton);
        public UserButton()
        {
            DefaultStyleKey = typeof(Button);
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), _typeofSelf, new UIPropertyMetadata(0));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }


        public static readonly DependencyProperty DefaultFgProperty = DependencyProperty.Register("DefaultFg", typeof(Brush), _typeofSelf, new UIPropertyMetadata(Brushes.Black));
        public Brush DefaultFg
        {
            get { return (Brush)GetValue(DefaultFgProperty); }
            set { SetValue(DefaultFgProperty, value); }
        }


        public static readonly DependencyProperty DefaultBgProperty = DependencyProperty.Register("DefaultBg", typeof(Brush), _typeofSelf, new UIPropertyMetadata(Brushes.Transparent));
        public Brush DefaultBg
        {
            get { return (Brush)GetValue(DefaultBgProperty); }
            set { SetValue(DefaultBgProperty, value); }
        }


        public static readonly DependencyProperty MouseOverFgProperty = DependencyProperty.Register("MouseOverFg", typeof(Brush), _typeofSelf, new UIPropertyMetadata(Brushes.Black));
        public Brush MouseOverFg
        {
            get { return (Brush)GetValue(MouseOverFgProperty); }
            set { SetValue(MouseOverFgProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBgProperty = DependencyProperty.Register("MouseOverBg", typeof(Brush), _typeofSelf, new UIPropertyMetadata(Brushes.Transparent));
        public Brush MouseOverBg
        {
            get { return (Brush)GetValue(MouseOverBgProperty); }
            set { SetValue(MouseOverBgProperty, value); }
        }

        public static readonly DependencyProperty PressedFgProperty = DependencyProperty.Register("PressedFg", typeof(Brush), _typeofSelf, new UIPropertyMetadata(Brushes.Black));
        public Brush PressedFg
        {
            get { return (Brush)GetValue(PressedFgProperty); }
            set { SetValue(PressedFgProperty, value); }
        }

        public static readonly DependencyProperty PressedBgProperty = DependencyProperty.Register("PressedBg", typeof(Brush), _typeofSelf, new UIPropertyMetadata(Brushes.Transparent));
        public Brush PressedBg
        {
            get { return (Brush)GetValue(PressedBgProperty); }
            set { SetValue(PressedBgProperty, value); }
        }

    }
}
