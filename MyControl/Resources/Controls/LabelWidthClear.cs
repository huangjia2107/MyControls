using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyControl.Resources.Controls
{
    [TemplatePart(Name = "PART_ClearBtn", Type = typeof(Button))]
    public class LabelWidthClear : Label
    {
        Button _ClearBtn = null;

        static LabelWidthClear()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LabelWidthClear), new FrameworkPropertyMetadata(typeof(LabelWidthClear)));
            CommandManager.RegisterClassCommandBinding(typeof(LabelWidthClear), new CommandBinding(ClearCommand, OnClearCommand));
        }

        public LabelWidthClear()
        {
            var v = DependencyPropertyDescriptor.FromProperty(LabelWidthClear.IsMouseOverProperty, typeof(LabelWidthClear));
            v.AddValueChanged(this, OnIsMouseOverPropertyChanged);

            this.MouseRightButtonDown += new MouseButtonEventHandler(LabelWidthClear_MouseRightButtonDown);
            this.LostFocus += new RoutedEventHandler(LabelWidthClear_LostFocus);
        }

        void LabelWidthClear_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_ClearBtn != null)
            {
                _ClearBtn.Visibility = Visibility.Collapsed;
            }
        }

        void LabelWidthClear_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.Content != null)
            {
                if (!string.IsNullOrEmpty(this.Content.ToString()) && _ClearBtn != null)
                {
                    if (_ClearBtn.Visibility == Visibility.Visible)
                        return;

                    _ClearBtn.Visibility = Visibility.Visible;
                    this.Focus();
                }
            }
        }

        private void OnIsMouseOverPropertyChanged(object sender, EventArgs e)
        {
            LabelWidthClear obj = sender as LabelWidthClear;
            if (obj.IsMouseOver)
            {
                if (obj.Content != null)
                {
                    if (!string.IsNullOrEmpty(obj.Content.ToString()) && obj._ClearBtn != null)
                    {
                        obj._ClearBtn.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                if (obj._ClearBtn != null)
                {
                    obj._ClearBtn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private event EventHandler _ClearCompleted;
        public event EventHandler ClearCompleted
        {
            add { _ClearCompleted += value; }
            remove { _ClearCompleted -= value; }
        }

        public static RoutedUICommand ClearCommand
        {
            get { return clearCommand; }
        }
        private static RoutedUICommand clearCommand = new RoutedUICommand("ClearCommand", "ClearCommand", typeof(LabelWidthClear));

        private static void OnClearCommand(object sender, RoutedEventArgs e)
        {
            LabelWidthClear obj = sender as LabelWidthClear;
            obj.Content = "";

            if (obj._ClearBtn != null)
                obj._ClearBtn.Visibility = Visibility.Collapsed;

            if (obj._ClearCompleted != null)
                obj._ClearCompleted(obj, new EventArgs());
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _ClearBtn = base.GetTemplateChild("PART_ClearBtn") as Button;
        }
    }
}
