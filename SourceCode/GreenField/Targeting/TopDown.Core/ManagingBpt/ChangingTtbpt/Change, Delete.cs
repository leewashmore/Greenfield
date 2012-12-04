using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingTtbpt
{
    public class DeleteChange : IChange
    {
        [DebuggerStepThrough]
        public DeleteChange(Int32 basketId, Decimal targetBefore, String comment)
        {
            this.BasketId = basketId;
            this.TargetBefore = targetBefore;
            this.Comment = comment;
        }

        public Int32 BasketId { get; private set; }
        public Decimal TargetBefore { get; private set; }
        public String Comment { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
