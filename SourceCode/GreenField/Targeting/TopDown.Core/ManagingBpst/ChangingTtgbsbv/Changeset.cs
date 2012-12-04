using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpst.ChangingTtgbsbv
{
    public class Changeset
    {
        [DebuggerStepThrough]
        public Changeset(
			Int32 targetingTypeGroupId,
			Int32 basketId, 
			TargetingTypeGroupBasketSecurityBaseValueChangesetInfo latestChangesetInfo,
			String username,
			IEnumerable<IChange> changes
		)
        {
			this.TargetingTypeGroupId = targetingTypeGroupId;
			this.BasketId = basketId;
            this.LatestChangesetSnapshot = latestChangesetInfo;
            this.Username = username;
            this.Changes = changes.ToList();
        }

		public Int32 TargetingTypeGroupId { get; set; }
		public Int32 BasketId { get; set; }
		public TargetingTypeGroupBasketSecurityBaseValueChangesetInfo LatestChangesetSnapshot { get; private set; }
        public String Username { get; private set; }
        public IEnumerable<IChange> Changes { get; private set; }
    }
}
