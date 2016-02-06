using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Shell;

namespace MyControl.Resources.Controls
{
    internal sealed class ButtonCollecion : INotifyPropertyChanged
    {
        public ButtonCollecion()
        {
            _CtrlButtonCollection = new ObservableCollection<Button>();
        }

        private ObservableCollection<Button> _CtrlButtonCollection;
        public ObservableCollection<Button> CtrlButtonCollection
        {
            get { return _CtrlButtonCollection; }
            set { _CtrlButtonCollection = value; InvokePropertyChanged("CtrlButtonCollection"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string strName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(strName));
            }
        }
    }

    public sealed class MessageBoxModule : Window
    {
        #region Private static fields

        private static Style CTRL_BUTTON_STYLE = Button.StyleProperty.DefaultMetadata.DefaultValue as Style;

        private static string globalSettingXMLPath = AppDomain.CurrentDomain.BaseDirectory + "Settings\\GlobalSetting.xml";
        private static string BaseStylePath = @"Resources/Base_Style.xaml";
        private static string BaseLangPath = @"Languages/en-US.xaml";

        static ButtonCollecion bc;

        private static string DefaultTitle = "Tracking";
        private static string Msg_OK = "OK";
        private static string Msg_Cancel = "Cancel";
        private static string Msg_Yes = "Yes";
        private static string Msg_No = "No";
        private static string Msg_IsRunning = "MyControl is running.";

        private static string Logol_Image;
        private static string Infomation_Image;
        private static string Question_Image;
        private static string Warning_Image;
        private static string Error_Image;


        #endregion // Private static fields

        public static bool CurrentInstanceIsRunning = false;

        #region Dependency Properties

        public new static readonly DependencyProperty TitleProperty =
    DependencyProperty.Register("Title", typeof(string), typeof(MessageBoxModule), new PropertyMetadata(DefaultTitle));

        public static readonly DependencyProperty LogolSourceProperty =
    DependencyProperty.Register("LogolSource", typeof(string), typeof(MessageBoxModule), new PropertyMetadata(""));

        public static readonly DependencyProperty ImgSourceProperty =
    DependencyProperty.Register("ImgSource", typeof(string), typeof(MessageBoxModule), new PropertyMetadata(""));

        public static readonly DependencyProperty MessageProperty =
    DependencyProperty.Register("Message", typeof(string), typeof(MessageBoxModule), new PropertyMetadata(""));

        public static readonly DependencyProperty TitleForegroundProperty =
    DependencyProperty.Register("TitleForeground", typeof(Brush), typeof(MessageBoxModule), new PropertyMetadata(Brushes.Black));

        #endregion // Dependency Properties

        #region ctors

        static MessageBoxModule()
        {

        }

        public MessageBoxModule()
        {
            try
            {
                this.DefaultStyleKey = typeof(MessageBoxModule);
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.AllowsTransparency = true;
                this.WindowStyle = WindowStyle.None;
                this.MouseLeftButtonDown += (o, e) =>
                {
                    this.DragMove();
                };
                this.Loaded += new RoutedEventHandler(MessageBoxModule_Loaded);
                this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));

                if (Application.Current == null)
                {
                    if (this.Resources.MergedDictionaries.Count != 0)
                        this.Resources.MergedDictionaries.Clear();

                    this.Resources.MergedDictionaries.Insert(0, Application.LoadComponent(new Uri(BaseStylePath, UriKind.Relative)) as ResourceDictionary);

                    //MessageBoxModule.SetDefaultCtorButtonStyle(Application.Current.Resources["NormalBtnStyle"] as Style); 

                    Msg_OK = this.Resources["StringKey_MessageBox_OK"] as string;
                    Msg_Cancel = this.Resources["StringKey_MessageBox_Cancel"] as string;
                    Msg_Yes = this.Resources["StringKey_MessageBox_Yes"] as string;
                    Msg_No = this.Resources["StringKey_MessageBox_No"] as string;
                    Msg_IsRunning = this.Resources["StringKey_App_MessageBox_Running"] as string;

                    Logol_Image = this.Resources["MessageBox_Logol"] as string;
                    Infomation_Image = this.Resources["MessageBox_Infomation"] as string;
                    Question_Image = this.Resources["MessageBox_Question"] as string;
                    Warning_Image = this.Resources["MessageBox_Warning"] as string;
                    Error_Image = this.Resources["MessageBox_Error"] as string;
                }
                else
                {
                    //MessageBoxModule.SetDefaultCtorButtonStyle(Application.Current.Resources["NormalBtnStyle"] as Style); 

                    Msg_OK = Application.Current.Resources["StringKey_MessageBox_OK"] as string;
                    Msg_Cancel = Application.Current.Resources["StringKey_MessageBox_Cancel"] as string;
                    Msg_Yes = Application.Current.Resources["StringKey_MessageBox_Yes"] as string;
                    Msg_No = Application.Current.Resources["StringKey_MessageBox_No"] as string;
                    Msg_IsRunning = Application.Current.Resources["StringKey_App_MessageBox_Running"] as string;

                    Logol_Image = Application.Current.Resources["MessageBox_Logol"] as string;
                    Infomation_Image = Application.Current.Resources["MessageBox_Infomation"] as string;
                    Question_Image = Application.Current.Resources["MessageBox_Question"] as string;
                    Warning_Image = Application.Current.Resources["MessageBox_Warning"] as string;
                    Error_Image = Application.Current.Resources["MessageBox_Error"] as string;
                }    
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.Message);
            }

        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        void MessageBoxModule_Loaded(object sender, RoutedEventArgs e)
        {
            if (bc != null)
                bc.CtrlButtonCollection[0].Focus();
        }

        #endregion // ctors

        #region Public Static Functions

        public static void SetDefaultCtorButtonStyle(Style buttonStyle)
        {
            CTRL_BUTTON_STYLE = buttonStyle;
        }

        #region Show MessageBox Functions

        #region No Owner

        public new static MessageBoxResult Show()
        {
            return Show(null, null, null, DefaultTitle, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText)
        {
            return Show(null, null, messageBoxText, DefaultTitle, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(null, null, messageBoxText, caption, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxImageType imageType)
        {
            return Show(null, null, messageBoxText, DefaultTitle, imageType, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button)
        {
            return Show(null, null, messageBoxText, DefaultTitle, MessageBoxImageType.None, button);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return Show(null, null, messageBoxText, DefaultTitle, imageType, button);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImageType imageType)
        {
            return Show(null, null, messageBoxText, caption, imageType, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(null, null, messageBoxText, caption, MessageBoxImageType.None, button);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return Show(null, null, messageBoxText, caption, imageType, button);
        }

        #endregion

        #region Owner

        public static MessageBoxResult Show(Window owner)
        {
            return Show(owner, null, null, DefaultTitle, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            return Show(owner, null, messageBoxText, DefaultTitle, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            return Show(owner, null, messageBoxText, caption, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, MessageBoxImageType imageType)
        {
            return Show(owner, null, messageBoxText, DefaultTitle, imageType, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, MessageBoxButton button)
        {
            return Show(owner, null, messageBoxText, DefaultTitle, MessageBoxImageType.None, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return Show(owner, null, messageBoxText, DefaultTitle, imageType, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxImageType imageType)
        {
            return Show(owner, null, messageBoxText, caption, imageType, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(owner, null, messageBoxText, caption, MessageBoxImageType.None, button);
        }


        #endregion

        #region FrameworkElement

        public static MessageBoxResult Show(UserControl userControl)
        {
            return Show(null, userControl, null, DefaultTitle, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText)
        {
            return Show(null, userControl, messageBoxText, DefaultTitle, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption)
        {
            return Show(null, userControl, messageBoxText, caption, MessageBoxImageType.None, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, MessageBoxImageType imageType)
        {
            return Show(null, userControl, messageBoxText, DefaultTitle, imageType, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, MessageBoxButton button)
        {
            return Show(null, userControl, messageBoxText, DefaultTitle, MessageBoxImageType.None, button);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return Show(null, userControl, messageBoxText, DefaultTitle, imageType, button);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption, MessageBoxImageType imageType)
        {
            return Show(null, userControl, messageBoxText, caption, imageType, MessageBoxButton.OK);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption, MessageBoxButton button)
        {
            return Show(null, userControl, messageBoxText, caption, MessageBoxImageType.None, button);
        }

        #endregion

        public static MessageBoxResult Show(Window owner, UserControl userControl, string messageBoxText, string caption, MessageBoxImageType imageType, MessageBoxButton button)
        {
            var mbox = new MessageBoxModule();

            if (CurrentInstanceIsRunning == true)
            {
                mbox.Message = Msg_IsRunning;
            }
            else
                mbox.Message = messageBoxText;
            mbox.Title = caption;
            mbox.LogolSource = Logol_Image;
            bc = new ButtonCollecion();
            mbox.DataContext = bc;

            switch (imageType)
            {
                case MessageBoxImageType.None:
                    mbox.ImgSource = Infomation_Image;
                    break;
                case MessageBoxImageType.Infomation:
                    mbox.ImgSource = Infomation_Image;
                    break;
                case MessageBoxImageType.Question:
                    mbox.ImgSource = Question_Image;
                    break;
                case MessageBoxImageType.Warning:
                    mbox.ImgSource = Warning_Image;
                    break;
                case MessageBoxImageType.Error:
                    mbox.ImgSource = Error_Image;
                    break;
            }

            if (owner != null)
            {
                mbox.Owner = owner;
                mbox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            switch (button)
            {
                case MessageBoxButton.OKCancel:

                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultTrue(mbox, Msg_OK, MessageBoxResult.OK));

                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultFalse(mbox, Msg_Cancel, MessageBoxResult.Cancel));
                    break;
                //break;
                case MessageBoxButton.YesNo:
                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultTrue(mbox, Msg_Yes, MessageBoxResult.Yes));

                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultFalse(mbox, Msg_No, MessageBoxResult.No));

                    break;
                case MessageBoxButton.YesNoCancel:
                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultTrue(mbox, Msg_Yes, MessageBoxResult.Yes));

                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultFalse(mbox, Msg_No, MessageBoxResult.No));

                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultFalse(mbox, Msg_Cancel, MessageBoxResult.Cancel));
                    break;
                case MessageBoxButton.OK:
                default:
                    bc.CtrlButtonCollection.Add(CreateCtrlButton_ResultTrue(mbox, Msg_OK, MessageBoxResult.OK));
                    break;
            }
            mbox.Activate();
            bool? result = mbox.ShowDialog();

            switch (button)
            {

                //break;
                case MessageBoxButton.OKCancel:
                    {
                        return result == true ? MessageBoxResult.OK
                            : result == false ? MessageBoxResult.Cancel :
                            MessageBoxResult.None;
                    }
                //break;
                case MessageBoxButton.YesNo:
                    {
                        return result == true ? MessageBoxResult.Yes : MessageBoxResult.No;
                    }
                //break;
                case MessageBoxButton.YesNoCancel:
                    {
                        return result == true ? MessageBoxResult.Yes
                            : result == false ? MessageBoxResult.No :
                            MessageBoxResult.Cancel;
                    }

                case MessageBoxButton.OK:
                default:
                    {
                        return result == true ? MessageBoxResult.OK : MessageBoxResult.None;
                    }
            }
        }

        public static MessageBoxResult Show(Window owner, UserControl userControl, string messageBoxText, string caption, MessageBoxImageType imageType, IList<MessageBoxButtonInfo> ctrlButtons)
        {
            var mbox = new MessageBoxModule();

            if (CurrentInstanceIsRunning == true)
            {
                mbox.Message = Msg_IsRunning;
            }
            else
                mbox.Message = messageBoxText;
            mbox.Title = caption;
            mbox.LogolSource = Logol_Image;
            bc = new ButtonCollecion();
            mbox.DataContext = bc;

            switch (imageType)
            {
                case MessageBoxImageType.None:
                    mbox.ImgSource = Infomation_Image;
                    break;
                case MessageBoxImageType.Infomation:
                    mbox.ImgSource = Infomation_Image;
                    break;
                case MessageBoxImageType.Question:
                    mbox.ImgSource = Question_Image;
                    break;
                case MessageBoxImageType.Warning:
                    mbox.ImgSource = Warning_Image;
                    break;
                case MessageBoxImageType.Error:
                    mbox.ImgSource = Error_Image;
                    break;
            }

            if (owner != null)
            {
                mbox.Owner = owner;
                mbox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            if (null != ctrlButtons && ctrlButtons.Count > 0)
            {
                foreach (var btnInfo in ctrlButtons)
                {
                    switch (btnInfo.Result)
                    {
                        case MessageBoxResult.Cancel:
                        case MessageBoxResult.No:
                            {
                                var btn = CreateCtrlButton_ResultFalse(mbox, btnInfo.ContentText, MessageBoxResult.No);
                                btn.Command = new MessageBoxCommand(btnInfo.Action, null);
                                bc.CtrlButtonCollection.Add(btn);
                            }
                            break;
                        case MessageBoxResult.None:
                            {
                                var btn = CreateCtrlButton_ResultNull(mbox, btnInfo.ContentText, MessageBoxResult.None);
                                btn.Command = new MessageBoxCommand(btnInfo.Action, null);
                                bc.CtrlButtonCollection.Add(btn);
                            }
                            break;
                        case MessageBoxResult.OK:
                        case MessageBoxResult.Yes:
                        default:
                            {
                                var btn = CreateCtrlButton_ResultTrue(mbox, btnInfo.ContentText, MessageBoxResult.Yes);
                                btn.Command = new MessageBoxCommand(btnInfo.Action, null);
                                bc.CtrlButtonCollection.Add(btn);
                            }
                            break;
                    }
                }

                bool? result = mbox.ShowDialog();

                mbox.Activate();
                return MessageBoxResult.None;
            }
            else
            {
                return Show(owner, userControl, messageBoxText, caption, imageType, MessageBoxButton.OK);
            }


        }

        #endregion // Show MessageBox Functions

        #endregion // Public Static Functions

        #region Properties

        public new string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string LogolSource
        {
            get { return (string)GetValue(LogolSourceProperty); }
            set { SetValue(LogolSourceProperty, value); }
        }

        public string ImgSource
        {
            get { return (string)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public Brush TitleForeground
        {
            get { return (Brush)GetValue(TitleForegroundProperty); }
            set { SetValue(TitleForegroundProperty, value); }
        }

        #endregion // Properties

        #region Private static functions

        private static Button CreateCtrlButton(string content, RoutedEventHandler clickHandler, MessageBoxResult result)
        {
            Button btn = new Button();
           // btn.Style = CTRL_BUTTON_STYLE;

            if (result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
            {
                btn.IsDefault = true;
            }
            else
            {
                btn.IsCancel = true;
                btn.TabIndex = 0;
            }

            btn.Width = 70;
            btn.Height = 32;
            btn.Padding = new Thickness(10, 4, 10, 4);
            btn.Margin = new Thickness(5, 0, 5, 0);
            btn.Content = content;
            btn.Click += clickHandler;

            return btn;
        }

        private static Button CreateCtrlButton_ResultTrue(MessageBoxModule mbox, string content, MessageBoxResult result)
        {
            return CreateCtrlButton(content, new RoutedEventHandler((o, e) =>
            {
                try
                {
                    mbox.DialogResult = true;
                    mbox.Close();
                }
                catch { }
            }), result);
        }

        private static Button CreateCtrlButton_ResultFalse(MessageBoxModule mbox, string content, MessageBoxResult result)
        {
            return CreateCtrlButton(content, new RoutedEventHandler((o, e) =>
            {
                try
                {
                    mbox.DialogResult = false;
                    mbox.Close();
                }
                catch { }
            }), result);
        }

        private static Button CreateCtrlButton_ResultNull(MessageBoxModule mbox, string content, MessageBoxResult result)
        {
            return CreateCtrlButton(content, new RoutedEventHandler((o, e) =>
            {
                try
                {
                    mbox.DialogResult = null;
                    mbox.Close();
                }
                catch { }
            }), result);
        }

        #endregion // Private static functions
    }

    /// <summary>
    /// MessageBoxModule 组件中的信息图标显示
    /// </summary>
    public enum MessageBoxImageType
    {
        None = 0,
        Infomation = 1,
        Question = 2,
        Warning = 3,
        Error = 4
    }

    /// <summary>
    /// MessageBoxModule 组件中 MessageBoxButton 的自定义设置信息.
    /// </summary>
    public class MessageBoxButtonInfo
    {
        #region fields

        private string _contentText = "";
        private MessageBoxResult _result = MessageBoxResult.OK;
        private Action<object> _action = null;

        #endregion // fields

        #region ctor

        /// <summary>
        /// 初始化 MessageBox 自定义按钮的基本信息.
        /// </summary>
        /// <param name="contentText">按钮的文本内容</param>
        /// <param name="result">按钮响应的返回结果</param>
        /// <param name="action">按钮的响应动作</param>
        public MessageBoxButtonInfo(string contentText, MessageBoxResult result, Action<object> action)
        {
            this._contentText = contentText;
            this._result = result;
            if (null != action)
            {
                this._action = action;
            }
            else
            {
                this._action = new Action<object>((o) =>
                {

                });
            }
        }

        #endregion // ctor

        #region Readonly Properties

        /// <summary>
        /// 获取 MessageBox 按钮的文本内容.
        /// </summary>
        public string ContentText
        {
            get { return _contentText; }
        }

        /// <summary>
        /// 获取 MessageBox 按钮响应的返回结果.
        /// </summary>
        public MessageBoxResult Result
        {
            get { return _result; }
        }

        /// <summary>
        /// 获取 MessageBox 按钮的响应动作.
        /// </summary>
        public Action<object> Action
        {
            get { return _action; }
        }

        #endregion // Readonly Properties
    }

    /// <summary>
    /// MessageBoxModule 组件的自定义事件
    /// </summary>
    public class MessageBoxCommand : ICommand
    {
        #region Private Fields
        private readonly Action<object> _command;
        private readonly Func<object, bool> _canExecute;
        #endregion

        #region Constructor
        public MessageBoxCommand(Action<object> command, Func<object, bool> canExecute)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            _canExecute = canExecute;
            _command = command;
        }
        #endregion

        #region ICommand Members
        public void Execute(object parameter)
        {
            _command(parameter);
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        #endregion
    }

    /// <summary>
    /// 显示消息框.
    /// </summary>
    public sealed class MessageBox
    {
        #region ctors

        static MessageBox() { }
        private MessageBox() { }

        #endregion // ctors

        #region Show functions

        #region No Owner
        //No Owner

        public static MessageBoxResult ShowCurrentInstanceIsRunning()
        {
            MessageBoxModule.CurrentInstanceIsRunning = true;
            return MessageBoxModule.Show();
        }

        public static MessageBoxResult Show(string messageBoxText)
        {
            return MessageBoxModule.Show(messageBoxText);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return MessageBoxModule.Show(messageBoxText, caption);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxImageType imageType)
        {
            return MessageBoxModule.Show(messageBoxText, imageType);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxButton button)
        {
            return MessageBoxModule.Show(messageBoxText, button);
        }

        public static MessageBoxResult Show(string messageBoxText, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return MessageBoxModule.Show(messageBoxText, imageType, button);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImageType imageType)
        {
            return MessageBoxModule.Show(messageBoxText, caption, imageType);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return MessageBoxModule.Show(messageBoxText, caption, button);
        }

        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return MessageBoxModule.Show(messageBoxText, caption, imageType, button);
        }

        #endregion

        #region Owner

        //Owner
        public static MessageBoxResult Show(Window owner)
        {
            return MessageBoxModule.Show(owner);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText)
        {
            return MessageBoxModule.Show(owner, messageBoxText);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption)
        {
            return MessageBoxModule.Show(owner, messageBoxText, caption);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, MessageBoxImageType imageType)
        {
            return MessageBoxModule.Show(owner, messageBoxText, imageType);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, MessageBoxButton button)
        {
            return MessageBoxModule.Show(owner, messageBoxText, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return MessageBoxModule.Show(owner, messageBoxText, imageType, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxImageType imageType)
        {
            return MessageBoxModule.Show(owner, messageBoxText, caption, imageType);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxButton button)
        {
            return MessageBoxModule.Show(owner, messageBoxText, caption, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return MessageBoxModule.Show(owner, null, messageBoxText, caption, imageType, button);
        }

        public static MessageBoxResult Show(Window owner, string messageBoxText, string caption, MessageBoxImageType imageType, IList<MessageBoxButtonInfo> ctrlButtons)
        {
            return MessageBoxModule.Show(owner, null, messageBoxText, caption, imageType, ctrlButtons);
        }

        #endregion

        #region FrameworkElement

        //FrameworkElement
        public static MessageBoxResult Show(UserControl userControl)
        {
            return MessageBoxModule.Show(userControl);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText)
        {
            return MessageBoxModule.Show(userControl, messageBoxText);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption)
        {
            return MessageBoxModule.Show(userControl, messageBoxText, caption);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, MessageBoxImageType imageType)
        {
            return MessageBoxModule.Show(userControl, messageBoxText, imageType);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, MessageBoxButton button)
        {
            return MessageBoxModule.Show(userControl, messageBoxText, button);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return MessageBoxModule.Show(userControl, messageBoxText, imageType, button);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption, MessageBoxImageType imageType)
        {
            return MessageBoxModule.Show(userControl, messageBoxText, caption, imageType);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption, MessageBoxButton button)
        {
            return MessageBoxModule.Show(userControl, messageBoxText, caption, button);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption, MessageBoxImageType imageType, MessageBoxButton button)
        {
            return MessageBoxModule.Show(null, userControl, messageBoxText, caption, imageType, button);
        }

        public static MessageBoxResult Show(UserControl userControl, string messageBoxText, string caption, MessageBoxImageType imageType, IList<MessageBoxButtonInfo> ctrlButtons)
        {
            return MessageBoxModule.Show(null, userControl, messageBoxText, caption, imageType, ctrlButtons);
        }

        #endregion

        #endregion // Show functions
    }
}
