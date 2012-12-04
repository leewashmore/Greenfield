using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingTtbbv
{
    public class DeleteChange : IChange
    {
        [DebuggerStepThrough]
        public DeleteChange(Int32 basketId, Decimal baseValueBefore, String comment)
        {
            this.BasketId = basketId;
            this.BaseValueBefore = baseValueBefore;
            this.Comment = comment;
        }

        public Int32 BasketId { get; set; }
        public Decimal BaseValueBefore { get; set; }
        public String Comment { get; set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
