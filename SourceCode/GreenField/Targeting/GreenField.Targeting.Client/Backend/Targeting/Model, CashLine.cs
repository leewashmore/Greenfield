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

namespace TopDown.FacingServer.Backend.Targeting
{
    public class CashLineModel : GlobeResident
    {
        public CashLineModel(NullableExpressionModel baseExpression, NullableExpressionModel portfolioScaledExpression)
        {
            this.Base = baseExpression;
            this.PortfolioScaled = portfolioScaledExpression;
        }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        public NullableExpressionModel Base { get; private set; }
        public NullableExpressionModel PortfolioScaled { get; private set; }
    }
}
