using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class BaseWherePortfoioAdjustmentSetTotalFormula : IFormula<Decimal?>
    {
        private GlobeModel root;
        private GlobeTraverser traverser;

        public BaseWherePortfoioAdjustmentSetTotalFormula(GlobeModel root, GlobeTraverser traverser)
        {
            this.root = root;
            this.traverser = traverser;
        }

        public Decimal? Calculate(CalculationTicket ticket)
        {
            var result = this.Calculate(ticket, No.CalculationTracer);
            return result;

        }

        public Decimal? Calculate(CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Base where portfolio adjustment is set");
            tracer.Indent();
            var models = this.traverser.TraverseGlobe(this.root);
            var result = this.ComputeBaseTotalWherePortfoioAdjustmentSetValue(models, tracer);
            tracer.WriteValue("Total", result);
            tracer.Unindent();
            return result;
        }

        protected Decimal? ComputeBaseTotalWherePortfoioAdjustmentSetValue(IEnumerable<IModel> models, ICalculationTracer tracer)
        {
            Decimal? total = null;
            foreach (var model in models)
            {
                var resolver = new BaseTotalWherePortfoioAdjustmentSetValueMultimethod();
                model.Accept(resolver);
                var value = resolver.Result;
                if (value.HasValue)
                {
                    tracer.WriteValue("+", value);
                    if (!total.HasValue)
                    {
                        total = value;
                    }
                    else
                    {
                        total += value;
                    }
                }
            }
            return total;
        }



        private class BaseTotalWherePortfoioAdjustmentSetValueMultimethod : IModelResolver
        {
            public Decimal? Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = model.PortfolioAdjustment.EditedValue.HasValue ? model.Base.EditedValue : null;
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = model.PortfolioAdjustment.EditedValue.HasValue ? model.Base.EditedValue : null;
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.Result = model.PortfolioAdjustment.EditedValue.HasValue ? model.Base.EditedValue : null;
            }

            public void Resolve(CountryModel model) { }

            public void Resolve(OtherModel model) { }

            public void Resolve(RegionModel model) { }
        }



    }
}
