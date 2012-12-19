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
using System.Linq;

namespace GreenField.Targeting.Controls
{
    public class BusyIndicatorTemplateSelector : IValueConverter
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }
        public DataTemplate LoadingTemplate { get; set; }
        public DataTemplate IssuesTemplate { get; set; }
        public DataTemplate LoadedTemplate { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            DataTemplate result = null;
            var content = value as ICommunicationStateModel;
            var resolver = new Convert_IIndicatorContentResolver(this);
            content.Accept(resolver);
            result = resolver.Result;
            return result ?? this.DefaultTemplate;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private class Convert_IIndicatorContentResolver : ICommunicationStateModelResolver
        {
            private BusyIndicatorTemplateSelector selector;

            public Convert_IIndicatorContentResolver(BusyIndicatorTemplateSelector selector)
            {
                this.selector = selector;
            }

            public DataTemplate Result { get; private set; }

            public void Resolve(ErrorCommunicationStateModel content)
            {
                this.Result = this.selector.ErrorTemplate;
            }

            public void Resolve(LoadingCommunicationStateModel content)
            {
                this.Result = this.selector.LoadingTemplate;
            }

            public void Resolve(IssuesCommunicationStateModel content)
            {
                this.Result = this.selector.IssuesTemplate;
            }

            public void Resolve(LoadedCommunicationStateModel content)
            {
                this.Result = this.selector.LoadedTemplate;
            }
        }
    }
}
