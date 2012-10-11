using System;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Data;

namespace GreenField.Gadgets.Helpers.Converters
{
    /// <summary>
    /// Converts value of type AggregateResultCollection of grid to visibility
    /// </summary>
    public class GroupAggregationToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts value of type AggregateResultCollection of grid to visibility
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var collection = value as AggregateResultCollection;
            if (value == null)
                return Visibility.Visible;
            return collection.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
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
