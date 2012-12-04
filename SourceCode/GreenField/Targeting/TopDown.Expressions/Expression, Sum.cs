using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
#warning NEXT: We don't need to have a separated expression for sum, it can be replaced with a generic one.

    public class SumExpression : IExpression<Decimal>
    {
        private Decimal defaultValue;
        private IEnumerable<IExpression<Decimal>> expressions;
        private Func<SumExpression, CalculationTicket, IEnumerable<IValidationIssue>> validator;

        [DebuggerStepThrough]
        public SumExpression(
			String name,
            IEnumerable<IExpression<Decimal>> expressions,
            Decimal defaultValue,
            Func<SumExpression, CalculationTicket, IEnumerable<IValidationIssue>> validator
        )
        {
			this.Name = name;
            this.defaultValue = defaultValue;
            this.expressions = expressions;
            this.validator = validator;
        }

		public String Name { get; private set; }
        public Decimal Value(CalculationTicket ticket)
        {
            var result = this.defaultValue;
            foreach (var expressionOpt in this.expressions)
            {
                if (expressionOpt == null) continue;
                var value = expressionOpt.Value(ticket);
                result = result + value;
            }
            return result;
        }
        [DebuggerStepThrough]
        public IEnumerable<IValidationIssue> Validate(CalculationTicket ticket)
        {
            return this.validator(this, ticket);
        }

        [DebuggerStepThrough]
        public void Accept(IExpressionResolver<Decimal> resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(IExpressionResolver resolver)
        {
            resolver.Resolve(this);
        }

		
	}
}
