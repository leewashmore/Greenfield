using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class AddFormula : IFormula<Decimal?>
    {
        private IExpression<Decimal?> oneExpression;
        private IExpression<Decimal?> anotherExpression;

        public AddFormula(IExpression<Decimal?> oneExpression, IExpression<Decimal?> anotherExpression)
        {
            this.oneExpression = oneExpression;
            this.anotherExpression = anotherExpression;
        }

        public Decimal? Calculate(CalculationTicket ticket)
        {
            var one = this.oneExpression.Value(ticket);
            var another = this.anotherExpression.Value(ticket);
            if (one.HasValue)
            {
                if (another.HasValue)
                {
                    return one.Value + another.Value;
                }
                else
                {
                    return one.Value;
                }
            }
            else
            {
                if (another.HasValue)
                {
                    return another.Value;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
