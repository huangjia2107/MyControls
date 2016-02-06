using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyControl.Helper
{
    public class ToolsClass
    {
        //过滤数字中多余的“0”
        /***
         * 00022200.0002220000    =>    22200.000222
         * 00000.0000             =>    0
         * 00000.002323000        =>    0.002323
         * 00000233330000         =>    233330000
         */
        public static string FilterNeedlessZero(string numberStr)
        {
            double nResult = 0;
            if (!double.TryParse(numberStr, out nResult))
            {
                return numberStr;
            }

            if (numberStr.Contains('.') || numberStr.Contains(','))
                numberStr = Regex.Replace(numberStr, @"^0+(?=(,|.))|(?<=(,|.))0+$", "").TrimEnd(",.".ToCharArray());
            else
                numberStr = Regex.Replace(numberStr, @"^0+(?=[1-9]*)", "");

            return string.IsNullOrEmpty(numberStr) ? "0" : numberStr.IndexOf(',') == 0 || numberStr.IndexOf('.') == 0 ? "0" + numberStr : numberStr;
        }

        //过滤数字中多余的“0”
        /***
         * 00022200.0002220000    =>    22200.000222
         * 00000.0000             =>    0.0
         * 00000.002323000        =>    0.002323
         * 00000233330000         =>    233330000.0
         */
        public static string FilterNeedlessZeroWidthPoint(string numberStr, bool isInteger,string separatorStr)
        {
            double nResult = 0;
            if (!double.TryParse(numberStr, out nResult))
            {
                return numberStr;
            }
            //由于numberStr可能为"-.3"，转换之后，nResult=-0.3， 故此处做一矫正
            numberStr = nResult.ToString();
            
            string negativeSign = numberStr.Contains('-') && numberStr.IndexOf('-') == 0 && nResult != 0 ? "-" : string.Empty;
            string tempStr= numberStr.Contains('-') && numberStr.IndexOf('-') == 0 ? numberStr.Remove(0, 1) : numberStr;
            
            if (tempStr.Contains(separatorStr))
                tempStr = Regex.Replace(tempStr, @"^0+(?=" + separatorStr + ")|(?<=" + separatorStr + ")0+$", "");//@"^0+(?=(,|.))|(?<=(,|.))0+$", "");//.TrimEnd(",.".ToCharArray());
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

            return negativeSign + tempStr;
        }
        
        //string pattern = @"(([-]?\d+" + @"\"+SeparatorStr + @"\d+)|([-]?\d+))";
        public static string RegexReplace(string input, string pattern, Func<string, string> func)
        {
            MatchCollection mac = Regex.Matches(input, pattern, RegexOptions.ExplicitCapture);
            string result = input;

            foreach (Match m in mac)
            {
                Regex r = new Regex(@"(?<![\d.,])" + Regex.Escape(m.Value) + @"(?![\d.,])");
                result = r.Replace(result, func(m.Value), 1, result.IndexOf(m.Value));
            }

            return result;
        }
        
        public static string EnumToString(Enum e)
        {
            return Enum.GetName(e.GetType(), e);
        }

        public static T StringToEnum<T>(string enumValue)
        {
            return (T)Enum.Parse(typeof(T), enumValue);
        }
        
         //检查文件名是否已经包含数字序号后缀，返回该后缀
        public static string CheckNameNumber(string StrName)
        {
            int index1 = StrName.LastIndexOf('(');
            int index2 = StrName.LastIndexOf(')');
            if (index1 >= 0 && index2 == StrName.Length - 1 && index2 - index1 >= 2 && Regex.IsMatch(StrName.Substring(index1), @"^\({1}[1-9][\d]*\){1}$"))
            {
                return StrName.Substring(index1);
            }
            else
            {
                return null;
            }
        }

        public static string GetPropertyValue<T>(T obj, string propertyName)
        {
            return typeof(T).GetProperty(propertyName).GetValue(obj, null).ToString();
        }

        public static string GetNameWithNumber<T>(string OriginName, ObservableCollection<T> CompareCollection, string PropertyName)
        {
            string NewName = string.Empty;
            int i = 2;
            T CompareType = CompareCollection.FirstOrDefault(t => GetPropertyValue<T>(t, PropertyName) == OriginName);
            if (CompareType == null)
                return OriginName;
            else
            {
                string indexStr = CheckNameNumber(OriginName);
                while (true)
                { 
                    if (string.IsNullOrEmpty(indexStr))
                        NewName = OriginName + "(" + i + ")";
                    else
                        NewName = OriginName.Substring(0, OriginName.Length - indexStr.Length) + "(" + i + ")";

                    CompareType = CompareCollection.FirstOrDefault(t => GetPropertyValue<T>(t, PropertyName) == NewName);
                    if (CompareType == null)
                        break;

                    i++;
                }
            }
            return NewName;
        }

        //////////////////////////////////////////////////////////////////////////
        /// IntPtr ptr = GetCursor();
        /// if ((int)ptr != (int)CursorValue.Cross)
        ///    return;
        //////////////////////////////////////////////////////////////////////////
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetCursor();
    } 
    
    // .cs
    //IsShowTimeProperty  依赖性属性
    //static DataGridContextHelper dc = new DataGridContextHelper();  成员变量
    //this.DataContext = this;  构造函数中
    // .xaml
    //Visibility="{Binding (FrameworkElement.DataContext).IsShowTime,RelativeSource={x:Static RelativeSource.Self},Converter={StaticResource BTVConverter},ConverterParameter={x:Static Visibility.Visible}}"
    public class DataGridContextHelper
    {
        static DataGridContextHelper()
        {
            DependencyProperty dp = FrameworkElement.DataContextProperty.AddOwner(typeof(DataGridColumn));
            FrameworkElement.DataContextProperty.OverrideMetadata(typeof(DataGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnDataContextChanged)));
        }

        public static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGrid grid = d as DataGrid;
            if (grid != null)
            {
                foreach (DataGridColumn col in grid.Columns)
                {
                    col.SetValue(FrameworkElement.DataContextProperty, e.NewValue);
                }
            }
        }

    } 
    
    public class ScrollViewerBehavior
    {
        public static DependencyProperty VerticalOffsetProperty =
        DependencyProperty.RegisterAttached("VerticalOffset",
                                            typeof(double),
                                            typeof(ScrollViewerBehavior),
                                            new UIPropertyMetadata(0.0, OnVerticalOffsetChanged));

        public static void SetVerticalOffset(FrameworkElement target, double value)
        {
            target.SetValue(VerticalOffsetProperty, value);
        }
        public static double GetVerticalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(VerticalOffsetProperty);
        }
        private static void OnVerticalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToVerticalOffset((double)e.NewValue);
            }
        }

        public static DependencyProperty HorizontalOffsetProperty =
        DependencyProperty.RegisterAttached("HorizontalOffset",
                                            typeof(double),
                                            typeof(ScrollViewerBehavior),
                                            new UIPropertyMetadata(0.0, OnHorizontalOffsetChanged));

        public static void SetHorizontalOffset(FrameworkElement target, double value)
        {
            target.SetValue(HorizontalOffsetProperty, value);
        }
        public static double GetHorizontalOffset(FrameworkElement target)
        {
            return (double)target.GetValue(HorizontalOffsetProperty);
        }
        private static void OnHorizontalOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ScrollViewer scrollViewer = target as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset((double)e.NewValue);
            }
        }
    }
}
