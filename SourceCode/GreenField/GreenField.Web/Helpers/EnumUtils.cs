using System;
using System.ComponentModel;
using System.Reflection;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Enum related operations
    /// </summary>
    public class EnumUtils
    {
        /// <summary>
        /// Converts enumeration value to it's description attribute
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>description attribute</returns>
        public static string ToString(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// converts a string value to enum matching it's description attribute
        /// </summary>
        /// <param name="value">value</param>
        /// <param name="enumType">enum type</param>
        /// <returns>enum</returns>
        public static object ToEnum(string value, Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (ToString((Enum)Enum.Parse(enumType, name)).Equals(value))
                {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }
    }
}