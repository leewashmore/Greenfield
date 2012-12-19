using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class BaseLessOverlayTotalFormula : IFormula<Decimal?>
    {
        private IEnumerable<IModel> models;
        private IModelFormula<IModel, Decimal?> baseLessOverlayFormula;
        private IExpression<Decimal?> cashRescaledBase;

        public BaseLessOverlayTotalFormula(
            IEnumerable<IModel> models,
            IModelFormula<IModel, Decimal?> baseLessOverlayFormula,
            IExpression<Decimal?> cashRescaledBase
        )
        {
            this.models = models;
            this.baseLessOverlayFormula = baseLessOverlayFormula;
            this.cashRescaledBase = cashRescaledBase;
        }
        public Decimal? Calculate(CalculationTicket ticket)
        {
            var result = this.Calculate(ticket, No.CalculationTracer);
            return result;


        }


        public decimal? Calculate(CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Base less overlay total formula");
            tracer.Indent();


            Decimal? result = this.cashRescaledBase.Value(ticket, tracer, "Cash rescaled base");
            tracer.WriteValue(" + Cash rescaled base", result);
            foreach (var model in models)
            {
                var value = this.baseLessOverlayFormula.Calculate(model, ticket, tracer);
                if (value.HasValue)
                {
                    tracer.WriteValue("+ Base less overlay", value);
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
    }
}
