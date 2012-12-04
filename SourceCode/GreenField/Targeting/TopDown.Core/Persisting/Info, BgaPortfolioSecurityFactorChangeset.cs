using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the BGA_PORTFOLIO_SECURITY_FACTOR_CHANGESET table.
    /// </summary>
    public class BgaPortfolioSecurityFactorChangesetInfo : ChangesetInfoBase
    {
        [DebuggerStepThrough]
        public BgaPortfolioSecurityFactorChangesetInfo()
        {
        }

        [DebuggerStepThrough]
        public BgaPortfolioSecurityFactorChangesetInfo(Int32 id, String username, DateTime timestamp, Int32 computationId)
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
