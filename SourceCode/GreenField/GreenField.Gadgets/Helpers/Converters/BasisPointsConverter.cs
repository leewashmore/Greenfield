using System;
using System.Windows.Data;
using Telerik.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts basis points to a multiplier custom format text
    /// </summary>
    public class BasisPointsConverter : IValueConverter
    {
        /// <summary>
        /// Converts basis points to a multiplier custom format text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Decimal)
            {
                Decimal basisPoints = (Decimal)value * 10000;
                //return basis points with no decimal places
                return basisPoints.ToString("n0");
            }
            else if (value is AggregateResult)
            {
                Decimal basisPoints = (Decimal)((AggregateResult)value).Value * 10000;
                return basisPoints.ToString("n0");
            }
            return value;
        }

        /// <summary>
        /// Stub - no implementation
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