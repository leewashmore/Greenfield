using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingBaskets;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
	public class BasketCountryModel : IRegionModelResident, IModel
	{
		[DebuggerStepThrough]
		public BasketCountryModel(
			CountryBasket basket,
			UnchangableExpression<Decimal> benchmarkExpression,
			EditableExpression baseExpression,
			Func<BasketCountryModel, IExpression<Decimal?>> baseActiveExpressionCreator,
			UnchangableExpression<Decimal> overlayExpression,
			EditableExpression portfolioAdjustmentExpression,
            Func<BasketCountryModel, IExpression<Decimal?>> portfolioScaledExpressionCreator,
            Func<BasketCountryModel, IExpression<Decimal?>> trueExposureExpressionCreator,
            Func<BasketCountryModel, IExpression<Decimal?>> trueActiveExpressionCreator,
			CommonParts commonParts
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
		}
       
		public CountryBasket Basket { get; set; }
		public UnchangableExpression<Decimal> Benchmark { get; private set; }
		public EditableExpression Base { get; private set; }
		public IExpression<Decimal?> BaseActive { get; private set; }
		public UnchangableExpression<Decimal> Overlay { get; private set; }
		public EditableExpression PortfolioAdjustment { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<Decimal?> TrueExposure { get; private set; }
        public IExpression<Decimal?> TrueActive { get; private set; }

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
    }
}
