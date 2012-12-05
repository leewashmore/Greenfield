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
using TopDown.FacingServer.Backend.Targeting;

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    public class PortfolioPickedEventArgs : CancellableEventArgs
    {
        [DebuggerStepThrough]
        public PortfolioPickedEventArgs(BgaTargetingTypePickerModel targetingType, BgaPortfolioPickerModel portfolio, Boolean isCancelled)
            : base(isCancelled)
        {
            this.TargetingType = targetingType;
            this.Portfolio = portfolio;
        }

        public BgaTargetingTypePickerModel TargetingType { get; private set; }
        public BgaPortfolioPickerModel Portfolio { get; private set; }
    }

    public delegate void PortfolioPickedEventHandler(Object sender, PortfolioPickedEventArgs args);
}
