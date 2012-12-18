using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    [DataContract]
    public class CashModel
    {
        [DebuggerStepThrough]
        public CashModel()
        {
        }


        [DebuggerStepThrough]
        public CashModel(NullableExpressionModel baseExpression, NullableExpressionModel scaledExpression, NullableExpressionModel trueExposureExpression, NullableExpressionModel trueActiveExpression)
            : this()
        {
            this.Base = baseExpression;
            this.Scaled = scaledExpression;
            this.TrueExposure = trueExposureExpression;
            this.TrueActive = trueActiveExpression;
        }

        [DataMember]
        public NullableExpressionModel Base { get; set; }
        [DataMember]
        public NullableExpressionModel Scaled { get; set; }
        [DataMember]
        public NullableExpressionModel TrueExposure { get; set; }
        [DataMember]
        public NullableExpressionModel TrueActive { get; set; }
    }
}
