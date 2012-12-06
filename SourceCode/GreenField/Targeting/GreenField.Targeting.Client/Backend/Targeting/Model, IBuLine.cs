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

namespace TopDown.FacingServer.Backend.Targeting
{
    public interface IBuLineModel
    {
        void Accept(IBuLineModelResolver resolver);
    }

    public interface IBuLineModelResolver
    {
        void Resolve(BuTotalModel model);
        void Resolve(BuCashModel model);
        void Resolve(BuItemModel model);
    }
}
