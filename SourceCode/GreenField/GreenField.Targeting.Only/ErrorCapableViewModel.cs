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
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel;

namespace GreenField.Targeting.Only
{
    /// <summary>
    /// Is capable of reporting errors.
    /// </summary>
    public class ErrorCapableViewModel : NotificationObject
    {
        protected void TakeCareOfResult<TResult, TValue>(String signature, TResult e, Func<TResult, TValue> getter, Action<TValue> callback)
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
    }
}
