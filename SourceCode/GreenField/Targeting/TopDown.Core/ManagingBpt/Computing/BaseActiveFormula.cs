using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class BaseActiveFormula : IModelFormula<IModel, Decimal?>
    {
        private ExpressionPicker picker;

        public BaseActiveFormula(ExpressionPicker picker)
        {
            this.picker = picker;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            var baseExpressionOpt = picker.Base.TryPickExpression(model);
            var benchmarkExpressionOpt = picker.Benchmark.TryPickExpression(model);
            if (baseExpressionOpt == null || benchmarkExpressionOpt == null) return null;
            var baseValue = baseExpressionOpt.Value(ticket);
            var benchmarkValue = benchmarkExpressionOpt.Value(ticket);
            if (!baseValue.HasValue) return null;
            var result = baseValue.Value - benchmarkValue;
            return result;
        }
    }
}
