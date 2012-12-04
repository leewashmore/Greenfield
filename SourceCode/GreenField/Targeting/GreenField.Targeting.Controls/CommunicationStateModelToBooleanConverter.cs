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
using System.Windows.Data;

namespace GreenField.Targeting.Controls
{
    public class CommunicationStateModelToBooleanConverter : IValueConverter
    {

        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            Boolean result;

            var model = value as ICommunicationStateModel;
            var method = new ConvertMultiMethod();

            model.Accept(method);
            result = method.Result;

            return result;
        }

        private class ConvertMultiMethod : ICommunicationStateModelResolver
        {
            public Boolean Result { get; private set; }

            public void Resolve(ErrorCommunicationStateModel content)
            {
                this.Result = true;
            }

            public void Resolve(LoadingCommunicationStateModel content)
            {
                this.Result = true;
            }

            public void Resolve(IssuesCommunicationStateModel content)
            {
                this.Result = true;
            }

            public void Resolve(LoadedCommunicationStateModel content)
            {
                this.Result = false;
            }
        }


        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
