using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    /// <summary>
    /// Used for Benchmarks.
    /// </summary>
    public class UnchangableExpression<TValue> : IExpression<TValue>
    {
        private Func<UnchangableExpression<TValue>, IEnumerable<IValidationIssue>> validator;

        [DebuggerStepThrough]
        public UnchangableExpression(
            String name,
            TValue value,
            IValueAdapter<TValue> adapter,
            Func<UnchangableExpression<TValue>, IEnumerable<IValidationIssue>> validator)
        {
            this.Name = name;
            this.DefaultValue = this.InitialValue = value;
            this.Adapter = adapter;
            this.validator = validator;
        }
        public String Name { get; private set; }
        public TValue DefaultValue { get; private set; }

        private TValue initialValue;
        public TValue InitialValue
        {
            get { return this.initialValue; }
            set { this.initialValue = value; }
        }

        public IValueAdapter<TValue> Adapter { get; private set; }
        public TValue Value(CalculationTicket ticket)
        {
            var result = this.Value(ticket, No.CalculationTracer, No.ExpressionName);
            return result;
        }
        public TValue Value(CalculationTicket ticket, ICalculationTracer tracer, String name)
        {
            var result = this.InitialValue;
            tracer.WriteValue(name ?? this.Name, result, this.Adapter);
            return result;
        }

        [DebuggerStepThrough]
        public IEnumerable<IValidationIssue> Validate(CalculationTicket ticket)
        {
            return this.validator(this);
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
