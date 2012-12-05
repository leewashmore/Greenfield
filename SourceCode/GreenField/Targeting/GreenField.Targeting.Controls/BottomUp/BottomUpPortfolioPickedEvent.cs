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
using TopDown.FacingServer.Backend.Targeting;
using System.Diagnostics;

namespace GreenField.Targeting.Controls.BottomUp
{
    public class BottomUpPortfolioPickedEventArgs : CancellableEventArgs
    {
        [DebuggerStepThrough]
        public BottomUpPortfolioPickedEventArgs(BottomUpPortfolioModel bottomUpPortfolio, Boolean isCancelled)
            : base(isCancelled)
        {
            this.BottomUpPortfolio = bottomUpPortfolio;
        }

        public BottomUpPortfolioModel BottomUpPortfolio { get; private set; }
    }

    public delegate void BottomUpPortfolioEventHandler(Object sender, BottomUpPortfolioPickedEventArgs args);
}
