using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using Aims.Data.Server;

namespace GreenField.Targeting.Server.BasketTargets
{
    [DataContract(Name = "BtPortfolioTargetModel")]
    public class PortfolioTargetModel
    {
        [DebuggerStepThrough]
        public PortfolioTargetModel(
            BroadGlobalActivePortfolioModel broadGlobalActivePortfolio,
            EditableExpressionModel portfolioTargetExpression
        )
        {
            this.BroadGlobalActivePortfolio = broadGlobalActivePortfolio;
            this.PortfolioTarget = portfolioTargetExpression;
        }

        [DataMember]
        public BroadGlobalActivePortfolioModel BroadGlobalActivePortfolio { get; set; }
        [DataMember]
        public EditableExpressionModel PortfolioTarget { get; set; }
    }
}
