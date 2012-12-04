using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingBpst.ChangingBpst
{
    public class Changeset
    {
        [DebuggerStepThrough]
        public Changeset(
			BasketPortfolioSecurityTargetChangesetInfo latestChangesetInfo,
            Int32 basketId,
			String username,
			IEnumerable<IChange> changes
		)
        {
            this.BasketId = basketId;
            this.LatestChangesetSnapshot = latestChangesetInfo;
            this.Username = username;
            this.Changes = changes.ToList();
        }

        public BasketPortfolioSecurityTargetChangesetInfo LatestChangesetSnapshot { get; private set; }
        public String Username { get; private set; }
        public IEnumerable<IChange> Changes { get; private set; }
        public Int32 BasketId { get; private set; }
    }
}
