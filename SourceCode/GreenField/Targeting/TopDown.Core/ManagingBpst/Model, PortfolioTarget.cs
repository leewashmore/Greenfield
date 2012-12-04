using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;
using TopDown.Core.ManagingPortfolios;

namespace TopDown.Core.ManagingBpst
{
	public class PortfolioTargetModel
	{
		[DebuggerStepThrough]
		public PortfolioTargetModel(BroadGlobalActivePortfolio broadGlobalActivePortfolio, EditableExpression portfolioTargetExpression)
		{
            this.BroadGlobalActivePortfolio = broadGlobalActivePortfolio;
			this.Target = portfolioTargetExpression;
		}

        public BroadGlobalActivePortfolio BroadGlobalActivePortfolio { get; private set; }
		public EditableExpression Target { get; private set; }
    }
}
