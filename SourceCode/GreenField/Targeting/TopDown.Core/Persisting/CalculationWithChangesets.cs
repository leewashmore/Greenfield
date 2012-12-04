using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	public class CalculationWithChangesets
	{
		[DebuggerStepThrough]
		public CalculationWithChangesets()
		{
			this.CalculationInfo = new TargetingCalculationInfo();
		}

		public TargetingCalculationInfo CalculationInfo { get; set; }
		public BasketPortfolioSecurityTargetChangesetInfo BpstChangesetOpt { get; set; }
		public BgaPortfolioSecurityFactorChangesetInfo PsfChangesetOpt { get; set; }
		public BuPortfolioSecurityTargetChangesetInfo PstChangesetOpt { get; set; }
		public TargetingTypeBasketBaseValueChangesetInfo TtbbvChangesetOpt { get; set; }
		public TargetingTypeBasketPortfolioTargetChangesetInfo TtbptChangesetOpt { get; set; }
		public TargetingTypeGroupBasketSecurityBaseValueChangesetInfo TtgbsbvChangesetOpt { get; set; }
	}
}
