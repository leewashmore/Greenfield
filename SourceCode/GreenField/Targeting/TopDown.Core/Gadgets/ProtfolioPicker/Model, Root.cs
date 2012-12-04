using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Gadgets.PortfolioPicker
{
	public class RootModel
	{
		[DebuggerStepThrough]
		public RootModel(IEnumerable<TargetingTypeModel> targetingTypes)
		{
			this.TargetingTypes = targetingTypes.ToList();
		}

		public IEnumerable<TargetingTypeModel> TargetingTypes { get; private set; }
	}
}
