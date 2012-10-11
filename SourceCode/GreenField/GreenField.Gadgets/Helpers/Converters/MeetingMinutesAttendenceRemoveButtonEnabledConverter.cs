using System;
using System.Windows.Data;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts MeetingMinuteData value to Meeting Minutes Attendence Remove Button enabled by allowing non-voting members to be removed only
    /// </summary>
    public class MeetingMinutesAttendenceRemoveButtonEnabledConverter : IValueConverter
    {
        /// <summary>
        /// Converts MeetingMinuteData value to Meeting Minutes Attendence Remove Button enabled by allowing non-voting members to be removed only
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MeetingMinuteData)
            {
                MeetingMinuteData contextValue = value as MeetingMinuteData;
                return contextValue.VoterID == null;
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
