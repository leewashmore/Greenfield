using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingPortfolios;
using TopDown.Core.ManagingSecurities;
using System.Diagnostics;

namespace TopDown.Core.ManagingCalculations
{
	public class TargetRecord
	{
		[DebuggerStepThrough]
		public TargetRecord(
			BroadGlobalActivePortfolio portfolio,
            ISecurity security,
			Decimal target
		)
		{
			this.Portfolio = portfolio;
			this.Security = security;
			this.Target = target;
		}

		public BroadGlobalActivePortfolio Portfolio { get; private set; }
        public ISecurity Security { get; private set; }
		public Decimal Target { get; private set; }
	}
}
