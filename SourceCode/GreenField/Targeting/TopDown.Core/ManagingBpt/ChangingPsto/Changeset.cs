using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingPsto
{
	public class Changeset
	{
		[DebuggerStepThrough]
		public Changeset(
			String portfolioId,
			BgaPortfolioSecurityFactorChangesetInfo latestChangesetInfo,
			String username,
			IEnumerable<IChange> changes
		)
		{
			this.PortfolioId = portfolioId;
			this.LatestChangesetSnapshot = latestChangesetInfo;
			this.Username = username;
			this.Changes = changes.ToList();
		}

		public String PortfolioId { get; private set; }
		public BgaPortfolioSecurityFactorChangesetInfo LatestChangesetSnapshot { get; private set; }
		public String Username { get; private set; }
		public IEnumerable<IChange> Changes { get; private set; }
    }
}
