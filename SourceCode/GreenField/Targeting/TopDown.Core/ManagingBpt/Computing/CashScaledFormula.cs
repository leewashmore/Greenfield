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
            IExpression<Decimal?> rescaledBase,
            IExpression<Decimal?> baseLessOverlayPositiveTotal,
            IExpression<Decimal?> baseLessOverlayTotal
            )
        {
            this.rescaledBase = rescaledBase;
            this.baseLessOverlayPositiveTotal = baseLessOverlayPositiveTotal;
            this.baseLessOverlayTotal = baseLessOverlayTotal;

        }
    
        private IExpression<Decimal?> rescaledBase;
        private IExpression<Decimal?> baseLessOverlayPositiveTotal;
        private IExpression<Decimal?> baseLessOverlayTotal;



        public decimal? Calculate(CalculationTicket ticket)
        {
            var baseCash = this.rescaledBase.Value(ticket);
            if (!baseCash.HasValue) return null;
            if (baseCash.Value < 0) return 0m;

            var positiveTotal = baseLessOverlayPositiveTotal.Value(ticket);
            if (!positiveTotal.HasValue) return null;

            var total = baseLessOverlayTotal.Value(ticket);
            if (!total.HasValue) return null;

            if (positiveTotal.Value == 0)
            {
                throw new NotImplementedException();
            }
            var value = baseCash.Value / positiveTotal.Value * total.Value;
            return value;
        }
    }
}
