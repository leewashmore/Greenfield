using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract]
    public class OtherModel : GlobeResident
    {
        [DebuggerStepThrough]
        public OtherModel()
        {
            this.BasketCountries = new List<BasketCountryModel>();
            this.UnsavedBasketCountries = new List<UnsavedBasketCountryModel>();
        }

        [DebuggerStepThrough]
        public OtherModel(
            NullableExpressionModel baseExpression,
            NullableExpressionModel baseActiveExpression,
            IEnumerable<BasketCountryModel> savedBasketCountries,
            ExpressionModel benchmarkExpression,
            ExpressionModel overlayExpression,
            NullableExpressionModel portfolioAdjustmentExpression,
            NullableExpressionModel portfolioScaledExpression,
            NullableExpressionModel trueActiveExpression,
            NullableExpressionModel trueExposureExpression,
            IEnumerable<UnsavedBasketCountryModel> unsavedBasketCountries
        )
            : this()
        {
            this.Base = baseExpression;
            this.BaseActive = baseActiveExpression;
            this.Benchmark = benchmarkExpression;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueActive = trueActiveExpression;
            this.TrueExposure = trueExposureExpression;
            this.BasketCountries.AddRange(savedBasketCountries);
            this.UnsavedBasketCountries.AddRange(unsavedBasketCountries);
        }

        [DataMember]
        public NullableExpressionModel Base { get; set; }

        [DataMember]
        public NullableExpressionModel BaseActive { get; set; }

        [DataMember]
        public ExpressionModel Benchmark { get; set; }

        [DataMember]
        public ExpressionModel Overlay { get; set; }

        [DataMember]
        public NullableExpressionModel PortfolioAdjustment { get; set; }

        [DataMember]
        public NullableExpressionModel PortfolioScaled { get; set; }

        [DataMember]
        public NullableExpressionModel TrueActive { get; set; }

        [DataMember]
        public NullableExpressionModel TrueExposure { get; set; }

        [DataMember]
        public List<BasketCountryModel> BasketCountries { get; set; }

        [DataMember]
        public List<UnsavedBasketCountryModel> UnsavedBasketCountries { get; set; }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
