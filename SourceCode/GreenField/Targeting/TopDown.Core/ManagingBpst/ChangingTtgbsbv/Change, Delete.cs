using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpst.ChangingTtgbsbv
{
    public class DeleteChange : IChange
    {
        [DebuggerStepThrough]
        public DeleteChange(String securityId, Decimal? baseValueBefore, String comment)
        {
            this.SecurityId = securityId;
            this.BaseValueBefore = baseValueBefore;
            this.Comment = comment;
        }

		public String SecurityId { get; private  set; }
		public Decimal? BaseValueBefore { get; private set; }
        public String Comment { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
