using System;
using System.Windows.Data;
using GreenField.Common;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts ic presentation modify vote value selection to return enabled value
    /// </summary>
    public class MeetingDecisionEntryPFVBuySellInputEnabledConverter : IValueConverter
    {
        /// <summary>
        /// Converts ic presentation modify vote value selection to return enabled value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value as String == VoteType.MODIFY)
            {
                return true;
            }
            return false;
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
