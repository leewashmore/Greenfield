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

namespace GreenField.Targeting.Only
{
    public class RuntimeHelper
    {
        public static TValue DataContextAs<TValue>(FrameworkElement element) where TValue: class
        {
            try
            {
                if (element.DataContext == null) throw new ApplicationException("DataContext is null.");
                var casted = element.DataContext as TValue;
                if (casted == null) throw new ApplicationException("Value of the DataContext property is of the unexpected \"" + casted.GetType().Name + "\" type.");
                return casted;
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to get a value of the DataContext property of the \"" + element.Name + "\" element of the type \"" + element.GetType().Name + "\" type as \"" + typeof(TValue).Name + "\". Reason: " + exception.Message, exception);
            }
        }
    }
}
