using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract]
    public class BasketCountryModel : GlobeResident
    {
        [DebuggerStepThrough]
        public BasketCountryModel()
        {
        }

        [DebuggerStepThrough]
        public BasketCountryModel(
            EditableExpressionModel baseExpression,
            NullableExpressionModel baseActiveExpression,
            CountryBasketModel basket,
            ExpressionModel benchmarkExpression,
            ExpressionModel overlayExpression,
            EditableExpressionModel portfolioAdjustmentExpression,
            NullableExpressionModel portfolioScaledExpression,
            NullableExpressionModel trueActiveExpression,
            NullableExpressionModel trueExposureExpression
        )
        {
            this.Base = baseExpression;
            this.BaseActive = baseActiveExpression;
            this.Basket = basket;
            this.Benchmark = benchmarkExpression;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueActive = trueActiveExpression;
            this.TrueExposure = trueExposureExpression;
        }

        [DataMember]
        public EditableExpressionModel Base { get; set; }

        [DataMember]
        public NullableExpressionModel BaseActive { get; set; }

        [DataMember]
        public CountryBasketModel Basket { get; set; }

        [DataMember]
        public ExpressionModel Benchmark { get; set; }

        [DataMember]
        public ExpressionModel Overlay { get; set; }

        [DataMember]
        public EditableExpressionModel PortfolioAdjustment { get; set; }

        [DataMember]
        public NullableExpressionModel PortfolioScaled { get; set; }

        [DataMember]
        public NullableExpressionModel TrueActive { get; set; }

        [DataMember]
        public NullableExpressionModel TrueExposure { get; set; }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
