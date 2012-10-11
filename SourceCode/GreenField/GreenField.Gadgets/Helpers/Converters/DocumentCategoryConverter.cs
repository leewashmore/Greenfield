using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using GreenField.ServiceCaller.MeetingDefinitions;


namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Screens list of FileMaster based on ConverterParameter
    /// </summary>
    public class DocumentCategoryConverter : IValueConverter
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
            if (value is List<FileMaster> && parameter is String)
            {
                List<FileMaster> result = (value as List<FileMaster>).Where(record => record.Category == parameter as String).ToList();
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
