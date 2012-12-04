using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingPsto
{
	public class DeleteChange : IChange
	{
		[DebuggerStepThrough]
		public DeleteChange(
            String securityId,
			Decimal targetOverlayBefore,
			String comment
		)
		{
			this.SecurityId = securityId;
			this.TargetOverlayBefore = targetOverlayBefore;
			this.Comment = comment;
		}

		public String SecurityId { get; private set; }
		public Decimal? TargetOverlayBefore { get; private set; }
		public String Comment { get; private set; }

		[DebuggerStepThrough]
		public void Accept(IChangeResolver resolver)
		{
			resolver.Resolve(this);
		}
	}
}