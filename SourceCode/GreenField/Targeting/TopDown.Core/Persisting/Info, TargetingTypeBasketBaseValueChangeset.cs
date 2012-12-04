using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the TARGETING_TYPE_BASKET_BASE_VALUE_CHANGESET table.
    /// </summary>
    public class TargetingTypeBasketBaseValueChangesetInfo : ChangesetInfoBase
    {
        [DebuggerStepThrough]
        public TargetingTypeBasketBaseValueChangesetInfo()
        {
        }

        [DebuggerStepThrough]
        public TargetingTypeBasketBaseValueChangesetInfo(Int32 id, String username, DateTime timestamp, Int32 computationId)
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
