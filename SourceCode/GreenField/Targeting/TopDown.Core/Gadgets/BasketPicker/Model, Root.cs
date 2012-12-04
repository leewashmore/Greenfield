using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.BasketPicker
{
	public class RootModel : KeyedRepository<Int32, TargetingGroupModel>
	{
		[DebuggerStepThrough]
		public RootModel(IEnumerable<TargetingGroupModel> groups)
		{
			groups.ForEach(x => base.RegisterValue(x, x.TargetingGroupId));
		}

		[DebuggerStepThrough]
		public IEnumerable<TargetingGroupModel> GetGroups()
		{
			return base.GetValues();
		}
	}
}
