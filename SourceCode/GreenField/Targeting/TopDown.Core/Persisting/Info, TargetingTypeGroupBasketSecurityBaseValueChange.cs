using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class TargetingTypeGroupBasketSecurityBaseValueChangeInfo
    {
        [DebuggerStepThrough]
        public TargetingTypeGroupBasketSecurityBaseValueChangeInfo()
        {
        }


        [DebuggerStepThrough]
        public TargetingTypeGroupBasketSecurityBaseValueChangeInfo(
            Int32 id,
            Int32 targetingTypeGroupId,
            Int32 basketId,
            String securityId,
            Decimal? baseValueBefore,
            Decimal? baseValueAfter,
            Int32 changesetId,
            String comment
        )
        {
            this.Id = id;
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.BasketId = basketId;
            this.SecurityId = securityId;
            this.BaseValueBefore = baseValueBefore;
            this.BaseValueAfter = baseValueAfter;
            this.ChangesetId = changesetId;
            this.Comment = comment;
        }

        public Int32 Id { get; set; }
        public Int32 TargetingTypeGroupId { get; set; }
        public Int32 BasketId { get; set; }
        public String SecurityId { get; set; }
        public Decimal? BaseValueBefore { get; set; }
        public Decimal? BaseValueAfter { get; set; }
        public Int32 ChangesetId { get; set; }
        public String Comment { get; set; }

    }
}
