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
using System.Linq;
using GreenField.ServiceCaller.MeetingDefinitions;


namespace GreenField.Gadgets.Helpers
{
    public class MarketCapitalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                Decimal convValue;
                if (Decimal.TryParse(value.ToString(), out convValue) == false)
                    return value;
                String result = String.Format("${0:n4}mn", convValue / Decimal.Parse("1000000"));
                return result;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
