using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	public class TargetingTypeInfo
	{
		[DebuggerStepThrough]
		public TargetingTypeInfo()
		{
		}

		[DebuggerStepThrough]
		public TargetingTypeInfo(Int32 id, String name, Int32 targetingTypeGroupId, String benchmarkIdOpt, Int32 taxonomyId)
		{
			this.Id = id;
			this.Name = name;
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.BenchmarkIdOpt = benchmarkIdOpt;
            this.TaxonomyId = taxonomyId;
		}

		public Int32 Id { get; set; }
		public String Name { get; set; }
        public Int32 TargetingTypeGroupId { get; set; }
        public String BenchmarkIdOpt { get; set; }
        public Int32 TaxonomyId { get; set; }

        
    }
}
