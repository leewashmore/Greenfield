using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingBaskets;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class BasketRegionModel : IRegionModelResident, IModel, IGlobeResident
	{
		[DebuggerStepThrough]
		public BasketRegionModel(
			RegionBasket basket,
			IExpression<Decimal> benchmarkExpression,
			EditableExpression baseExpression,
            Func<BasketRegionModel, IExpression<Decimal?>> baseActiveExpressionCreator,
            IExpression<Decimal> overlayExpression,
			EditableExpression portfolioAdjustmentExpression,
            Func<BasketRegionModel, IExpression<Decimal?>> portfolioScaledExpressionCreator,
            Func<BasketRegionModel, IExpression<Decimal?>> trueExposureExpressionCreator,
            Func<BasketRegionModel, IExpression<Decimal?>> trueActiveExpressionCreator,
			IEnumerable<CountryModel> countries
		)
		{
			this.Basket = basket;
            this.Benchmark = benchmarkExpression;
			this.Base = baseExpression;
            this.BaseActive = baseActiveExpressionCreator(this);
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpressionCreator(this);
            this.TrueExposure = trueExposureExpressionCreator(this);
            this.TrueActive = trueActiveExpressionCreator(this);
			this.Countries = countries;
		}
		public RegionBasket Basket { get; private set; }
		public IExpression<Decimal> Benchmark { get; private set; }
		public EditableExpression Base { get; private set; }
        public IExpression<Decimal?> BaseActive { get; private set; }
		public IExpression<Decimal> Overlay { get; private set; }
		public EditableExpression PortfolioAdjustment { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<Decimal?> TrueExposure { get; private set; }
        public IExpression<Decimal?> TrueActive { get; private set; }
		public IEnumerable<CountryModel> Countries { get; private set; }

		[DebuggerStepThrough]
		public void Accept(IRegionModelResidentResolver resolver)
		{
			resolver.Resolve(this);
		}

		[DebuggerStepThrough]
		public void Accept(IModelResolver resolver)
		{
			resolver.Resolve(this);
		}

        [DebuggerStepThrough]
        public void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        
    }
}
