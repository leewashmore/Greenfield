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
            var result = this.Calculate(model, ticket, No.CalculationTracer);
            return result;

        }


        public Decimal? Calculate(IModel model, CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Base active formula");
            tracer.Indent();
            var baseExpressionOpt = picker.Base.TryPickExpression(model);
            var benchmarkExpressionOpt = picker.Benchmark.TryPickExpression(model);
            if (baseExpressionOpt == null || benchmarkExpressionOpt == null) return null;
            var baseValue = baseExpressionOpt.Value(ticket, tracer, "Base");
            var benchmarkValue = benchmarkExpressionOpt.Value(ticket, tracer, "Benchmark");
            Decimal? result;
            if (!baseValue.HasValue)
            {
                result = null;
            }
            else
            {
                result = baseValue.Value - benchmarkValue;
            }
            tracer.WriteValue("Base - Benchmark", result);
            tracer.Unindent();
            return result;
        }
    }
}
