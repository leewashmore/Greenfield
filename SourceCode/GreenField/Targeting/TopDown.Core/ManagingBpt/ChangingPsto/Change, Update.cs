using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingPsto
{
	public class UpdateChange : IChange
	{
		[DebuggerStepThrough]
		public UpdateChange(
            String securityId,
			Decimal targetBefore,
			Decimal targetAfter,
			String comment
		)
		{
			this.SecurityId = securityId;
			this.TargetBefore = targetBefore;
			this.TargetAfter = targetAfter;
			this.Comment = comment;
		}

        public String SecurityId { get; private set; }
		public Decimal TargetBefore { get; private set; }
		public Decimal TargetAfter { get; private set; }
		public String Comment { get; private set; }

		[DebuggerStepThrough]
		public void Accept(IChangeResolver resolver)
		{
			resolver.Resolve(this);
		}



		
	}
}