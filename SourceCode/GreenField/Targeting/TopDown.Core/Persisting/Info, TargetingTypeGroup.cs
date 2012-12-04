using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
	public class TargetingTypeGroupInfo
	{
		[DebuggerStepThrough]
		public TargetingTypeGroupInfo()
		{
		}

		[DebuggerStepThrough]
		public TargetingTypeGroupInfo(Int32 id, String name, String benchmarkIdOpt)
		{
			this.Id = id;
			this.Name = name;
            this.BenchmarkIdOpt = benchmarkIdOpt;
		}

		public Int32 Id { get; set; }
		public String Name { get; set; }
        public String BenchmarkIdOpt { get; set; }
    }
}
