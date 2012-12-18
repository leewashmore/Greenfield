using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class CashScaledFormula : IFormula<Decimal?>
    {
        public CashScaledFormula(
            IExpression<Decimal?> rescaledCashBase,
            IExpression<Decimal?> baseLessOverlayPositiveTotal,
            IExpression<Decimal?> baseLessOverlayTotal
            )
        {
            this.rescaledCashBase = rescaledCashBase;
            this.baseLessOverlayPositiveTotal = baseLessOverlayPositiveTotal;
            this.baseLessOverlayTotal = baseLessOverlayTotal;

        }

        private IExpression<Decimal?> rescaledCashBase;
        private IExpression<Decimal?> baseLessOverlayPositiveTotal;
        private IExpression<Decimal?> baseLessOverlayTotal;

        public Decimal? Calculate(CalculationTicket ticket)
        {
            var result = this.Calculate(ticket, No.CalculationTracer);
            return result;
            
        }


        public Decimal? Calculate(CalculationTicket ticket, ICalculationTracer tracer)
        {
            var rescaledBaseCash = this.rescaledCashBase.Value(ticket, tracer, "Rescaled cash base");
            if (!rescaledBaseCash.HasValue) return null;
            if (rescaledBaseCash.Value < 0) return 0m;

            var positiveTotal = baseLessOverlayPositiveTotal.Value(ticket, tracer, "Base less overlay positive total");
            if (!positiveTotal.HasValue) return null;

            var total = baseLessOverlayTotal.Value(ticket, tracer, "Base less overlay total");
            if (!total.HasValue) return null;

            if (positiveTotal.Value == 0)
            {
                throw new NotImplementedException();
            }
            var value = rescaledBaseCash.Value / positiveTotal.Value * total.Value;
            tracer.WriteValue("Rescaled cash base / positive base less overlay total * base less overlay total", value);
            return value;
        }
    }
}
