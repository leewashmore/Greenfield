using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.BasketPicker
{
	public class TargetingGroupModel : Repository<BasketModel>
	{
		[DebuggerStepThrough]
		public TargetingGroupModel(Int32 targetingGroupId, String targetingGroupName, IEnumerable<BasketModel> baskets)
		{
			this.TargetingGroupId = targetingGroupId;
			this.TargetingGroupName = targetingGroupName;
			baskets.ForEach(x => this.RegisterValue(x));
		}
		
		public Int32 TargetingGroupId { get; private set; }
		public String TargetingGroupName { get; private set; }

		[DebuggerStepThrough]
		public IEnumerable<BasketModel> GetBaskets()
		{
			return base.GetValues();
		}

		
	}
}
