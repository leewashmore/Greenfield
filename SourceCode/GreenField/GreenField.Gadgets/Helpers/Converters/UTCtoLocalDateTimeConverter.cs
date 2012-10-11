using System;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts DateTime value from kind UTC to kind Local and vice versa
    /// </summary>
    public class UTCToLocalDateTimeConverter : IValueConverter
    {
        /// <summary>
        /// Converts DateTime value to kind Local
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Converts DateTime value to kind UTC
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
