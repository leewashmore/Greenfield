using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingPortfolios;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst
{
    public class PortfolioModel
    {
        [DebuggerStepThrough]
        public PortfolioModel(BroadGlobalActivePortfolio portfolio, IExpression<Decimal?> portfolioTargetTotal)
        {
            this.Portfolio = portfolio;
            this.PortfolioTargetTotal = portfolioTargetTotal;
        }

        public BroadGlobalActivePortfolio Portfolio { get; private set; }
        public IExpression<Decimal?> PortfolioTargetTotal { get; private set; }
    }
}
