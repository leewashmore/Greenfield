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

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class CellTemplateSelector : IValueConverter
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate SecurityTemplate { get; set; }
        public DataTemplate RootTemplate { get; set; }

        public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            DataTemplate result;
            var line = value as IBtLineModel;
            if (line == null)
            {
                result = this.DefaultTemplate;
            }
            else
            {
                var resolver = new Convert_IBtLineModelResolver(this);
                line.Accept(resolver);
                result = resolver.Result ?? this.DefaultTemplate;
            }
            return result;
        }

        private class Convert_IBtLineModelResolver : IBtLineModelResolver
        {
            private CellTemplateSelector selector;

            public Convert_IBtLineModelResolver(CellTemplateSelector selector)
            {
                this.selector = selector;
            }

            public DataTemplate Result { get; private set; }

            public void Resolve(BtSecurityModel model)
            {
                this.Result = this.selector.SecurityTemplate;
            }

            public void Resolve(BtRootModel model)
            {
                this.Result = this.selector.RootTemplate;
            }
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
