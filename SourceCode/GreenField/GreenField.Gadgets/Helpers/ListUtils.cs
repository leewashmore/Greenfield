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

namespace GreenField.Gadgets.Helpers
{
    public static class ListUtils
    {
        public static List<T> GetDeepCopy<T>(List<T> list) where T: new()
        {
            List<T> result = new List<T>();
            foreach (T item in list)
            {
                T resultNode = new T();
                PropertyInfo[] propInfo = typeof(T).GetProperties();
                foreach (PropertyInfo prop in propInfo)
                {
                    if (prop.Name != "EntityKey")
                    {
                        var value = prop.GetValue(item, null);
                        prop.SetValue(resultNode, value, null); 
                    }
                }

                result.Add(resultNode);
            }

            return result;
        }
    }
}
