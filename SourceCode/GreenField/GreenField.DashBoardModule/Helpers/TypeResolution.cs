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
using System.Windows.Resources;
using System.Reflection;

namespace GreenField.DashBoardModule.Helpers
{
    public static class TypeResolution
    {
        #region Get Dynamic Type
        /// <summary>
        /// Evaluates Type of className specified within deployed current assemblies
        /// </summary>
        /// <param name="className">class name with complete namespace</param>
        /// <returns>class Type if exists in project else null</returns>
        public static Type GetAssemblyType(string className)
        {
            if (String.IsNullOrEmpty(className))
                throw new ArgumentNullException();

            Type type = null;
            foreach (AssemblyPart part in Deployment.Current.Parts)
            {
                type = GetSpecificAssemblyType(part.Source, className);
                if (type != null)
                    break;
            }
            return type;
        }

        /// <summary>
        /// Evaluates Type of className specified within specific assembly
        /// </summary>
        /// <param name="assemblyName">assembly name with complete namespace</param>
        /// <param name="className">class name with complete namespace</param>
        /// <returns>class Type if exists in assembly else null</returns>
        public static Type GetSpecificAssemblyType(string assemblyName, string className)
        {
            if (String.IsNullOrEmpty(className) || String.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException();

            Type type = null;

            StreamResourceInfo info = Application.GetResourceStream(new Uri(assemblyName, UriKind.Relative));
            Assembly assembly = new AssemblyPart().Load(info.Stream);
            type = assembly.GetType(className);

            return type;
        }
        #endregion

        #region Get Assembly of Dynamic Type
        /// <summary>
        /// Evaluates Assembly of className specified within deployed current assemblies
        /// </summary>
        /// <param name="className">class name with complete namespace</param>
        /// <returns>class assembly if exists in project else null</returns>
        public static Assembly GetAssembly(String className)
        {
            if (String.IsNullOrEmpty(className))
                throw new ArgumentNullException();

            Assembly assembly = null;

            foreach (AssemblyPart part in Deployment.Current.Parts)
            {
                assembly = GetSpecificAssembly(part.Source, className);
                if (assembly != null)
                    break;
            }

            return assembly;
        }

        /// <summary>
        /// Evaluates Assembly of className specified within specific assembly
        /// </summary>
        /// <param name="assemblyName">assembly name with complete namespace</param>
        /// <param name="className">class name with complete namespace</param>
        /// <returns>Assembly of class name if exists else null</returns>
        public static Assembly GetSpecificAssembly(string assemblyName, string className)
        {
            if (String.IsNullOrEmpty(className) || String.IsNullOrEmpty(assemblyName))
                throw new ArgumentNullException();

            Assembly assembly = null;

            StreamResourceInfo info = Application.GetResourceStream(new Uri(assemblyName, UriKind.Relative));
            assembly = new AssemblyPart().Load(info.Stream);
            Type type = assembly.GetType(className);
            if (type == null)
                assembly = null;

            return assembly;
        } 
        #endregion

        #region Get object of Dyanamic Type
        /// <summary>
        /// Get object's Type FullName
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>Type name</returns>
        public static string GetTypeFullName(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException();
            return obj.GetType().FullName;
        }

        /// <summary>
        /// Get Type FullName for object's DataContext property
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>Type Name if property exists else empty string</returns>
        public static string GetTypeDataContextFullName(object obj)
        {
            try
            {
                if (obj == null)
                    throw new ArgumentNullException();
                return obj.GetType().GetProperty("DataContext").GetValue(obj, new object[] { }).ToString();
            }
            catch (NullReferenceException)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates class object of specific type that takes single argument in it's constructor
        /// </summary>
        /// <param name="type">Type of class</param>
        /// <param name="argumentType">Type of argument</param>
        /// <param name="argumentValue">Value of argument</param>
        /// <returns>class object; null if overloaded constructor not found</returns>
        public static object GetNewTypeObject(Type type, Type[] argumentTypes, object[] argumentValues)
        {
            try
            {
                if (type == null || argumentTypes == null || argumentValues == null)
                    throw new ArgumentNullException();
                return type.GetConstructor(argumentTypes).Invoke(argumentValues);                
            }
            catch
            {
                return default(object);
            }
        } 
        #endregion
    }
}
