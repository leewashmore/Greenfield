using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingPsto
{
	public class InsertChange : IChange
	{
		[DebuggerStepThrough]
		public InsertChange(
            String securityId,
			Decimal targetOverlayAfter,
			String comment
		)
		{
			this.SecurityId = securityId;
			this.TargetOverlayAfter = targetOverlayAfter;
			this.Comment = comment;
		}

        public String SecurityId { get; private set; }
		public Decimal TargetOverlayAfter { get; private set; }
		public String Comment { get; private set; }
		
		
		[DebuggerStepThrough]
		public void Accept(IChangeResolver resolver)
		{
			resolver.Resolve(this);
		}
	}
}