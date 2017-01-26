using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace AmicaUI.Resources.Controls
{
    class StatusCheckBox : CheckBox
    {
        static StatusCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusCheckBox), new FrameworkPropertyMetadata(typeof(StatusCheckBox)));
        }

        public static readonly DependencyProperty IsBgTransparentProperty = DependencyProperty.Register("IsBgTransparent", typeof(bool), typeof(StatusCheckBox), new FrameworkPropertyMetadata(false));
        public bool IsBgTransparent
        {
            get { return (bool)GetValue(IsBgTransparentProperty); }
            set { SetValue(IsBgTransparentProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(StatusCheckBox), new FrameworkPropertyMetadata(new CornerRadius(0, 0, 0, 0)));
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StatusCheckBox ctrl = (StatusCheckBox)d; 
            ctrl.OnContentChanged(e.OldValue, e.NewValue);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent); 
        }

        public static readonly DependencyProperty CheckedContentProperty = DependencyProperty.Register("CheckedContent", typeof(object), typeof(StatusCheckBox),
            new FrameworkPropertyMetadata((object)null,OnContentChanged));
        [Bindable(true)]
        public object CheckedContent
        {
            get { return (object)GetValue(CheckedContentProperty); }
            set { SetValue(CheckedContentProperty, value); }
        }

        public static readonly DependencyProperty UnCheckedContentProperty = DependencyProperty.Register("UnCheckedContent", typeof(object), typeof(StatusCheckBox),
            new FrameworkPropertyMetadata((object)null,OnContentChanged));
        [Bindable(true)]
        public object UnCheckedContent
        {
            get { return (object)GetValue(UnCheckedContentProperty); }
            set { SetValue(UnCheckedContentProperty, value); }
        }

        public static readonly DependencyProperty CheckedToolTipProperty = DependencyProperty.Register("CheckedToolTip", typeof(string), typeof(StatusCheckBox));
        public string CheckedToolTip
        {
            get { return (string)GetValue(CheckedToolTipProperty); }
            set { SetValue(CheckedToolTipProperty, value); }
        }

        public static readonly DependencyProperty UnCheckedToolTipProperty = DependencyProperty.Register("UnCheckedToolTip", typeof(string), typeof(StatusCheckBox));
        public string UnCheckedToolTip
        {
            get { return (string)GetValue(UnCheckedToolTipProperty); }
            set { SetValue(UnCheckedToolTipProperty, value); }
        }
    }
}
