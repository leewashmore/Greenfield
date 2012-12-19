using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class PortfolioScaledFormula : IModelFormula<IModel, Decimal?>
    {
        private IModelFormula<IModel, Decimal?> baseLessOverlayFormula;
        private IExpression<Decimal?> baseLessOverlayPositiveTotal;
        private IExpression<Decimal?> baseLessOverlayTotal;

        public PortfolioScaledFormula(
            IModelFormula<IModel, Decimal?> baseLessOverlayFormula,
            IExpression<Decimal?> baseLessOverlayPositiveTotal,
            IExpression<Decimal?> baseLessOverlayTotal
        )
        {
            this.baseLessOverlayFormula = baseLessOverlayFormula;
            this.baseLessOverlayPositiveTotal = baseLessOverlayPositiveTotal;
            this.baseLessOverlayTotal = baseLessOverlayTotal;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            var result = this.Calculate(model, ticket, No.CalculationTracer);
            return result;

        }


        public decimal? Calculate(IModel model, CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Portfolio scaled formula");
            tracer.Indent();
            var baseLessOverlay = this.baseLessOverlayFormula.Calculate(model, ticket, tracer);
            if (!baseLessOverlay.HasValue) return null;
            if (baseLessOverlay.Value < 0) return 0m;

            var positiveTotal = baseLessOverlayPositiveTotal.Value(ticket, tracer, "Base less overlay positive total");
            if (!positiveTotal.HasValue) return null;

            var total = baseLessOverlayTotal.Value(ticket);
            if (!total.HasValue) return null;

            if (positiveTotal.Value == 0)
            {
                throw new NotImplementedException();
            }
            tracer.WriteLine("Undone");
            var result = baseLessOverlay.Value / positiveTotal.Value * total.Value;
            tracer.Unindent();
            return result;
        }
    }
}
