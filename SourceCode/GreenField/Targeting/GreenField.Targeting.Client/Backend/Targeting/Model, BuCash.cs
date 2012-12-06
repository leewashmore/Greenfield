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
    public class BuCashModel : IBuLineModel
    {
        [DebuggerStepThrough]
        public BuCashModel(NullableExpressionModel cashExpression)
        {
            this.Cash = cashExpression;
        }

        public NullableExpressionModel Cash { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IBuLineModelResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
