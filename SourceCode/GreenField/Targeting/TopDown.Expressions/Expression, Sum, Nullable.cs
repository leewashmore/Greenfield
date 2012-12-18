using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class NullableSumExpression : IExpression<Decimal?>
    {
        private Decimal? defaultValue;
        private IEnumerable<IExpression<Decimal?>> expressions;
        private Func<NullableSumExpression, CalculationTicket, IEnumerable<IValidationIssue>> validator;

        public NullableSumExpression(
            String name,
            IEnumerable<IExpression<Decimal?>> expressions,
            Decimal? defaultValue,
            Func<NullableSumExpression, CalculationTicket, IEnumerable<IValidationIssue>> validator
        )
        {
            this.Name = name;
            this.defaultValue = defaultValue;
            this.expressions = expressions;
            this.validator = validator;
        }

        public String Name { get; private set; }
        public Decimal? Value(CalculationTicket ticket)
        {
            var result = this.Value(ticket, No.CalculationTracer, No.ExpressionName);
            return result;
        }

        public Decimal? Value(CalculationTicket ticket, ICalculationTracer tracer, String name)
        {
            tracer.WriteLine("Nullable sum: " + (name ?? this.Name));
            tracer.Indent();
            var result = this.defaultValue;
            foreach (var expressionOpt in this.expressions)
            {
                if (expressionOpt == null) continue;
                var value = expressionOpt.Value(ticket);
                tracer.WriteValue("+", value);
                if (value.HasValue)
                {
                    if (result.HasValue)
                    {
                        result = result.Value + value.Value;
                    }
                    else
                    {
                        result = value.Value;
                    }
                }

            }
            tracer.WriteValue("Total", result);
            tracer.Unindent();
            return result;
        }
        [DebuggerStepThrough]
        public IEnumerable<IValidationIssue> Validate(CalculationTicket ticket)
        {
            return this.validator(this, ticket);
        }

        [DebuggerStepThrough]
        public void Accept(IExpressionResolver<Decimal?> resolver)
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
