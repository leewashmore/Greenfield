using System.Collections.Generic;
using System.Reflection;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// List Utility features
    /// </summary>
    public static class ListUtils
    {
        /// <summary>
        /// Creation of deep copy of a single dimension list
        /// </summary>
        /// <typeparam name="T">generic type</typeparam>
        /// <param name="list">input list</param>
        /// <returns>deep copy list</returns>
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
