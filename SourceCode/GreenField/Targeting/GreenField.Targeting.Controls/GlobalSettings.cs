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

namespace GreenField.Targeting.Controls
{
    public class GlobalSettings
    {
        [DebuggerStepThrough]
        public GlobalSettings(
            IClientFactory clientFactory,
            BroadGlobalActive.Settings bgaSettings,
            BottomUp.Settings buSettings,
            BasketTargets.Settings btSettings
        )
        {
            this.BgaSettings = bgaSettings;
            this.BuSettings = buSettings;
            this.BtSettings = btSettings;
            this.ClientFactory = clientFactory;
        }

        public BroadGlobalActive.Settings BgaSettings { get; private set; }
        public BottomUp.Settings BuSettings { get; private set; }
        public BasketTargets.Settings BtSettings { get; private set; }
        public IClientFactory ClientFactory { get; private set; }
    }
}
