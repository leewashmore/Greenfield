using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class TrueExposureFormula : IModelFormula<IModel, Decimal?>
    {
        private ExpressionPicker picker;

        public TrueExposureFormula(ExpressionPicker picker)
        {
            this.picker = picker;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            var overlayExpressionOpt = this.picker.Overlay.TryPickExpression(model);
            var portfolioScaledExpressionOpt = this.picker.PortfolioScaled.TryPickExpression(model);
            if (overlayExpressionOpt == null || portfolioScaledExpressionOpt == null) return null;
            var overlayValue = overlayExpressionOpt.Value(ticket);
            var portfolioScaledValue = portfolioScaledExpressionOpt.Value(ticket);
            if (!portfolioScaledValue.HasValue) return null;
            var value = overlayValue + portfolioScaledValue.Value;
            return value;
        }
    }
}
