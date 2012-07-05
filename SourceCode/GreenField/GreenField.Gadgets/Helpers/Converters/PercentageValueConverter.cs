using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    public class PercentageValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n2}%", value);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
                return result;
            else
                return null;           
        }        
    }
    public class PercentageValueConverterOneDec : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n1}%", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
                return result;
            else
                return null;
        }
    }
    public class PercentageValConverterFourDec : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n4}%", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
                return result;
            else
                return null;
        }
    }


}
