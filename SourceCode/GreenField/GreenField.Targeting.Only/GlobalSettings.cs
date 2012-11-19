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

namespace GreenField.Targeting.Only
{
    public class GlobalSettings
    {
        [DebuggerStepThrough]
        public GlobalSettings(BroadGlobalActive.Settings bgaSettings)
        {
            this.BgaSettings = bgaSettings;
        }

        public BroadGlobalActive.Settings BgaSettings { get; private set; }
    }
}
