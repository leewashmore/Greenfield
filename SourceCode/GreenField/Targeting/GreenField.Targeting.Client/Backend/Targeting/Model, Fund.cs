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
using Aims.Data.Client;
using System.Diagnostics;

namespace TopDown.FacingServer.Backend.Targeting
{
    public partial class FundModel : IFund
    {
        public FundModel()
        {
        }
        public FundModel(String id, String name, String shortName, String ticker)
            : base(id, name)
        {
            this.ShortName = shortName;
            this.Ticker = ticker;
        }

        [DebuggerStepThrough]
        public override void Accept(Aims.Data.Client.ISecurityResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
