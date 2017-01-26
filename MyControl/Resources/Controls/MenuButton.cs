using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace MyControl.Resources.Controls
{
    [TemplatePart(Name = PART_StatusCheckBox, Type = typeof(StatusCheckBox))]
    public class MenuButton : CheckBox
    {
        private const string PART_StatusCheckBox = "PART_StatusCheckBox";
        private StatusCheckBox _StatusCheckBox = null;

        static MenuButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuButton), new FrameworkPropertyMetadata(typeof(MenuButton)));
            CommandManager.RegisterClassCommandBinding(typeof(MenuButton), new CommandBinding(StatusCheckBoxClickCommand, OnStatusCheckBoxClickCommand));
        }

        public static readonly DependencyProperty IsShowClickButtonProperty = DependencyProperty.Register("IsShowClickButton", typeof(bool), typeof(MenuButton), new UIPropertyMetadata(true));
        public bool IsShowClickButton
        {
            get { return (bool)GetValue(IsShowClickButtonProperty); }
            set { SetValue(IsShowClickButtonProperty, value); }
        }

        #region override

        protected override void OnClick()
        {
            base.OnClick();

            if (!IsShowClickButton)
                OpenMenu(this);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);

            if (this.ContextMenu != null)
                this.ContextMenu.Visibility = Visibility.Collapsed;
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (this.ContextMenu != null && e.Key == Key.Apps)
                this.ContextMenu.Visibility = Visibility.Collapsed;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _StatusCheckBox = base.GetTemplateChild(PART_StatusCheckBox) as StatusCheckBox;
            _StatusCheckBox.Click += (s, e) => e.Handled = true;
        }

        #endregion

        #region Command

        public static RoutedUICommand StatusCheckBoxClickCommand
        {
            get { return _StatusCheckBoxClickCommand; }
        }
        private static RoutedUICommand _StatusCheckBoxClickCommand = new RoutedUICommand("StatusCheckBoxClickCommand", "StatusCheckBoxClickCommand", typeof(MenuButton));

        private static void OnStatusCheckBoxClickCommand(object sender, RoutedEventArgs e)
        {
            MenuButton obj = sender as MenuButton;
            obj.OpenMenu(obj._StatusCheckBox);
        }
        private void OpenMenu(CheckBox checkBox)
        {
            if (this.ContextMenu == null || checkBox == null || this.Visibility != Visibility.Visible)
                return;

            this.ContextMenu.Closed -= ContextMenu_Closed;
            this.ContextMenu.Closed += ContextMenu_Closed;

            this.ContextMenu.Visibility = Visibility.Visible;
            this.ContextMenu.PlacementTarget = this;
            this.ContextMenu.Placement = PlacementMode.Bottom;

            this.ContextMenu.IsOpen = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
        }

        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            if (_StatusCheckBox != null)
                _StatusCheckBox.IsChecked = false;

            this.IsChecked = false;
        }

        #endregion
    }
}
