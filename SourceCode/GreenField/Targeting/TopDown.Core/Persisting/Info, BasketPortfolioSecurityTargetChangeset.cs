using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class BasketPortfolioSecurityTargetChangesetInfo : ChangesetInfoBase
    {
        [DebuggerStepThrough]
        public BasketPortfolioSecurityTargetChangesetInfo()
        {
        }

        [DebuggerStepThrough]
        public BasketPortfolioSecurityTargetChangesetInfo(Int32 id, String username, DateTime timestamp, Int32 computationId)
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
