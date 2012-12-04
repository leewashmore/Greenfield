using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract]
    public class GlobeModel
    {
        [DebuggerStepThrough]
        public GlobeModel()
        {
            this.Residents = new List<GlobeResident>();
        }

        [DebuggerStepThrough]
        public GlobeModel(
            IEnumerable<GlobeResident> residents,
            NullableExpressionModel baseExpression,
            ExpressionModel benchmarkExpression,
            ExpressionModel overlayExpression,
            NullableExpressionModel portfolioAdjustmentExpression,
            NullableExpressionModel portfolioScaledExpression,
            NullableExpressionModel trueActiveExpression,
            NullableExpressionModel trueExposureExpression
        ) : this()
        {
            this.Residents.AddRange(residents.ToList());
            this.Base = baseExpression;
            this.Benchmark = benchmarkExpression;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueActive = trueActiveExpression;
            this.TrueExposure = trueExposureExpression;
        }

        [DataMember]
        public NullableExpressionModel Base { get; set; }
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
        public List<GlobeResident> Residents { get; set; }
    }
}
