using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingTtbbv
{
    public class InsertChange : IChange
    {
        [DebuggerStepThrough]
        public InsertChange(Int32 basketId, Decimal baseValueAfter, String comment)
        {
            this.BasketId = basketId;
            this.BaseValueAfter = baseValueAfter;
            this.Comment = comment;
        }

        public Int32 BasketId { get; set; }
        public Decimal BaseValueAfter { get; set; }
        public String Comment { get; set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
