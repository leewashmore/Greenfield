using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using Aims.Core;

namespace TopDown.Core.ManagingTargetingTypes
{
	public class TargetingTypeGroupRepository : KeyedRepository<Int32, TargetingTypeGroup>
	{

		[DebuggerStepThrough]
		public TargetingTypeGroupRepository(IEnumerable<TargetingTypeGroupInfo> targetingTypeGroupInfos, TargetingTypeRepository targetingTypeRepository)
		{
			var targetingTypeGroups = targetingTypeGroupInfos.Select(x =>
                new TargetingTypeGroup(
                    x.Id,
                    x.Name,
                    x.BenchmarkIdOpt,
                    targetingTypeRepository.GetTargetingTypes().Where(y => x.Id == y.TargetingTypeGroupId)
                )
            );
			targetingTypeGroups.ForEach(x => base.RegisterValue(x, x.Id));
		}

		[DebuggerStepThrough]
		public TargetingTypeGroup GetTargetingTypeGroup(Int32 targetingTypeGroupId)
		{
			var found = base.FindValue(targetingTypeGroupId);
			if (found == null) throw new ApplicationException("There is no targeting type group with the \"" + targetingTypeGroupId + "\" ID.");
			return found;
		}

		[DebuggerStepThrough]
		public IEnumerable<TargetingTypeGroup> GetTargetingTypeGroups()
		{
			return base.GetValues();
		}
	}
}
