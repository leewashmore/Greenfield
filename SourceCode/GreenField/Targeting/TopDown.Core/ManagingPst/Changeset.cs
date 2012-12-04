using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingPst
{
    public class Changeset
    {
        [DebuggerStepThrough]
        public Changeset(BuPortfolioSecurityTargetChangesetInfo latestChangeset, String username, IEnumerable<IPstChange> changes)
        {
            this.LatestChangeset = latestChangeset;
            this.Username = username;
            this.Changes = changes.ToList();
        }

        public BuPortfolioSecurityTargetChangesetInfo LatestChangeset { get; private set; }
        public String Username { get; private set; }
        public IEnumerable<IPstChange> Changes { get; private set; }
    }
}
