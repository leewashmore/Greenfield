using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions.ManagingBpt;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class ExpressionPicker
    {
        public ExpressionPicker()
        {
            this.Benchmark = new BenchmarkPicker();
            this.Base = new BasePicker();
            this.BaseActive = new BaseActivePicker();
            this.Overlay = new OverlayPicker();
            this.PortfolioAdjustment = new PortfolioAdjustmentPicker();
            this.PortfolioScaled = new PortfolioScaledPicker();
            this.TrueExposure = new TrueExposurePicker();
            this.TrueActive = new TreuActivePicker();
        }


        public IExpressionPicker<IModel, Decimal?> Base { get; private set; }
        public IExpressionPicker<IModel, Decimal?> BaseActive { get; private set; }
        public IExpressionPicker<IModel, Decimal> Benchmark { get; private set; }
        public IExpressionPicker<IModel, Decimal> Overlay { get; private set; }
        public IExpressionPicker<IModel, Decimal?> PortfolioAdjustment { get; private set; }
        public IExpressionPicker<IModel, Decimal?> PortfolioScaled { get; private set; }
        public IExpressionPicker<IModel, Decimal?> TrueExposure { get; private set; }
        public IExpressionPicker<IModel, Decimal?> TrueActive { get; private set; }


        private class BenchmarkPicker : IExpressionPicker<IModel, Decimal>
        {
            public IExpression<Decimal> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver();
                model.Accept(resolver);
                return resolver.Result;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                public IExpression<Decimal> Result { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.Result = model.Benchmark;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.Result = model.Benchmark;
                }

                public void Resolve(CountryModel model)
                {
                    this.Result = model.Benchmark;
                }

                public void Resolve(OtherModel model)
                {
                    this.Result = model.Benchmark;
                }

                public void Resolve(RegionModel model)
                {
                    this.Result = model.Benchmark;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.Result = model.Benchmark;
                }
            }
        }

        private class BasePicker : IExpressionPicker<IModel, Decimal?>
        {
            public IExpression<Decimal?> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver();
                model.Accept(resolver);
                return resolver.ResultOpt;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                public IExpression<Decimal?> ResultOpt { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.ResultOpt = model.Base;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.ResultOpt = model.Base;
                }

                public void Resolve(CountryModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(OtherModel model)
                {
                    this.ResultOpt = model.Base;
                }

                public void Resolve(RegionModel model)
                {
                    this.ResultOpt = model.Base;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.ResultOpt = model.Base;
                }
            }
        }

        private class BaseActivePicker : IExpressionPicker<IModel, Decimal?>
        {
            public IExpression<Decimal?> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver(this);
                model.Accept(resolver);
                return resolver.ResultOpt;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                private BaseActivePicker picker;

                public PickExpression_IModelResolver(BaseActivePicker picker)
                {
                    this.picker = picker;
                }

                public IExpression<Decimal?> ResultOpt { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.ResultOpt = model.BaseActive;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.ResultOpt = model.BaseActive;
                }

                public void Resolve(CountryModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(OtherModel model)
                {
                    this.ResultOpt = model.BaseActive;
                }

                public void Resolve(RegionModel model)
                {
                    this.ResultOpt = model.BaseActive;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.ResultOpt = model.BaseActive;
                }
            }
        }

        private class OverlayPicker : IExpressionPicker<IModel, Decimal>
        {
            public IExpression<Decimal> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver(this);
                model.Accept(resolver);
                return resolver.Result;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                private OverlayPicker picker;

                public PickExpression_IModelResolver(OverlayPicker picker)
                {
                    this.picker = picker;
                }

                public IExpression<Decimal> Result { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.Result = model.Overlay;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.Result = model.Overlay;
                }

                public void Resolve(CashModel model)
                {
                    this.Result = null;
                }

                public void Resolve(CountryModel model)
                {
                    this.Result = model.Overlay;
                }

                public void Resolve(OtherModel model)
                {
                    this.Result = model.Overlay;
                }

                public void Resolve(RegionModel model)
                {
                    this.Result = model.Overlay;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.Result = model.Overlay;
                }
            }
        }

        private class PortfolioAdjustmentPicker : IExpressionPicker<IModel, Decimal?>
        {
            public IExpression<Decimal?> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver();
                model.Accept(resolver);
                return resolver.ResultOpt;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                public IExpression<Decimal?> ResultOpt { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.ResultOpt = model.PortfolioAdjustment;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.ResultOpt = model.PortfolioAdjustment;
                }

                public void Resolve(CashModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(CountryModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(OtherModel model)
                {
                    this.ResultOpt = model.PortfolioAdjustment;
                }

                public void Resolve(RegionModel model)
                {
                    this.ResultOpt = model.PortfolioAdjustment;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.ResultOpt = model.PortfolioAdjustment;
                }
            }
        }

        private class PortfolioScaledPicker : IExpressionPicker<IModel, Decimal?>
        {
            public IExpression<Decimal?> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver(this);
                model.Accept(resolver);
                return resolver.ResultOpt;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                private PortfolioScaledPicker picker;

                public PickExpression_IModelResolver(PortfolioScaledPicker picker)
                {
                    this.picker = picker;
                }

                public IExpression<Decimal?> ResultOpt { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.ResultOpt = model.PortfolioScaled;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.ResultOpt = model.PortfolioScaled;
                }

                public void Resolve(CountryModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(OtherModel model)
                {
                    this.ResultOpt = model.PortfolioScaled;
                }

                public void Resolve(RegionModel model)
                {
                    this.ResultOpt = model.PortfolioScaled;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.ResultOpt = model.PortfolioScaled;
                }
            }
        }

        private class TrueExposurePicker : IExpressionPicker<IModel, Decimal?>
        {
            public IExpression<Decimal?> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver();
                model.Accept(resolver);
                return resolver.ResultOpt;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                public IExpression<Decimal?> ResultOpt { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.ResultOpt = model.TrueExposure;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.ResultOpt = model.TrueExposure;
                }

                public void Resolve(CountryModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(OtherModel model)
                {
                    this.ResultOpt = model.TrueExposure;
                }

                public void Resolve(RegionModel model)
                {
                    this.ResultOpt = model.TrueExposure;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.ResultOpt = model.TrueExposure;
                }
            }
        }

        private class TreuActivePicker : IExpressionPicker<IModel, Decimal?>
        {
            public IExpression<Decimal?> TryPickExpression(IModel model)
            {
                var resolver = new PickExpression_IModelResolver();
                model.Accept(resolver);
                return resolver.ResultOpt;
            }

            private class PickExpression_IModelResolver : IModelResolver
            {
                public IExpression<Decimal?> ResultOpt { get; private set; }

                public void Resolve(BasketCountryModel model)
                {
                    this.ResultOpt = model.TrueActive;
                }

                public void Resolve(BasketRegionModel model)
                {
                    this.ResultOpt = model.TrueActive;
                }

                public void Resolve(CountryModel model)
                {
                    this.ResultOpt = null;
                }

                public void Resolve(OtherModel model)
                {
                    this.ResultOpt = model.TrueActive;
                }

                public void Resolve(RegionModel model)
                {
                    this.ResultOpt = model.TrueActive;
                }

                public void Resolve(UnsavedBasketCountryModel model)
                {
                    this.ResultOpt = model.TrueActive;
                }
            }
        }
    }
}
