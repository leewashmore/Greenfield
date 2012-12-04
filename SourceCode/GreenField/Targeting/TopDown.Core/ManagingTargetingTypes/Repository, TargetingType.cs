using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingTargetingTypes
{
	public class TargetingTypeRepository : KeyedRepository<Int32, TargetingType>
	{
        [DebuggerStepThrough]
		public TargetingTypeRepository(IEnumerable<TargetingType> targetingTypes)
		{
			targetingTypes.ForEach(x => base.RegisterValue(x, x.Id));
		}

        [DebuggerStepThrough]
		public TargetingType GetTargetingType(Int32 id)
		{
            var found = base.FindValue(id);
            if (found == null) throw new ApplicationException("There is no targeting type with the \"" + id + "\" ID.");
            return found;
		}

        [DebuggerStepThrough]
        public IEnumerable<TargetingType> GetTargetingTypes()
        {
            return base.GetValues();
        }
    }
}
