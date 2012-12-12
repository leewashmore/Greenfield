using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace GreenField.Targeting.Server.BasketTargets
{
    [DataContract(Name = "BtSecurityModel")]
    public class SecurityModel
    {
        [DebuggerStepThrough]
        public SecurityModel()
        {
            this.PortfolioTargets = new List<PortfolioTargetModel>();
        }

        [DebuggerStepThrough]
        public SecurityModel(
            Aims.Data.Server.SecurityModel security,
            EditableExpressionModel baseExpression,
            ExpressionModel benchmarkExpression,
            IEnumerable<PortfolioTargetModel> portfolioTargets
        )
            : this()
        {
            this.Security = security;
            this.Base = baseExpression;
            this.Benchmark = benchmarkExpression;
            this.PortfolioTargets.AddRange(portfolioTargets);
        }


        [DataMember]
        public Aims.Data.Server.SecurityModel Security { get; set; }
        [DataMember]
        public EditableExpressionModel Base { get; set; }
        [DataMember]
        public ExpressionModel Benchmark { get; set; }
        [DataMember]
        public List<PortfolioTargetModel> PortfolioTargets { get; set; }
    }
}
