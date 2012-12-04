using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingTtbbv
{
    public class UpdateChange : IChange
    {
        [DebuggerStepThrough]
        public UpdateChange(Int32 basketId, Decimal baseValueBefore, Decimal baseValueAfter, String comment)
        {
            this.BasketId = basketId;
            this.BaseValueBefore = baseValueBefore;
            this.BaseValueAfter = baseValueAfter;
            this.Comment = comment;
        }

        public Int32 BasketId { get; set; }
        public Decimal BaseValueBefore { get; set; }
        public Decimal BaseValueAfter { get; set; }
        public String Comment { get; set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
