using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.BasketSelector
{
	public class TargetingTypeGroupModel
	{
		[DebuggerStepThrough]
		public TargetingTypeGroupModel(
			Int32 targetingGroupId,
			String targetingGroupName,
			IEnumerable<BasketModel> baskets)
		{
			this.Id = targetingGroupId;
			this.Name = targetingGroupName;
			this.Baskets = new List<BasketModel>(baskets);
		}
		
		public Int32 Id { get; private set; }
		public String Name { get; private set; }
		public IEnumerable<BasketModel> Baskets { get; private set; }		
	}
}
