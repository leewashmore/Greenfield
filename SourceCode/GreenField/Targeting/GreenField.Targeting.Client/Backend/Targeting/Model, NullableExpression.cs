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
using System.Linq;

namespace TopDown.FacingServer.Backend.Targeting
{
    public partial class NullableExpressionModel : INotifyDataErrorInfo, IExpressionModel
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(String propertyName)
        {
            return ValidationHelper.GetErrors(propertyName, "Value", this);
        }

        public Boolean HasErrors
        {
            get { return this.Issues.Any(); }
        }
    }
}
