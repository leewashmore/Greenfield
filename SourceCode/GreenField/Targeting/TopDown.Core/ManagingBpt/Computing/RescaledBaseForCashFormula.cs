using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class RescaledBaseForCashFormula : IFormula<Decimal?>
    {
        private IExpression<Decimal?> cashBase;
        private IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal;
        private IExpression<Decimal?> porfolioAdjustmentTotal;

        public RescaledBaseForCashFormula(IExpression<Decimal?> cashBase, IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal, IExpression<Decimal?> porfolioAdjustmentTotal)
        {
            this.cashBase = cashBase;
            this.baseWherePortfoioAdjustmentSetTotal = baseWherePortfoioAdjustmentSetTotal;
            this.porfolioAdjustmentTotal = porfolioAdjustmentTotal;
        }

        public Decimal? Calculate(CalculationTicket ticket)
        {
            var baseValue = cashBase.Value(ticket);
            var baseWherePortfoioAdjustmentSetTotalValue = baseWherePortfoioAdjustmentSetTotal.Value(ticket);
            var porfolioAdjustmentTotalValue = porfolioAdjustmentTotal.Value(ticket);
            if (baseValue.HasValue)
            {
                var result = baseValue.Value
                    / (1 - (baseWherePortfoioAdjustmentSetTotalValue.HasValue ? baseWherePortfoioAdjustmentSetTotalValue.Value : 0m))
                    * (1 - (porfolioAdjustmentTotalValue.HasValue ? porfolioAdjustmentTotalValue.Value : 0m));
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
