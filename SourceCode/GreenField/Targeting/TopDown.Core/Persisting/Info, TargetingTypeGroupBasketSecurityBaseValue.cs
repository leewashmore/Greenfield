using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    /// <summary>
    /// Represents a record of the TARGETING_TYPE_GROUP_BASKET_SECURITY_BASE_VALUE table.
    /// </summary>
    public class TargetingTypeGroupBasketSecurityBaseValueInfo
    {
        [DebuggerStepThrough]
        public TargetingTypeGroupBasketSecurityBaseValueInfo()
        {
        }


        [DebuggerStepThrough]
        public TargetingTypeGroupBasketSecurityBaseValueInfo(Int32 targetingTypeGroupId, Int32 basketId, String securityId, Decimal baseValue, Int32 changeId)
        {
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.BasketId = basketId;
            this.SecurityId = securityId;
            this.BaseValue = baseValue;
            this.ChangeId = changeId;
        }

        public Int32 TargetingTypeGroupId { get; set; }
        public Int32 BasketId { get; set; }
        public String SecurityId { get; set; }
        public Decimal BaseValue { get; set; }
        public Int32 ChangeId { get; set; }
    }
}
