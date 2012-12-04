using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingBpt.ChangingTtbpt
{
	public class Changeset
	{
		[DebuggerStepThrough]
		public Changeset(TargetingTypeBasketPortfolioTargetChangesetInfo latestChangesetInfo, Int32 targetingTypeId, String portfolioId, String username, IEnumerable<IChange> changes)
		{
            this.LatestChangesetSnapshot = latestChangesetInfo;
			this.TargetingTypeId = targetingTypeId;
			this.PortfolioId = portfolioId;
            this.Username = username;
            this.Changes = changes.ToList();
		}

        public TargetingTypeBasketPortfolioTargetChangesetInfo LatestChangesetSnapshot { get; private set; }
		public Int32 TargetingTypeId { get; private set; }
		public String PortfolioId { get; private set; }
        public String Username { get; private set; }
        public IEnumerable<IChange> Changes { get; private set; }
    }
}