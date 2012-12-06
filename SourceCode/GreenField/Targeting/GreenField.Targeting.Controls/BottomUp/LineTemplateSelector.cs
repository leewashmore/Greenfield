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

namespace GreenField.Targeting.Controls.BottomUp
{
    public class LineTemplateSelector : IValueConverter
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate SecurityTemplate { get; set; }
        public DataTemplate CashTemplate { get; set; }
        public DataTemplate TotalTemplate { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            DataTemplate result;
            var model = value as IBuLineModel;
            if (model == null) 
            {
                result = this.DefaultTemplate;
            }
            else
            {
                var resolver = new Convert_IBuModelResolver(this);
                model.Accept(resolver);
                result = resolver.Result ?? this.TotalTemplate;
            }
            return result;
        }

        private class Convert_IBuModelResolver : IBuLineModelResolver
        {
            private LineTemplateSelector selector;

            public Convert_IBuModelResolver(LineTemplateSelector selector)
            {
                this.selector = selector;
            }

            public DataTemplate Result { get; private set; }

            public void Resolve(BuTotalModel model)
            {
                this.Result = this.selector.TotalTemplate;
            }

            public void Resolve(BuCashModel model)
            {
                this.Result = this.selector.CashTemplate;
            }

            public void Resolve(BuItemModel model)
            {
                this.Result = this.selector.SecurityTemplate;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
