using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class PortfolioScaledTotalFormula : IFormula<Decimal?>
    {
        private IExpression<Decimal?> cashBaseExpression;
        private IExpression<Decimal?> globePortfolioScaledExpression;

        public PortfolioScaledTotalFormula(IExpression<Decimal?> cashBaseExpression, IExpression<Decimal?> globePortfolioScaledExpression)
        {
            this.cashBaseExpression = cashBaseExpression;
            this.globePortfolioScaledExpression = globePortfolioScaledExpression;
        }

        public Decimal? Calculate(CalculationTicket ticket)
        {
            var cashBase = this.cashBaseExpression.Value(ticket);
            var globePortfolioScaled = this.globePortfolioScaledExpression.Value(ticket);
            if (cashBase.HasValue)
            {
                if (globePortfolioScaled.HasValue)
                {
                    return cashBase.Value + globePortfolioScaled.Value;
                }
                else
                {
                    return cashBase.Value;
                }
            }
            else
            {
                if (globePortfolioScaled.HasValue)
                {
                    return globePortfolioScaled.Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
