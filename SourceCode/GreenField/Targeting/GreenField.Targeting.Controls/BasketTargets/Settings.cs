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
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BasketTargets
{
    public class Settings
    {
        [DebuggerStepThrough]
        public Settings(IClientFactory clientFactory)
        {
            this.ClientFactory = clientFactory;
        }

        public IClientFactory ClientFactory { get; private set; }
    }
}
