using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class TrueActiveFormula : IModelFormula<IModel, Decimal?>
    {
        private ExpressionPicker picker;

        public TrueActiveFormula(ExpressionPicker picker)
        {
            this.picker = picker;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            var trueExposureExpressionOpt = this.picker.TrueExposure.TryPickExpression(model);
            var benchmarkExpressionOpt = this.picker.Benchmark.TryPickExpression(model);
            if (trueExposureExpressionOpt == null || benchmarkExpressionOpt == null) return null;
            var trueExposure = trueExposureExpressionOpt.Value(ticket);
            if (!trueExposure.HasValue) return null;
            var benchmark = benchmarkExpressionOpt.Value(ticket);
            // var value = trueExposure.Value - benchmark
            // per email from Gerred: Gadget is not calculating the True Active (Benchmark – True Exposure).
            var value = benchmark - trueExposure.Value;
            return value;
        }
    }
}
