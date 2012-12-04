using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Only applicable to bottom-up portfolios.
	/// Answer the question what securities a bottom-up portfolio is made of and what is the weight of each of these securities in the composition.
	/// Represents a record of the BU_PORTFOLIO_SECURITY_TARGET table.
    /// </summary>
    public class BuPortfolioSecurityTargetInfo
    {
        public BuPortfolioSecurityTargetInfo()
        {
        }

        [DebuggerStepThrough]
        public BuPortfolioSecurityTargetInfo(String bottomUpPortfolioId, String securityId, Decimal target, Int32 changeId)
        {
            this.BottomUpPortfolioId = bottomUpPortfolioId;
            this.SecurityId = securityId;
            this.Target = target;
            this.ChangeId = changeId;
        }

        public String BottomUpPortfolioId { get; set; }
        public String SecurityId { get; set; }
        public Decimal Target { get; set; }
        public Int32 ChangeId { get; set; }
    }
}
