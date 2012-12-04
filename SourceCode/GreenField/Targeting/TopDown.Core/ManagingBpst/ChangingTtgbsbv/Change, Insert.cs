using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpst.ChangingTtgbsbv
{
    public class InsertChange : IChange
    {
        [DebuggerStepThrough]
        public InsertChange(String securityId, Decimal baseValueAfter, String comment)
        {
            this.SecurityId = securityId;
            this.BaseValueAfter = baseValueAfter;
            this.Comment = comment;
        }

        public String SecurityId { get; private set; }
        public Decimal BaseValueAfter { get; private set; }
        public String Comment { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
