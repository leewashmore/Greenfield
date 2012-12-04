using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingTtbbv
{
    public class Changeset
    {
        [DebuggerStepThrough]
        public Changeset(Int32 targetingTypeId, TargetingTypeBasketBaseValueChangesetInfo latestChangesetInfo, String username, IEnumerable<IChange> changes)
        {
            this.TargetingTypeId = targetingTypeId;
            this.Username = username;
            this.LatestChangesetSnapshot = latestChangesetInfo;
            this.Changes = changes.ToList();
        }

        public Int32 TargetingTypeId { get; private set; }
        public String Username { get; private set; }
        public TargetingTypeBasketBaseValueChangesetInfo LatestChangesetSnapshot { get; private set; }
        public IEnumerable<IChange> Changes { get; private set; }
    }
}
