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
using System.Globalization;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class SecurityModelToPortfolioTargetConverter : IValueConverter
    {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            var wrap = value as DataAndIndexWrap;
            var model = wrap.Data as BtSecurityModel;
            return model.PortfolioTargets[wrap.Index].PortfolioTarget;
        }

        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
