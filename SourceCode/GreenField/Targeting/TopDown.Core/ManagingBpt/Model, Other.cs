using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class OtherModel : IGlobeResident, IModel
    {
        protected OtherModel()
        {
            this.BasketCountries = new List<BasketCountryModel>();
            this.UnsavedBasketCountries = new List<UnsavedBasketCountryModel>();
        }

        [DebuggerStepThrough]
        public OtherModel(
            IExpression<Decimal> benchmarkExpression,
            IExpression<Decimal?> baseExpression,
            IExpression<Decimal?> baseActiveExpression,
            IExpression<Decimal> overlayExpression,
            IExpression<Decimal?> portfolioAdjustmentExpression,
            IExpression<Decimal?> portfolioScaledExpression,
            IExpression<Decimal?> trueExposureExpression,
            IExpression<Decimal?> trueActiveExpression,
            List<BasketCountryModel> basketCountries,
            List<UnsavedBasketCountryModel> unsavedBasketCountries
        )
            : this()
        {
            this.Benchmark = benchmarkExpression;
            this.Base = baseExpression;
            this.BaseActive = baseActiveExpression;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueExposure = trueExposureExpression;
            this.TrueActive = trueActiveExpression;
            this.BasketCountries = basketCountries;
            this.UnsavedBasketCountries = unsavedBasketCountries;
        }

        public IExpression<Decimal> Benchmark { get; private set; }
        public IExpression<Decimal?> Base { get; private set; }
        public IExpression<Decimal?> BaseActive { get; private set; }
        public IExpression<Decimal> Overlay { get; private set; }
        public IExpression<Decimal?> PortfolioAdjustment { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<Decimal?> TrueExposure { get; private set; }
        public IExpression<Decimal?> TrueActive { get; private set; }

        public List<BasketCountryModel> BasketCountries { get; private set; }
        public List<UnsavedBasketCountryModel> UnsavedBasketCountries { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(IModelResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
