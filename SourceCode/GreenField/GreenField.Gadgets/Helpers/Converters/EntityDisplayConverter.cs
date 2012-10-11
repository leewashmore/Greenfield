using System;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts value to display '{N/A}' if null or empty string
    /// </summary>
    public class EntityDisplayConverter : IValueConverter
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
                    return "{N/A}";

            if (value is String)
            {
                String result = value as String;
                if (result.Trim() == String.Empty)
                    return "{N/A}";
            }
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
