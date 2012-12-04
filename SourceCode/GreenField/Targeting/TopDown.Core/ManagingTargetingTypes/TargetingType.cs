using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using System.Diagnostics;
using TopDown.Core.ManagingPortfolios;

namespace TopDown.Core.ManagingTargetingTypes
{
    public class TargetingType
    {
        [DebuggerStepThrough]
        public TargetingType(
			Int32 id,
			String name,
			Int32 targetingTypeGroupId,
			String benchmarkIdOpt,
			Taxonomy taxonomy,
			IEnumerable<BroadGlobalActivePortfolio> broadGlobalActivePortfolios,
			IEnumerable<BottomUpPortfolio> bottomUpPortfolios
		)
        {
            this.Id = id;
            this.Name = name;
            this.TargetingTypeGroupId = targetingTypeGroupId;
            this.Taxonomy = taxonomy;
            this.BenchmarkIdOpt = benchmarkIdOpt;
            this.BroadGlobalActivePortfolios = broadGlobalActivePortfolios.ToList();
			this.BottomUpPortfolios = bottomUpPortfolios.ToList();
        }

        public Int32 Id { get; private set; }
        public String Name { get; private set; }
        public Int32 TargetingTypeGroupId { get; private set; }
        public String BenchmarkIdOpt { get; private set; }
        public Taxonomy Taxonomy { get; private set; }
        public IEnumerable<BroadGlobalActivePortfolio> BroadGlobalActivePortfolios { get; private set; }
		public IEnumerable<BottomUpPortfolio> BottomUpPortfolios { get; private set; }
    }
}
