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

namespace GreenField.Targeting.Only.Backend.Targeting
{
    public partial class ExpressionModel : INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public System.Collections.IEnumerable GetErrors(String propertyName)
        {
            return this.Issues.Select(x => x.Message);
        }

        public Boolean HasErrors
        {
            get { return this.Issues.Any(); }
        }
    }
}
