using System;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts Market Capitalization value to display custom format
    /// </summary>
    public class MarketCapitalizationConverter : IValueConverter
    {
        /// <summary>
        /// Converts Market Capitalization value to display custom format
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
                Decimal convValue;
                if (Decimal.TryParse(value.ToString(), out convValue) == false)
                    return value;
                String result = String.Format("$ {0:n0} mn", convValue);
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
