using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core
{
    public abstract class ModelChangeDetectorBase
    {
        protected Boolean HasChanged(IEnumerable<IExpression> expressions)
        {
            return expressions.Any(expression => this.CheckIfChangedOnceResolved(expression));
        }
        protected Boolean CheckIfChangedOnceResolved(IExpression expression)
        {
            var resolver = new CheckIfChangedOnceResolved_IExpressionResolver(this);
            expression.Accept(resolver);
            return resolver.Result;
        }

        private class CheckIfChangedOnceResolved_IExpressionResolver : IExpressionResolver
        {
            private ModelChangeDetectorBase detector;

            public CheckIfChangedOnceResolved_IExpressionResolver(ModelChangeDetectorBase detector)
            {
                this.detector = detector;
            }

            public Boolean Result { get; private set; }

            public void Resolve<TValue>(Expression<TValue> expression)
            {
                this.Result = false;
            }

            public void Resolve<TModel, TValue>(ModelFormulaExpression<TModel, TValue> expression)
            {
                this.Result = false;
            }

            public void Resolve<TValue>(UnchangableExpression<TValue> expression)
            {
                this.Result = false;
            }

            public void Resolve(EditableExpression expression)
            {
                this.Result = expression.IsModified;
            }

            public void Resolve(SumExpression expression)
            {
                this.Result = false;
            }

            public void Resolve(NullableSumExpression expression)
            {
                this.Result = false;
            }

        }

    }
}
