using System;
using System.Collections.Generic;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts currency value to be diplayed in a custom format
    /// </summary>
    public class ReportedCurrencyConverter : IValueConverter
    {
        /// <summary>
        /// Converts currency value to be diplayed in a custom format
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is List<String>)
            {
                List<String> result = new List<String>();
                foreach (String item in value as List<String>)
                {
                    result.Add("Reported Currency (" + item + ")"); 
                }
                return result;
            }
            return null;
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
