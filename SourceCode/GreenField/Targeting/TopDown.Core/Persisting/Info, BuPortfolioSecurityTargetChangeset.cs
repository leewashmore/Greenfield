using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the BU_PORTFOLIO_SECURITY_TARGET_CHANGESET table.
    /// Every changeset is composed of separate change items.
    /// See also PortfolioSecurityTargetChangeInfo.
    /// </summary>
    public class BuPortfolioSecurityTargetChangesetInfo : ChangesetInfoBase
    {
        [DebuggerStepThrough]
        public BuPortfolioSecurityTargetChangesetInfo()
        {
        }

        [DebuggerStepThrough]
        public BuPortfolioSecurityTargetChangesetInfo(Int32 id, String username, DateTime timestamp, Int32 computationId)
            : base(id, username, timestamp, computationId)
        {
        }
		[DebuggerStepThrough]
		public override void Accept(IChangesetInfoResolver resolver)
		{
			resolver.Resolve(this);
		}

    }
}
