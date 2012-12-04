using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ServiceModel;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract]
    public class RegionModel : GlobeResident
    {
        [DebuggerStepThrough]
        public RegionModel()
        {
            this.Residents = new List<GlobeResident>();
        }

        [DebuggerStepThrough]
        public RegionModel(
            NullableExpressionModel baseExpression,
            NullableExpressionModel baseActiveExpression,
            ExpressionModel benchmarkExpression,
            String name,
            ExpressionModel overlayExpression,
            NullableExpressionModel portfolioAdjustmentExpression,
            NullableExpressionModel portfolioScaledExpression,
            IEnumerable<GlobeResident> residents,
            NullableExpressionModel trueActiveExpression,
            NullableExpressionModel trueExposureExpression
        ) : this()
        {
            this.Base = baseExpression;
            this.BaseActive = baseActiveExpression;
            this.Benchmark = benchmarkExpression;
            this.Name = name;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.Residents.AddRange(residents);
            this.TrueActive = trueActiveExpression;
            this.TrueExposure = trueExposureExpression;
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
        public List<GlobeResident> Residents { get; set; }

        [DataMember]
        public NullableExpressionModel TrueActive { get; set; }

        [DataMember]
        public NullableExpressionModel TrueExposure { get; set; }

        [DataMember]
        public String Name { get; set; }

        [DebuggerStepThrough]
        public override void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
