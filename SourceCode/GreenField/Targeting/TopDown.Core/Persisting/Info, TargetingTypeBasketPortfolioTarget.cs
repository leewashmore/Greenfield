using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	/// <summary>
	/// Represents a record of the TARGETING_TYPE_BASKET_PORTFOLIO_TARGET table.
	/// </summary>
    public class TargetingTypeBasketPortfolioTargetInfo
    {
        [DebuggerStepThrough]
        public TargetingTypeBasketPortfolioTargetInfo()
        {
        }

        [DebuggerStepThrough]
        public TargetingTypeBasketPortfolioTargetInfo(Int32 targetingTypeId, Int32 basketId, String portfolioId, Decimal target, Int32 changeId)
        {
            this.TargetingTypeId = targetingTypeId;
            this.BasketId = basketId;
            this.PortfolioId = portfolioId;
            this.Target = target;
            this.ChangeId = changeId;
        }


		/// <summary>
		/// TARGETING_TYPE_ID column.
		/// </summary>
        public Int32 TargetingTypeId { get; set; }

		/// <summary>
		/// BASKET_ID column.
		/// </summary>
        public Int32 BasketId { get; set; }

		/// <summary>
		/// PORTFOLIO_ID column.
		/// </summary>
        public String PortfolioId { get; set; }

		/// <summary>
		/// TARGET column.
		/// </summary>
        public Decimal Target { get; set; }

		/// <summary>
		/// CHANGE_ID column.
		/// </summary>
        public Int32 ChangeId { get; set; }
    }
}
