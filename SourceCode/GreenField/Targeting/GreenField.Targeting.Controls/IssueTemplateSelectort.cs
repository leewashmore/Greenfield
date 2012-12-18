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
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.Generic;

namespace GreenField.Targeting.Controls
{
    public class IssueTemplateSelector : IValueConverter
    {
        public DataTemplate CompoundIssueTemplate { get; set; }
        public DataTemplate WarningTemplate { get; set; }
        public DataTemplate ErrorTemplate { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //var issues = value as IssueModel;
            var issue = value as IssueModel;
            if (issue == null) return null;

            var resolver = new Convert_IIssueModelResolver(this);
            issue.Accept(resolver);
            return resolver.Result;
        }

        private class Convert_IIssueModelResolver : IIssueModelResolver
        {
            private IssueTemplateSelector selector;

            public Convert_IIssueModelResolver(IssueTemplateSelector selector)
            {
                this.selector = selector;
            }
            
            public DataTemplate Result { get; private set; }

            public void Resolve(CompoundIssueModel model)
            {
                this.Result = this.selector.CompoundIssueTemplate;
            }

            public void Resolve(WarningModel model)
            {
                this.Result = this.selector.WarningTemplate;
            }

            public void Resolve(ErrorModel model)
            {
                this.Result = this.selector.ErrorTemplate;
            }

            
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
