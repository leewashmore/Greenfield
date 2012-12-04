using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt.ChangingTtbpt
{
    public class InsertChange : IChange
    {
        [DebuggerStepThrough]
        public InsertChange(Int32 basketId, Decimal targetAfter, String comment)
        {
            this.BasketId = basketId;
            this.TargetAfter = targetAfter;
            this.Comment = comment;
        }

        public Int32 BasketId { get; private set; }
        public Decimal TargetAfter { get; private set; }
        public String Comment { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IChangeResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
