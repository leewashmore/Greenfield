using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class Expression<TValue> : IExpression<TValue>
    {
        private IFormula<TValue> formula;
        private Func<Expression<TValue>, CalculationTicket, IEnumerable<IValidationIssue>> validator;
        private TValue value;
        private CalculationTicket ticket;

        [DebuggerStepThrough]
        public Expression(
			String name,
            IFormula<TValue> formula,
            IValueAdapter<TValue> adapter,
            Func<Expression<TValue>, CalculationTicket, IEnumerable<IValidationIssue>> validator
        )
        {
			this.Name = name;
            this.formula = formula;
            this.Adapter = adapter;
            this.validator = validator;
        }

		public String Name { get; private set; }
		public IValueAdapter<TValue> Adapter { get; private set; }
		public TValue Value(CalculationTicket ticket)
        {
            if (this.ticket == ticket)
            {
                return this.value;
            }
            else
            {
                var value = this.formula.Calculate(ticket);
                this.ticket = ticket;
                this.value = value;
                return value;
            }
        }

        [DebuggerStepThrough]
        public IEnumerable<IValidationIssue> Validate(CalculationTicket ticket)
        {
            return this.validator(this, ticket);
        }

        [DebuggerStepThrough]
        public void Accept(IExpressionResolver<TValue> resolver)
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
