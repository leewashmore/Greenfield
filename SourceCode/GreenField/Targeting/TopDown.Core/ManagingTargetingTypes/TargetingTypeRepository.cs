using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTargetingTypes
{
	public class TargetingTypeRepository : KeyedRepository<Int32, TargetingType>
	{
		public TargetingTypeRepository(IEnumerable<TargetingType> targetingTypes)
		{
			targetingTypes.ForEach(x => this.RegisterValue(x, x.Id));
		}

		public TargetingType GetTrageting(Int32 targetingId)
		{
			var found = base.FindValue(targetingId);
			if (found == null) throw new ApplicationException("There is no targeting type with the \"" + targetingId + "\" ID.");
			return found;
		}
	}
}
