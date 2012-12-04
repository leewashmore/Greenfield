using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{

	/// <summary>
	/// Represents a record of the BGA_PORTFOLIO_SECURITY_FACTOR table.
	/// Answer the question what is the fraction (target overlay) of a bottom-up portfolio (securityId) in a broad-global-active portfolio (portfolioId).
	/// </summary>
    public class BgaPortfolioSecurityFactorInfo
    {
        [DebuggerStepThrough]
        public BgaPortfolioSecurityFactorInfo()
        {
        }

        [DebuggerStepThrough]
        public BgaPortfolioSecurityFactorInfo(String broadGlobalActivePortfolioId, String securityId, Decimal factor, Int32 changeId)
        {
            this.BroadGlobalActivePortfolioId = broadGlobalActivePortfolioId;
            this.SecurityId = securityId;
            this.Factor = factor;
            this.ChangeId = changeId;
        }

		/// <summary>
		/// Broad-global-active portfolio ID.
		/// PORTFOLIO_ID
		/// </summary>
        public String BroadGlobalActivePortfolioId { get; set; }

		/// <summary>
		/// Bottom-up protfolio ID.
		/// SECURITY_ID
		/// </summary>
        public String SecurityId { get; set; }

		/// <summary>
		/// FACTOR
		/// </summary>
        public Decimal Factor { get; set; }

		/// <summary>
		/// CHANGE_ID
		/// </summary>
        public Int32 ChangeId { get; set; }
    }
}
