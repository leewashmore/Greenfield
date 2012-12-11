using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingCountries;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingBpt
{
    public class UnsavedBasketCountryModel : IModel
    {
        public UnsavedBasketCountryModel(
            Country country,
            UnchangableExpression<Decimal> benchmarkExpression,
            EditableExpression baseExpression,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> baseActiveExpressionCreator,
            UnchangableExpression<Decimal> overlayExpression,
            EditableExpression portfolioAdjustmentExpression,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> portfolioScaledExpressionCreator,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> trueExposureExpressionCreator,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> trueActiveExpressionCreator,
            BasketCountryModel basketCountry
        )
            : this(country, benchmarkExpression, baseExpression, baseActiveExpressionCreator, overlayExpression, portfolioAdjustmentExpression, portfolioScaledExpressionCreator, trueExposureExpressionCreator, trueActiveExpressionCreator)
        {
            this.BasketCountry = basketCountry;
        }

        [DebuggerStepThrough]
        public UnsavedBasketCountryModel(
            Country country,
            UnchangableExpression<Decimal> benchmarkExpression,
            EditableExpression baseExpression,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> baseActiveExpressionCreator,
            UnchangableExpression<Decimal> overlayExpression,
            EditableExpression portfolioAdjustmentExpression,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> portfolioScaledExpressionCreator,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> trueExposureExpressionCreator,
            Func<UnsavedBasketCountryModel, IExpression<Decimal?>> trueActiveExpressionCreator
        )
        {
            this.BasketId = null;
            this.Country = country;
            this.Benchmark = benchmarkExpression;
            this.Base = baseExpression;
            this.BaseActive = baseActiveExpressionCreator(this);
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpressionCreator(this);
            this.TrueExposure = trueExposureExpressionCreator(this);
            this.TrueActive = trueActiveExpressionCreator(this);
        }

        public Country Country { get; private set; }
        [Obsolete("WTF????")]
        public Int32? BasketId { get; set; }
        public UnchangableExpression<Decimal> Benchmark { get; private set; }
        public EditableExpression Base { get; private set; }
        public IExpression<Decimal?> BaseActive { get; private set; }
        public UnchangableExpression<Decimal> Overlay { get; private set; }
        public EditableExpression PortfolioAdjustment { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<Decimal?> TrueExposure { get; private set; }
        public IExpression<Decimal?> TrueActive { get; private set; }
        [Obsolete("WTF????")]
        public BasketCountryModel  BasketCountry { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IModelResolver resolver)
        {
            resolver.Resolve(this);
        }

        
    }
}
