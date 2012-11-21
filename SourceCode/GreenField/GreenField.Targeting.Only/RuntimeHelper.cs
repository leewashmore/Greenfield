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
using System.ComponentModel;

namespace GreenField.Targeting.Only
{
    public class RuntimeHelper
    {
        public static void TakeCareOfResult<TResult, TValue>(String signature, TResult e, Func<TResult, TValue> getter, Action<TValue> callback)
            where TResult : AsyncCompletedEventArgs
        {
            try
            {
                if (e.Cancelled) throw new ApplicationException("The request to the backend service was cancelled.");
                if (e.Error != null) throw new ApplicationException("There was en error on the server side: " + e.Error.Message, e.Error);
                var value = getter(e);
                try
                {
                    callback(value);
                }
                catch (Exception exception)
                {
                    throw new ApplicationException("Unable to process the a valid result. Reason: " + exception.Message, exception);
                }
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to complete the \"" + signature + "\" request. Reason: " + exception.Message, exception);
            }
        }

        public static TValue TryDataContextAs<TValue>(FrameworkElement element) where TValue : class
        {
            return element.DataContext as TValue;
        }

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

        public static String GetViewName(UserControl view)
        {
            return view.GetType().FullName;
        }

        public static String GetViewName<TView>()
            where TView : UserControl
        {
            return typeof(TView).FullName;
        }
    }
}
