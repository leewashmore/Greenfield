﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class TargetingTypeBasketBaseValueChangeInfo : IChangeInfoBase
    {
        [DebuggerStepThrough]
        public TargetingTypeBasketBaseValueChangeInfo()
        {
        }
    
        [DebuggerStepThrough]
        public TargetingTypeBasketBaseValueChangeInfo(Int32 id, Int32 targetingTypeGroupId, Int32 basketId, Decimal? baseValueBefore, Decimal? baseValueAfter, Int32 changesetId, String comment)
        {
            this.Id = id;
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.BasketId = basketId;
            this.BaseValueBefore = baseValueBefore;
            this.BaseValueAfter = baseValueAfter;
            this.ChangesetId = changesetId;
            this.Comment = comment;
        }

        public Int32 Id { get; set; }
        public Int32 TargetingTypeGroupId { get; set; }
        public Int32 BasketId { get; set; }
        public Decimal? BaseValueBefore { get; set; }
        public Decimal? BaseValueAfter { get; set; }
        public Int32 ChangesetId { get; set; }
        public String Comment { get; set; }


        String IChangeInfoBase.Comment
        {
            get { return this.Comment; }
        }

        Decimal? IChangeInfoBase.Before
        {
            get { return this.BaseValueBefore; }
        }

        Decimal? IChangeInfoBase.After
        {
            get { return this.BaseValueAfter; }
        }
    }
}
