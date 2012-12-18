using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class BaseLessOverlayFormula : IModelFormula<IModel, Decimal?>
    {
        private ExpressionPicker picker;
        private IModelFormula<IModel, Decimal?> rescaledBaseForAdjustmentFormula;

        public BaseLessOverlayFormula(ExpressionPicker picker, IModelFormula<IModel, Decimal?> rescaledBaseForAdjustmentFormula)
        {
            this.picker = picker;
            this.rescaledBaseForAdjustmentFormula = rescaledBaseForAdjustmentFormula;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            var result = this.Calculate(model, ticket, No.CalculationTracer);
            return result;

        }


        public Decimal? Calculate(IModel model, CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Base less overlay formula");
            tracer.Indent();
            var rescaledBaseValue = this.rescaledBaseForAdjustmentFormula.Calculate(model, ticket, tracer);
            tracer.WriteValue("Rescaled base", rescaledBaseValue);
            Decimal? result;
            if (rescaledBaseValue.HasValue)
            {
                var overlayExpressionOpt = picker.Overlay.TryPickExpression(model);
                if (overlayExpressionOpt == null) throw new ApplicationException("There is no overlay expression.");
                var overlayValue = overlayExpressionOpt.Value(ticket, tracer, "Overlay");
                result = rescaledBaseValue.Value - overlayValue;
            }
            else
            {
                result = null;
            }
            tracer.WriteValue("Result", result);
            tracer.Unindent();
            return result;
        }
    }
}
