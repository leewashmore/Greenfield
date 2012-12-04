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
        public CashModel(NullableExpressionModel baseExpression, NullableExpressionModel scaledExpression)
            : this()
        {
            this.Base = baseExpression;
            this.Scaled = scaledExpression;
        }

        [DataMember]
        public NullableExpressionModel Base { get; set; }

        [DataMember]
        public NullableExpressionModel Scaled { get; set; }
    }
}
