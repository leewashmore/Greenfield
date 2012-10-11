using System;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts value to display 'No Content' if null
    /// </summary>
    public class DraggedItemTemplateConverter : IValueConverter
    {
        /// <summary>
        /// Converts value to display 'No Content' if null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return "No Content";
            return value;
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
            return value;
        }
    }
}
