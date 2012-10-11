using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts value to IEnumerable list of unioned items
    /// </summary>
    public class FlattenGroupConverter : IValueConverter
    {
        /// <summary>
        /// Converts value to IEnumerable list of unioned items
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value as IEnumerable<object>).SelectMany(g => new object[] { g }.Union((g as CollectionViewGroup).Items));
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
