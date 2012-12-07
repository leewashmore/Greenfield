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

namespace GreenField.Targeting.Controls
{
    /// <summary>
    /// Converts the nesting depth level of regions in the broad active global view to the left-padding, so that the regions are indented according their position in the hierarchy.
    /// </summary>
    public class IntToLeftPaddingConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            var level = System.Convert.ToInt32(value);
            var amplitude = System.Convert.ToInt32(parameter);
            var indent = level * amplitude;
            if (indent < 0)
            {
                indent = 0;
            }
            var result = new Thickness(indent, 0, 0, 0);
            return result;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
