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
    public interface IBtLineModel
    {
        void Accept(IBtLineModelResolver resolver);
    }

    public interface IBtLineModelResolver
    {
        void Resolve(BtSecurityModel model);
        void Resolve(BtRootModel model);
    }
}
