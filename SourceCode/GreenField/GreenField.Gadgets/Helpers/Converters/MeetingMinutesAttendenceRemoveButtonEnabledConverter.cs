using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Collections.Generic;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.Helpers
{
    public class MeetingMinutesAttendenceRemoveButtonEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MeetingMinuteData)
            {
                MeetingMinuteData contextValue = value as MeetingMinuteData;
                return contextValue.VoterID == null;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
