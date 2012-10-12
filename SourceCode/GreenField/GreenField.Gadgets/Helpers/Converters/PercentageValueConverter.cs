using System;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Converts value to a 2 decimal place percentage text
    /// </summary>
    public class PercentageValueConverter : IValueConverter
    {
        /// <summary>
        /// Converts value to a 2 decimal place percentage text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n2}%", value);            
        }

        /// <summary>
        /// Converts 2 decimal place percentage text to double value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
            {
                return result;
            }
            else
            {
                return null;
            }
        }        
    }

    /// <summary>
    /// Converts value to a 1 decimal place percentage text
    /// </summary>
    public class PercentageValueConverterOneDec : IValueConverter
    {
        /// <summary>
        /// Converts value to a 1 decimal place percentage text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n1}%", value);
        }

        /// <summary>
        /// Converts 1 decimal place percentage text to double value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Converts value to a 4 decimal place percentage text
    /// </summary>
    public class PercentageValConverterFourDec : IValueConverter
    {
        /// <summary>
        /// Converts value to a 4 decimal place percentage text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n4}%", value);
        }

        /// <summary>
        /// Converts 4 decimal place percentage text to double value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Converts value to a 2 decimal place percentage text
    /// </summary>
    public class PercentageValConverterTwoDec : IValueConverter
    {
        /// <summary>
        /// Converts value to a 2 decimal place percentage text
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? null : String.Format("{0:n2}%", value);
        }

        /// <summary>
        /// Converts 2 decimal place percentage text to double value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double result;
            bool parseValidation = double.TryParse((value as string).Replace("%", ""), out result);
            if (parseValidation)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
