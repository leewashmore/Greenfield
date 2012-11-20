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

namespace GreenField.Targeting.Only
{
    /// <summary>
    /// Converts numbers like 0.12345 to 12.345
    /// </summary>
    public class FractionToPercentConverter : IValueConverter
    {
        public const String EmptyValue = "";
        public const String ValueFormat = "#0.000";
        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            String result;
            if (value == null)
            {
                result = EmptyValue;
            }
            else
            {
                var input = System.Convert.ToDecimal(value);
                var output = Math.Round(input * 100000.0m) / 100000.0m * 100.00m;
                result = output.ToString(ValueFormat);
            }
            return result;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
