using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class ModelExpressionTraverser
    {
        private GlobeTraverser traverser;

        [DebuggerStepThrough]
        public ModelExpressionTraverser(GlobeTraverser traverser)
        {
            this.traverser = traverser;
        }

        public IEnumerable<IExpression> Traverse(RootModel root)
        {
            var result = new List<IExpression>();
            this.TraverseGlobe(root.Globe, result);
            this.TraverseOverlay(root.Factors, result);
            result.Add(root.PortfolioScaledTotal);
            return result;
        }

        protected void TraverseGlobe(GlobeModel globe, List<IExpression> result)
        {
            var models = this.traverser.TraverseGlobe(globe);
            foreach (var model in models)
            {
                this.TraverseOnceResolved(model, result);
            }
            result.Add(globe.Base);
            result.Add(globe.Benchmark);
            result.Add(globe.Overlay);
            result.Add(globe.PortfolioAdjustment);
            result.Add(globe.PortfolioScaled);
            result.Add(globe.TrueActive);
            result.Add(globe.TrueExposure);
        }

        private void TraverseOnceResolved(IModel model, List<IExpression> result)
        {
            var resolver = new TraverseOnceResolved_IModelResolver(this, result);
            model.Accept(resolver);
        }

        private class TraverseOnceResolved_IModelResolver : IModelResolver
        {
            private List<IExpression> result;
            private ModelExpressionTraverser traverser;

            public TraverseOnceResolved_IModelResolver(ModelExpressionTraverser traverser, List<IExpression> result)
            {
                this.traverser = traverser;
                this.result = result;
            }

            public void Resolve(BasketCountryModel model)
            {
                this.traverser.TraverseBasketCountry(model, this.result);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.traverser.TraverseBasketRegion(model, this.result);
            }

            public void Resolve(CountryModel model)
            {
                this.traverser.TraverseCountry(model, this.result);
            }

            public void Resolve(OtherModel model)
            {
                this.traverser.TraverseOther(model, this.result);
            }

            public void Resolve(RegionModel model)
            {
                this.traverser.TraverseRegion(model, this.result);
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.traverser.TraverseUnsavedBasketCountry(model, this.result);
            }
        }

        protected void TraverseBasketCountry(BasketCountryModel model, List<IExpression> result)
        {
            result.Add(model.Base);
            result.Add(model.BaseActive);
            result.Add(model.Benchmark);
            result.Add(model.Overlay);
            result.Add(model.PortfolioAdjustment);
            result.Add(model.PortfolioScaled);
            result.Add(model.TrueActive);
            result.Add(model.TrueExposure);
        }

        protected void TraverseBasketRegion(BasketRegionModel model, List<IExpression> result)
        {
            result.Add(model.Base);
            result.Add(model.BaseActive);
            result.Add(model.Benchmark);
            result.Add(model.Overlay);
            result.Add(model.PortfolioAdjustment);
            result.Add(model.PortfolioScaled);
            result.Add(model.TrueActive);
            result.Add(model.TrueExposure);
        }

        protected void TraverseCountry(CountryModel model, List<IExpression> result)
        {
            result.Add(model.Benchmark);
            result.Add(model.Overlay);
        }

        protected void TraverseOther(OtherModel model, List<IExpression> result)
        {
            result.Add(model.Base);
            result.Add(model.BaseActive);
            result.Add(model.Benchmark);
            result.Add(model.Overlay);
            result.Add(model.PortfolioAdjustment);
            result.Add(model.PortfolioScaled);
            result.Add(model.TrueActive);
            result.Add(model.TrueExposure);
        }

        protected void TraverseRegion(RegionModel model, List<IExpression> result)
        {
            result.Add(model.Base);
            result.Add(model.BaseActive);
            result.Add(model.Benchmark);
            result.Add(model.Overlay);
            result.Add(model.PortfolioAdjustment);
            result.Add(model.PortfolioScaled);
            result.Add(model.TrueActive);
            result.Add(model.TrueExposure);
        }

        protected void TraverseUnsavedBasketCountry(UnsavedBasketCountryModel model, List<IExpression> result)
        {
            result.Add(model.Base);
            result.Add(model.BaseActive);
            result.Add(model.Benchmark);
            result.Add(model.Overlay);
            result.Add(model.PortfolioAdjustment);
            result.Add(model.PortfolioScaled);
            result.Add(model.TrueActive);
            result.Add(model.TrueExposure);
        }

        protected void TraverseOverlay(Overlaying.RootModel model, List<IExpression> result)
        {
            model.Items.ForEach(item => result.Add(item.OverlayFactor));
        }
    }
}
