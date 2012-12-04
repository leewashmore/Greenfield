using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface IExpression
    {
        void Accept(IExpressionResolver resolver);
    }

    public interface IExpressionResolver
    {
        void Resolve<TValue>(Expression<TValue> expression);
        void Resolve<TModel, TValue>(ModelFormulaExpression<TModel, TValue> expression);
        void Resolve<TValue>(UnchangableExpression<TValue> expression);
        void Resolve(EditableExpression expression);
        void Resolve(SumExpression expression);
        void Resolve(NullableSumExpression expression);
    }

	public interface IExpression<TValue> : IExpression
	{
		String Name { get; }
        TValue Value(CalculationTicket ticket);
        void Accept(IExpressionResolver<TValue> resolver);
        IEnumerable<IValidationIssue> Validate(CalculationTicket ticket);
    }

    public interface IExpressionResolver<TValue>
    {
        void Resolve(EditableExpression expression);
        void Resolve(Expression<TValue> expression);
        void Resolve<TModel>(ModelFormulaExpression<TModel, TValue> expression);
        void Resolve(SumExpression expression);
        void Resolve(NullableSumExpression expression);
        void Resolve(UnchangableExpression<TValue> expression);
    }
}
