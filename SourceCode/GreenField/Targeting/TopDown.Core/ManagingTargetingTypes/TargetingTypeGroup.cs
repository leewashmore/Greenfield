using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.ManagingTargetingTypes
{
	public class TargetingTypeGroup : KeyedRepository<Int32, TargetingType>
	{
		[DebuggerStepThrough]
		public TargetingTypeGroup(Int32 id, String name, String benchmarkIdOpt, IEnumerable<TargetingType> targetingTypes)
		{
			this.Id = id;
			this.Name = name;
            this.BenchmarkIdOpt = benchmarkIdOpt;
            targetingTypes.ForEach(x => this.RegisterValue(x, x.Id));
		}

		public Int32 Id { get; private set; }
		public String Name { get; private set; }
        public String BenchmarkIdOpt { get; private set; }

		[DebuggerStepThrough]
		public IEnumerable<TargetingType> GetTargetingTypes()
		{
			return base.GetValues();
		}

		public IEnumerable<BroadGlobalActivePortfolio> GetBgaPortfolios()
		{
			return this.GetTargetingTypes()
				.SelectMany(x => x.BroadGlobalActivePortfolios)
				.GroupBy(x => x.Id).Select(x => x.Single()) // doing distinct
				.OrderBy(x => x.Name); 
		}
	}
}
