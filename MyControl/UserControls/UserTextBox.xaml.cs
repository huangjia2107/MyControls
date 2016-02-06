using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyControl.Helper;

namespace MyControl.UserControls
{
    /// <summary>
    /// Interaction logic for UserTextBox.xaml
    /// </summary>
    public partial class UserTextBox : UserControl
    {
        string SeparatorStr = ".";
        string oldValue = null; 

        public UserTextBox()
        {
            InitializeComponent();

            SeparatorStr = IsDecimalSeparatorComma() ? "," : ".";

            // 处理粘贴
            DataObject.AddPastingHandler(this.inputTextBox, new DataObjectPastingEventHandler(TextBox_Paste));

            //this.SetBinding(UserTextBox.IsTouchProperty, new Binding("IsTouch") { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1) });

            inputTextBox.SetBinding(TextBox.AllowDropProperty, new Binding("TextBoxAllowDrop") { Source = this });
            inputTextBox.SetBinding(TextBox.WidthProperty, new Binding("TextBoxWidth") { Source = this });
            inputTextBox.SetBinding(TextBox.MaxLengthProperty, new Binding("TextBoxMaxLength") { Source = this });
            inputTextBox.SetBinding(TextBox.BorderThicknessProperty, new Binding("TextBoxBorderThickness") { Source = this });
            inputTextBox.SetBinding(TextBox.IsReadOnlyProperty, new Binding("IsReadOnly") { Source = this });
            inputTextBox.SetBinding(TextBox.HorizontalContentAlignmentProperty, new Binding("TextBoxContentAlignment") { Source = this });
            unitTextBlock.SetBinding(TextBlock.TextProperty, new Binding("Units") { Source = this });

            inputTextBox.Text = "0";
        }

        private event EventHandler _TextChanged;
        public event EventHandler TextChanged
        {
            add { _TextChanged += value; }
            remove { _TextChanged -= value; }
        }

        public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.Register("AlignmentX", typeof(AlignmentX), typeof(UserTextBox), new UIPropertyMetadata(AlignmentX.Left));
        public AlignmentX AlignmentX
        {
            get { return (AlignmentX)GetValue(AlignmentXProperty); }
            set { SetValue(AlignmentXProperty, value); }
        }
        
        public static readonly DependencyProperty BgTextProperty = DependencyProperty.Register("BgText", typeof(string), typeof(UserTextBox));
        public string BgText
        {
            get { return (string)GetValue(BgTextProperty); }
            set { SetValue(BgTextProperty, value); }
        }

        public static readonly DependencyProperty TextBoxContentAlignmentProperty = DependencyProperty.Register("TextBoxContentAlignment", typeof(HorizontalAlignment), typeof(UserTextBox), new PropertyMetadata(HorizontalAlignment.Right));
        public HorizontalAlignment TextBoxContentAlignment
        {
            get { return (HorizontalAlignment)GetValue(TextBoxContentAlignmentProperty); }
            set { SetValue(TextBoxContentAlignmentProperty, value); }
        }

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(UserTextBox));
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(UserTextBox), new PropertyMetadata("0", new PropertyChangedCallback(TextPropertyChangedCallback)));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        static void TextPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserTextBox obj = sender as UserTextBox;
            obj.inputTextBox.Text = obj.Text;
            obj.ProcessUserInputData();
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(UserTextBox), new PropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(MaxValuePropertyChangedCallback)));
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        static void MaxValuePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserTextBox obj = sender as UserTextBox;

            obj.Dispatcher.BeginInvoke((Action)(() =>
                {
                    obj.ProcessUserInputData();
                }));
        }

        public static readonly DependencyProperty MiniValueProperty = DependencyProperty.Register("MiniValue", typeof(double), typeof(UserTextBox), new PropertyMetadata(double.NegativeInfinity, new PropertyChangedCallback(MiniValuePropertyChangedCallback)));
        public double MiniValue
        {
            get { return (double)GetValue(MiniValueProperty); }
            set { SetValue(MiniValueProperty, value); }
        }

        static void MiniValuePropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserTextBox obj = sender as UserTextBox;

            obj.Dispatcher.BeginInvoke((Action)(() =>
            {
                obj.ProcessUserInputData();
            }));
        }

        public static readonly DependencyProperty TextBoxAllowDropProperty = DependencyProperty.Register("TextBoxAllowDrop", typeof(bool), typeof(UserTextBox), new UIPropertyMetadata(true));
        public bool TextBoxAllowDrop
        {
            get { return (bool)GetValue(TextBoxAllowDropProperty); }
            set { SetValue(TextBoxAllowDropProperty, value); }
        }

        public static readonly DependencyProperty TextBoxBorderThicknessProperty = DependencyProperty.Register("TextBoxBorderThickness", typeof(Thickness), typeof(UserTextBox), new UIPropertyMetadata(new Thickness(0.7, 0.7, 0.7, 0.7)));
        public Thickness TextBoxBorderThickness
        {
            get { return (Thickness)GetValue(TextBoxBorderThicknessProperty); }
            set { SetValue(TextBoxBorderThicknessProperty, value); }
        }

        public static readonly DependencyProperty TextBoxWidthProperty = DependencyProperty.Register("TextBoxWidth", typeof(double), typeof(UserTextBox), new PropertyMetadata(70.0));
        [TypeConverter(typeof(LengthConverter))]
        public double TextBoxWidth
        {
            get { return (double)GetValue(TextBoxWidthProperty); }
            set { SetValue(TextBoxWidthProperty, value); }
        }

        public static readonly DependencyProperty TextBoxMaxLengthProperty = DependencyProperty.Register("TextBoxMaxLength", typeof(int), typeof(UserTextBox), new UIPropertyMetadata(0));
        public int TextBoxMaxLength
        {
            get { return (int)GetValue(TextBoxMaxLengthProperty); }
            set { SetValue(TextBoxMaxLengthProperty, value); }
        }

        public static readonly DependencyProperty IsIntegerProperty = DependencyProperty.Register("IsInteger", typeof(bool), typeof(UserTextBox), new UIPropertyMetadata(true, new PropertyChangedCallback(IsIntegerPropertyChangedCallback)));
        public bool IsInteger
        {
            get { return (bool)GetValue(IsIntegerProperty); }
            set { SetValue(IsIntegerProperty, value); }
        }

        static void IsIntegerPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserTextBox obj = sender as UserTextBox;
            obj.ProcessUserInputData();
        }

        public static readonly DependencyProperty IsPositiveProperty = DependencyProperty.Register("IsPositive", typeof(bool), typeof(UserTextBox), new PropertyMetadata(true));
        public bool IsPositive
        {
            get { return (bool)GetValue(IsPositiveProperty); }
            set { SetValue(IsPositiveProperty, value); }
        }

        public static readonly DependencyProperty UnitsProperty = DependencyProperty.Register("Units", typeof(string), typeof(UserTextBox), new PropertyMetadata(string.Empty, new PropertyChangedCallback(UnitsPropertyChangedCallback)));
        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }
        static void UnitsPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserTextBox obj = sender as UserTextBox;
            if (string.IsNullOrEmpty(obj.Units))
                obj.unitTextBlock.Visibility = Visibility.Collapsed;
            else
                obj.unitTextBlock.Visibility = Visibility.Visible;
        }

        public static readonly DependencyProperty IsTouchProperty = DependencyProperty.Register("IsTouch", typeof(bool), typeof(UserTextBox), new PropertyMetadata(false));
        public bool IsTouch
        {
            get { return (bool)GetValue(IsTouchProperty); }
            set { SetValue(IsTouchProperty, value); }
        }

        public static bool IsDecimalSeparatorComma()
        {
            return string.Format("{0:f}", 3.14).Contains(',');
        }

        // 处理粘贴  
        private void TextBox_Paste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string value = e.DataObject.GetData(typeof(string)).ToString().Trim();
                if (CheckPastingText(value) == true)
                {
                    inputTextBox.Text = value;
                    Text = value;
                    oldValue = value;

                    this.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (_TextChanged != null)
                            _TextChanged(this, new EventArgs());
                    }));
                }
            }
            e.CancelCommand();
        }

        private bool CheckPastingText(string inputText)
        {
            if (!Regex.IsMatch(inputText, @"^(([-]?\d+" + SeparatorStr + @"\d+)|([-]?\d+))$"))
            {
                return false;
            }

            if (IsInteger == true)
            {
                if (!Regex.IsMatch(inputText, @"^[-]?\d+$"))
                {
                    return false;
                }
            }

            return true;
        }

        //过滤数字中多余的“0”
        private string FilterNeedlessZero(string numberStr, bool isInteger, string separatorStr)
        {
            double nResult = 0;
            if (!double.TryParse(numberStr, out nResult))
            {
                return numberStr;
            }
            //由于numberStr可能为"-.3"，转换之后，nResult=-0.3， 故此处做一矫正
            numberStr = nResult.ToString();

            string negativeSign = numberStr.Contains('-') && numberStr.IndexOf('-') == 0 && nResult != 0 ? "-" : string.Empty;
            string tempStr = numberStr.Contains('-') && numberStr.IndexOf('-') == 0 ? numberStr.Remove(0, 1) : numberStr;

            if (tempStr.Contains(separatorStr))
                tempStr = Regex.Replace(tempStr, @"^0+(?=" + separatorStr + ")|(?<=" + separatorStr + ")0+$", "");//.TrimEnd(",.".ToCharArray());
            else
                tempStr = Regex.Replace(tempStr, @"^0+(?=[1-9]*)", "");

            if (string.IsNullOrEmpty(tempStr))
                tempStr = isInteger == true ? "0" : "0" + separatorStr + "0";

            if (!tempStr.Contains(separatorStr))
                tempStr = isInteger == true ? negativeSign + tempStr : negativeSign + tempStr + separatorStr + "0";

            if (tempStr.IndexOf(separatorStr) == 0)
                tempStr = negativeSign + "0" + tempStr;

            if (tempStr.LastIndexOf(separatorStr) == tempStr.Length - 1)
                tempStr = negativeSign + tempStr + "0";

            return tempStr.Contains(negativeSign) ? tempStr : negativeSign + tempStr;
        }


        void ProcessUserInputData()
        {
            if (string.IsNullOrEmpty(inputTextBox.Text.Trim()))
            {
                inputTextBox.Text = oldValue;
                return;
            }

            double result = 0;
            if (!double.TryParse(inputTextBox.Text, out result))
            {
                inputTextBox.Text = oldValue;
            }
            else
                inputTextBox.Text = IsInteger ? inputTextBox.Text : result.ToString();//string.Format("{0:#0.0#}", 

            //过滤输入字符串格式
            inputTextBox.Text = FilterNeedlessZero(inputTextBox.Text, IsInteger, SeparatorStr);

             if (!double.IsPositiveInfinity(MaxValue))
            {
                if (result > MaxValue)
                    inputTextBox.Text = Convert.ToString(MaxValue);
            }

            if (!double.IsNegativeInfinity(MiniValue))
            {
                if (result < MiniValue)
                    inputTextBox.Text = Convert.ToString(MiniValue);
            }

            Text = inputTextBox.Text;
        }

        private void inputText_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //避免Ctrl+字符键 或 Alt+字符键 会输入成 空串 造成异常
            if (e.Text.Length != 1)
                return;

            TextBox textBox = sender as TextBox;
            char ch = char.Parse(e.Text);

            if (ch != '-' && ch != char.Parse(SeparatorStr) && ch < '0' || ch > '9')
            {
                e.Handled = true;
                return;
            }

            if (IsInteger == true && ch == char.Parse(SeparatorStr) || IsPositive == true && ch == '-')
            {
                e.Handled = true;
                return;
            }

            string tempStr = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectedText.Length);
            if ((tempStr.Contains('-') || textBox.CaretIndex > 0 || tempStr == SeparatorStr) && ch == '-')
            {
                e.Handled = true;
                return;
            }

            if (tempStr.Contains('-') && textBox.CaretIndex == 0)
            {
                e.Handled = true;
                return;
            }

            if (tempStr.Contains(SeparatorStr) && ch == char.Parse(SeparatorStr))
            {
                e.Handled = true;
                return;
            }

            if (tempStr.Contains('-') && ch == char.Parse(SeparatorStr) && textBox.CaretIndex == 1)
            {
                e.Handled = true;
                return;
            }
        }

        private void inputText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (e.Key == Key.Enter)
            {
                ProcessUserInputData();
                this.Focus();

                if (_TextChanged != null && oldValue != Text)
                {
                    oldValue = Text;
                    _TextChanged(this, new EventArgs());
                }
            }

            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        private void inputText_LostFocus(object sender, RoutedEventArgs e)
        {
            //             if (IsTouch == true)
            //             {
            //                 ScreenKeyboardCtr.HideInputPanel();
            //             }

            ProcessUserInputData();
            if (_TextChanged != null && oldValue != Text)
            {
                oldValue = Text;
                _TextChanged(this, new EventArgs());
            }
        }

        private void inputText_GotFocus(object sender, RoutedEventArgs e)
        {
            //             if (IsTouch == true && IsReadOnly == false)
            //             {
            //                 ScreenKeyboardCtr.ShowInputPanel();
            //             }

            oldValue = Text;
            inputTextBox.SelectAll();
        }

        private void inputTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //             if (IsTouch == true && IsReadOnly == false)
            //             {
            //                 ScreenKeyboardCtr.ShowInputPanel();
            //             }

            if (inputTextBox.IsKeyboardFocusWithin == false)
            {
                e.Handled = true;
                inputTextBox.Focus();
            }
        }
    }
}

