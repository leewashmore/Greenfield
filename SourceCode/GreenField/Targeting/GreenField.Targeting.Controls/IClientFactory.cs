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

namespace GreenField.Targeting.Controls
{
    public interface IClientFactory
    {
        string GetUsername();
        void Initialize(String username);
        TopDown.FacingServer.Backend.Targeting.FacadeClient CreateClient();
    }
}
