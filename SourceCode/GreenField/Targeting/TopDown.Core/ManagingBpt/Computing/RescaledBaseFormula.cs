using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class RescaledBaseFormula : IModelFormula<IModel, Decimal?>
    {
        private IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal;
        private IExpression<Decimal?> porfolioAdjustmentTotal;

        public RescaledBaseFormula(IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal, IExpression<Decimal?> porfolioAdjustmentTotal)
        {
            // TODO: Complete member initialization
            this.baseWherePortfoioAdjustmentSetTotal = baseWherePortfoioAdjustmentSetTotal;
            this.porfolioAdjustmentTotal = porfolioAdjustmentTotal;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            // if portfolio adjustment has anything return it
            var multimethod = new RescaledBaseForAdjustmentValueMultimethod(
                ticket,
                this.baseWherePortfoioAdjustmentSetTotal,
                this.porfolioAdjustmentTotal
            );
            model.Accept(multimethod);
            return multimethod.Result;
        }

        public static Decimal? CalculateRescaledBase(
            Decimal? @base,
            Decimal? portfolioAdjustment,
            Decimal? baseWherePortfoioAdjustmentSetTotal,
            Decimal? porfolioAdjustmentTotal
        )
        {
            if (portfolioAdjustment.HasValue)
            {
                return portfolioAdjustment.Value;
            }
            else if (@base.HasValue)
            {
                var result = @base.Value
                    / (1.0m - (baseWherePortfoioAdjustmentSetTotal.HasValue ? baseWherePortfoioAdjustmentSetTotal.Value : 0m))
                    * (1.0m - (porfolioAdjustmentTotal.HasValue ? porfolioAdjustmentTotal.Value : 0m));
                return result;
            }
            else
            {
                return null;
            }
        }

        private class RescaledBaseForAdjustmentValueMultimethod : IModelResolver
        {
            private IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal;
            private IExpression<Decimal?> porfolioAdjustmentTotal;
            private CalculationTicket ticket;
            public RescaledBaseForAdjustmentValueMultimethod(
                CalculationTicket ticket,
                IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal,
                IExpression<Decimal?> porfolioAdjustmentTotal
            )
            {
                this.ticket = ticket;
                this.baseWherePortfoioAdjustmentSetTotal = baseWherePortfoioAdjustmentSetTotal;
                this.porfolioAdjustmentTotal = porfolioAdjustmentTotal;
            }
            public Decimal? Result { get; private set; }
            public void Resolve(BasketCountryModel model)
            {
                this.Result = CalculateRescaledBase(
                    model.Base.EditedValue,
                    model.PortfolioAdjustment.EditedValue,
                    this.baseWherePortfoioAdjustmentSetTotal.Value(this.ticket),
                    this.porfolioAdjustmentTotal.Value(this.ticket)
                );
            }
            public void Resolve(BasketRegionModel model)
            {
                this.Result = CalculateRescaledBase(
                    model.Base.EditedValue,
                    model.PortfolioAdjustment.EditedValue,
                    this.baseWherePortfoioAdjustmentSetTotal.Value(this.ticket),
                    this.porfolioAdjustmentTotal.Value(this.ticket)
                );
            }
            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.Result = CalculateRescaledBase(
                    model.Base.EditedValue,
                    model.PortfolioAdjustment.EditedValue,
                    this.baseWherePortfoioAdjustmentSetTotal.Value(this.ticket),
                    this.porfolioAdjustmentTotal.Value(this.ticket)
                );
            }
            public void Resolve(CountryModel model) { }
            public void Resolve(OtherModel model) { }
            public void Resolve(RegionModel model) { }
        }
    }
}
