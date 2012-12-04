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
    public class TotalLineModel : GlobeResident
    {
        [DebuggerStepThrough]
        public TotalLineModel(
            NullableExpressionModel baseExpression,
            ExpressionModel benchmarkExpression,
            ExpressionModel overlayExpression,
            NullableExpressionModel portfolioAdjustmentExpression,
            NullableExpressionModel portfolioScaledExpression,
            NullableExpressionModel trueExposureExpression,
            NullableExpressionModel trueActiveExpression
        )
        {
            this.Base = baseExpression;
            this.Benchmark = benchmarkExpression;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueExposure = trueExposureExpression;
            this.TrueActive = trueActiveExpression;
        }

        public NullableExpressionModel Base { get; private set; }
        public ExpressionModel Benchmark { get; private set; }
        public ExpressionModel Overlay { get; private set; }
        public NullableExpressionModel PortfolioAdjustment { get; private set; }
        public NullableExpressionModel PortfolioScaled { get; private set; }
        public NullableExpressionModel TrueExposure { get; private set; }
        public NullableExpressionModel TrueActive { get; private set; }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
