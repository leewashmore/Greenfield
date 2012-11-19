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
using GreenField.Targeting.Only.Backend.Targeting;

namespace GreenField.Targeting.Only
{
    public class DefaultClientFactory : IClientFactory
    {

        public Backend.Targeting.FacadeClient CreateClient()
        {
            var result = new FacadeClient();
            return result;
        }
    }
}
