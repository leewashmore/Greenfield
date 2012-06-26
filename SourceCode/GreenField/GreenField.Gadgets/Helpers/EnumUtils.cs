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
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;

namespace GreenField.Gadgets.Helpers
{
    public class EnumUtils
    {
        public static List<T> GetEnumDescriptions<T>()
        {
            Type type = typeof(T);

            if (!type.IsEnum)
                throw new ArgumentException("Type ‘" + type.Name + "’ is not an enum");

            FieldInfo[] fields = type.GetFields();
            List<T> result = new List<T>();
            foreach (FieldInfo field in fields)
            {
                if (field.IsLiteral)
                {
                    object value = field.GetValue(type);
                    result.Add((T)value);
                }
            }

            return result;
        }        
    }

    public class EnumDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            DescriptionAttribute[] descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (descriptionAttributes != null && descriptionAttributes.Length > 0)
                return descriptionAttributes[0].Description;
            return fieldInfo.GetValue(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }    
}
