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

        public RescaledBaseFormula(
            IExpression<Decimal?> baseWherePortfoioAdjustmentSetTotal,
            IExpression<Decimal?> porfolioAdjustmentTotal
        )
        {
            // TODO: Complete member initialization
            this.baseWherePortfoioAdjustmentSetTotal = baseWherePortfoioAdjustmentSetTotal;
            this.porfolioAdjustmentTotal = porfolioAdjustmentTotal;
        }

        public Decimal? Calculate(IModel model, CalculationTicket ticket)
        {
            var result = this.Calculate(model, ticket, No.CalculationTracer);
            return result;

        }
        public Decimal? Calculate(IModel model, CalculationTicket ticket, ICalculationTracer tracer)
        {
            // if portfolio adjustment has anything return it

            var baseWherePortfolioAdjustmentSetTotalValue = this.baseWherePortfoioAdjustmentSetTotal.Value(ticket, tracer, "");
            var porfolioAdjustmentTotalValue = porfolioAdjustmentTotal.Value(ticket, tracer, "");

            var multimethod = new RescaledBaseForAdjustmentValueMultimethod(
                ticket,
                baseWherePortfolioAdjustmentSetTotalValue,
                porfolioAdjustmentTotalValue,
                tracer
            );
            model.Accept(multimethod);
            return multimethod.Result;
        }

        public static Decimal? CalculateRescaledBase(
            Decimal? @base,
            Decimal? portfolioAdjustment,
            Decimal? baseWherePortfoioAdjustmentSetTotal,
            Decimal? porfolioAdjustmentTotal,
            ICalculationTracer tracer
        )
        {
            Decimal? result;
            if (portfolioAdjustment.HasValue)
            {
                result = portfolioAdjustment.Value;
                tracer.WriteValue("Portfolio adjustment as is", result);
            }
            else if (@base.HasValue)
            {
                result = @base.Value
                    / (1.0m - (baseWherePortfoioAdjustmentSetTotal.HasValue ? baseWherePortfoioAdjustmentSetTotal.Value : 0m))
                    * (1.0m - (porfolioAdjustmentTotal.HasValue ? porfolioAdjustmentTotal.Value : 0m));

                tracer.WriteValue("Base / (1 - baseWherePortfoioAdjustmentSetTotal) * (1 - porfolioAdjustmentTotal)", result);
            }
            else
            {
                result = null;
            }
            return result;
        }

        private class RescaledBaseForAdjustmentValueMultimethod : IModelResolver
        {
            private Decimal? baseWherePortfoioAdjustmentSetTotal;
            private Decimal? porfolioAdjustmentTotal;
            private CalculationTicket ticket;
            private ICalculationTracer tracer;
            public RescaledBaseForAdjustmentValueMultimethod(
                CalculationTicket ticket,
                Decimal? baseWherePortfoioAdjustmentSetTotal,
                Decimal? porfolioAdjustmentTotal,
                ICalculationTracer tracer
            )
            {
                this.ticket = ticket;
                this.baseWherePortfoioAdjustmentSetTotal = baseWherePortfoioAdjustmentSetTotal;
                this.porfolioAdjustmentTotal = porfolioAdjustmentTotal;
                this.tracer = tracer;
            }
            public Decimal? Result { get; private set; }
            public void Resolve(BasketCountryModel model)
            {
                this.Result = CalculateRescaledBase(
                    model.Base.EditedValue,
                    model.PortfolioAdjustment.EditedValue,
                    this.baseWherePortfoioAdjustmentSetTotal,
                    this.porfolioAdjustmentTotal,
                    tracer
                );
            }
            public void Resolve(BasketRegionModel model)
            {
                this.Result = CalculateRescaledBase(
                    model.Base.EditedValue,
                    model.PortfolioAdjustment.EditedValue,
                    this.baseWherePortfoioAdjustmentSetTotal,
                    this.porfolioAdjustmentTotal,
                    tracer
                );
            }
            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.Result = CalculateRescaledBase(
                    model.Base.EditedValue,
                    model.PortfolioAdjustment.EditedValue,
                    this.baseWherePortfoioAdjustmentSetTotal,
                    this.porfolioAdjustmentTotal,
                    tracer
                );
            }
            public void Resolve(CountryModel model)
            {
                this.Result = null;
            }
            public void Resolve(OtherModel model)
            {
                this.Result = null;
            }
            public void Resolve(RegionModel model)
            {
                this.Result = null;
            }
        }



    }
}
