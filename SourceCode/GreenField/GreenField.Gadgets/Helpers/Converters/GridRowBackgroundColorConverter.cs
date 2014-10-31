using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace GreenField.Gadgets.Helpers
{
    public class GridRowBackgroundColorConverter : IValueConverter 
    {

        /// <summary>
        /// Converts list of FileMaster based on ConverterParameter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Green;
                return brush;
            }
            else
            {
                return null;
            }
           
        }

        /// <summary>
        /// Stub - No implementation
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

    }
}
