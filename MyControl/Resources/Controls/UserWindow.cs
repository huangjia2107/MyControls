using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Shell;

namespace MyControl.Resources.Controls
{
    public class UserWindow : Window
    {
        private static readonly Type _typeofSelf = typeof(UserWindow);

        static UserWindow()
        {
            CommandManager.RegisterClassCommandBinding(_typeofSelf, new CommandBinding(SettingCommand, new ExecutedRoutedEventHandler(OnSettingCommand)));
        }

        public UserWindow()
        {
            this.DefaultStyleKey = typeof(UserWindow);

#if NET4
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(Microsoft.Windows.Shell.SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
#else
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
#endif
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.CloseWindow(this);
#else
            SystemCommands.CloseWindow(this);
#endif
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.MaximizeWindow(this);
#else
            SystemCommands.MaximizeWindow(this);
#endif
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.MinimizeWindow(this);
#else
            SystemCommands.MinimizeWindow(this);
#endif
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
#if NET4
            Microsoft.Windows.Shell.SystemCommands.RestoreWindow(this);
#else
            SystemCommands.RestoreWindow(this);
#endif
        }

        public static readonly DependencyProperty TitleBarBackgroundProperty = DependencyProperty.Register("TitleBarBackground", typeof(Brush), _typeofSelf);
        public Brush TitleBarBackground
        {
            get { return (Brush)GetValue(TitleBarBackgroundProperty); }
            set { SetValue(TitleBarBackgroundProperty, value); }
        }

        public static readonly DependencyProperty IsSupportSettingProperty = DependencyProperty.Register("IsSupportSetting", typeof(bool), _typeofSelf, new UIPropertyMetadata(false));
        public bool IsSupportSetting
        {
            get { return (bool)GetValue(IsSupportSettingProperty); }
            set { SetValue(IsSupportSettingProperty, value); }
        }

        private event EventHandler _OpenSetting;
        public event EventHandler OpenSetting
        {
            add { _OpenSetting += value; }
            remove { _OpenSetting -= value; }
        }

        public static RoutedUICommand SettingCommand
        {
            get { return settingCommand; }
        }
        private static RoutedUICommand settingCommand = new RoutedUICommand("SettingCommand", "SettingCommand", _typeofSelf);

        private static void OnSettingCommand(object sender, RoutedEventArgs e)
        {
            UserWindow obj = sender as UserWindow;
            if (obj._OpenSetting != null)
            {
                obj._OpenSetting(e.OriginalSource, e);
            }
        }
    }
}
