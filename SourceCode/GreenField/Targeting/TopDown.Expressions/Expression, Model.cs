using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Expressions
{
    public class ModelFormulaExpression<TModel, TValue> : IExpression<TValue>
    {
        private Func<ModelFormulaExpression<TModel, TValue>, CalculationTicket, IEnumerable<IValidationIssue>> validator;

        [DebuggerStepThrough]
        public ModelFormulaExpression(
			String name,
            TModel model,
            IModelFormula<TModel, TValue> formula,
            IValueAdapter<TValue> adapter,
            Func<ModelFormulaExpression<TModel, TValue>, CalculationTicket, IEnumerable<IValidationIssue>> validator
        )
        {
			this.Name = name;
            this.Model = model;
            this.Formula = formula;
            this.Adapter = adapter;
            this.validator = validator;
        }

		public String Name { get; private set; }
        public TModel Model { get; private set; }
        public IModelFormula<TModel, TValue> Formula { get; private set; }
        public IValueAdapter<TValue> Adapter { get; private set; }
		public TValue Value(CalculationTicket ticket)
		{
			return this.Formula.Calculate(this.Model, ticket);
		}
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
