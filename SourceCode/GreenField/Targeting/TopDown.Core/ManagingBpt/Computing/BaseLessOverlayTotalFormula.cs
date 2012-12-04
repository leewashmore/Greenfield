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
            Decimal? result = this.cashRescaledBase.Value(ticket);
            foreach (var model in models)
            {
                var value = this.baseLessOverlayFormula.Calculate(model, ticket);
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
            return result;
        }
    }
}
