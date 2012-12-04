using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the TARGETING_TYPE_PORTFOLIO table.
    /// </summary>
	public class TargetingTypePortfolioInfo
	{
		[DebuggerStepThrough]
		public TargetingTypePortfolioInfo()
		{
		}

		[DebuggerStepThrough]
		public TargetingTypePortfolioInfo(Int32 targetingTypeIf, String portfolioId)
		{
			this.TargetingTypeId = targetingTypeIf;
			this.PortfolioId = portfolioId;
		}

		public Int32 TargetingTypeId { get; set; }
		
		/// <summary>
		/// Either broad global active or bottom-up portfolio ID.
		/// </summary>
		public String PortfolioId { get; set; }
	}
}
