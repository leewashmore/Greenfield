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

namespace GreenField.Targeting.Only.BroadGlobalActive
{
    public class FormattedValueConverter : IValueConverter
    {
        public const String NoValue = "";
        public const String ValueFormat = "#,##0.000";

        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            var casted = (Decimal?)value;
            if (casted.HasValue)
            {
                return casted.Value.ToString(ValueFormat);
            }
            else
            {
                return NoValue;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            Decimal? result;
            var casted = value as String;
            if (String.IsNullOrEmpty(casted))
            {
                result = null;
            }
            else
            {
                Decimal parsed;
                if (Decimal.TryParse(casted, out parsed))
                {
                    result = parsed;
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }
    }
}
