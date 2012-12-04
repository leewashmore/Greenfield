using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	/// <summary>
	/// Connects targeting-type/basket/base value on the breakdown screen.
	/// Represents a record of the TARGETING_TYPE_BASKET_BASE_VALUE table.
	/// </summary>
	public class TargetingTypeBasketBaseValueInfo
	{
        [DebuggerStepThrough]
        public TargetingTypeBasketBaseValueInfo()
        {
        }
        
        [DebuggerStepThrough]
        public TargetingTypeBasketBaseValueInfo(Int32 targetingTypeId, Int32 basketId, Decimal baseValue, Int32 changeId)
        {
            this.TargetingTypeId = targetingTypeId;
            this.BasketId = basketId;
            this.BaseValue = baseValue;
            this.ChangeId = changeId;
        }

		public Int32 TargetingTypeId { get; set; }
		public Int32 BasketId { get; set; }
		public Decimal BaseValue { get; set; }
        public Int32 ChangeId { get; set; }
	}
}
