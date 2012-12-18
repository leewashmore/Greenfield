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
            var result = this.Calculate(ticket, No.CalculationTracer);
            return result;

        }


        public Decimal? Calculate(CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Add formula");
            tracer.Indent();
            var one = this.oneExpression.Value(ticket, tracer, No.ExpressionName);
            var another = this.anotherExpression.Value(ticket, tracer, No.ExpressionName);
            Decimal? result;
            if (one.HasValue)
            {
                if (another.HasValue)
                {
                    result = one.Value + another.Value;
                }
                else
                {
                    result = one.Value;
                }
            }
            else
            {
                if (another.HasValue)
                {
                    result = another.Value;
                }
                else
                {
                    result = null;
                }
            }
            tracer.WriteValue("Sum", result);
            tracer.Unindent();
            return result;
        }
    }
}
