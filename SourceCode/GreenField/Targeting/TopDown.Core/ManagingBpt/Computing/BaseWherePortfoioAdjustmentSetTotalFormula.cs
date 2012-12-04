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
            var models = this.traverser.TraverseGlobe(this.root);
            var root = this.ComputeBaseTotalWherePortfoioAdjustmentSetValue(models);
            return root;
        }

        protected Decimal? ComputeBaseTotalWherePortfoioAdjustmentSetValue(IEnumerable<IModel> models)
        {
            Decimal? total = null;
            foreach (var model in models)
            {
                var resolver = new BaseTotalWherePortfoioAdjustmentSetValueMultimethod();
                model.Accept(resolver);
                var value = resolver.Result;

                if (value.HasValue)
                {
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
