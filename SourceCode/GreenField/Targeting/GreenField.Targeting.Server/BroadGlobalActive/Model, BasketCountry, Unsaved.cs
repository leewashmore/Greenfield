using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract]
    public class UnsavedBasketCountryModel : GlobeResident
    {

        [DebuggerStepThrough]
        public UnsavedBasketCountryModel()
        {
        }

        [DebuggerStepThrough]
        public UnsavedBasketCountryModel(
            EditableExpressionModel baseExpression,
            NullableExpressionModel baseActiveExpression,
            ExpressionModel benchmarkExpression,
            Aims.Data.Server.CountryModel country,
            ExpressionModel overlayExpression,
            EditableExpressionModel portfolioAdjustmentExpression,
            NullableExpressionModel portfolioScaledExpression,
            NullableExpressionModel trueActiveExpression,
            NullableExpressionModel trueExposureExpression
        )
        {
            this.Base = baseExpression;
            this.BaseActive = baseActiveExpression;
            this.Benchmark = benchmarkExpression;
            this.Country = country;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueActive = trueActiveExpression;
            this.TrueExposure = trueExposureExpression;
        }

        [DataMember]
        public EditableExpressionModel Base { get; private set; }

        [DataMember]
        public NullableExpressionModel BaseActive { get; private set; }

        [DataMember]
        public ExpressionModel Benchmark { get; private set; }

        [DataMember]
        public Aims.Data.Server.CountryModel Country { get; private set; }

        [DataMember]
        public ExpressionModel Overlay { get; set; }

        [DataMember]
        public EditableExpressionModel PortfolioAdjustment { get; private set; }

        [DataMember]
        public NullableExpressionModel PortfolioScaled { get; private set; }

        [DataMember]
        public NullableExpressionModel TrueActive { get; private set; }

        [DataMember]
        public NullableExpressionModel TrueExposure { get; private set; }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
