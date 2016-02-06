using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging; 

namespace MyControl
{
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class StringToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string uri = (string)value;

            if (uri == "")
                return null;

            BitmapImage bitmapImage = new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TextBoxDoubleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = "0.0";
            if (value is double)
                strValue = ((double)value).ToString("#0.0#");
            else if (value is float)
                strValue = ((float)value).ToString("#0.0#");
            return strValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            double dbResult = 0.0;
            if (double.TryParse(strValue, out dbResult) == true)
                return dbResult;
            else
                return 0.0;
        }
    }

    public class PointerCenterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dblVal = (double)value;
            TransformGroup tg = new TransformGroup();
            RotateTransform rt = new RotateTransform();
            TranslateTransform tt = new TranslateTransform();

            tt.X = dblVal / 2;
            tg.Children.Add(rt);
            tg.Children.Add(tt);

            return tg;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultipleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double dblVal = (double)value;
            double multiple = System.Convert.ToDouble(parameter);
            return dblVal * multiple;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class DataGridRowNumberMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = values[0];
            var items = values[1] as ItemCollection;

            var index = items.IndexOf(item);
            return (index + 1).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
