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
using GreenField.Common;

namespace GreenField.Gadgets.Helpers
{
    public class UTCToLocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime?)
            {
                if (value == null)
                    return null;
                DateTime dateTime = new DateTime(((DateTime)value).Ticks, DateTimeKind.Utc);

                if ((DateTime.Now - DateTime.UtcNow) != TimeZoneInfo.Local.BaseUtcOffset)
                {
                    return dateTime.Add(DateTime.Now - DateTime.UtcNow);
                }
                return dateTime.Add(TimeZoneInfo.Local.BaseUtcOffset);
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime?)
            {
                if (value == null)
                    return null;
                DateTime dateTime = new DateTime(((DateTime)value).Ticks, DateTimeKind.Local);

                return dateTime.ToUniversalTime();
            }
            return value;
        }
    }
}
