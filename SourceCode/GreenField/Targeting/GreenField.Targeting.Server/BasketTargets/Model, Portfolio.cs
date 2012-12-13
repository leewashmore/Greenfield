using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Aims.Data.Server;

namespace GreenField.Targeting.Server.BasketTargets
{
    [DataContract(Name = "BtPorfolioModel")]
    public class PortfolioModel
    {
        [DebuggerStepThrough]
        public PortfolioModel(
            BroadGlobalActivePortfolioModel broadGlobalActivePortfolio,
            NullableExpressionModel portfolioTargetTotalExpression
        )
        {
            this.BroadGlobalActivePortfolio = broadGlobalActivePortfolio;
            this.PortfolioTargetTotal = portfolioTargetTotalExpression;
        }

        [DataMember]
        public BroadGlobalActivePortfolioModel BroadGlobalActivePortfolio { get; set; }
        [DataMember]
        public NullableExpressionModel PortfolioTargetTotal { get; set; }
    }
}
