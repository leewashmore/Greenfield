using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	public class BasketPortfolioSecurityTargetInfo
	{
		[DebuggerStepThrough]
		public BasketPortfolioSecurityTargetInfo()
		{
		}

		[DebuggerStepThrough]
		public BasketPortfolioSecurityTargetInfo(Int32 basketId, String portfolioId, String securityId, Decimal target, Int32 changeId)
		{
			this.BasketId = basketId;
			this.PortfolioId = portfolioId;
			this.SecurityId = securityId;
			this.Target = target;
			this.ChangeId = changeId;
		}

		public Int32 BasketId { get; set; }
		public String PortfolioId { get; set; }
		public String SecurityId { get; set; }
		public Decimal Target { get; set; }
		public Int32 ChangeId { get; set; }
	}
}
