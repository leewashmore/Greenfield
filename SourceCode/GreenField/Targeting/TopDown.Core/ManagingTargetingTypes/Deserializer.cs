using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingTaxonomies;

namespace TopDown.Core.ManagingTargetingTypes
{
	public class Deserializer
	{
		public TargetingTypeGroup DeserializeTargetingTypeGroup(
			TargetingTypeGroupInfo targetingTypeGroupInfo,
			IEnumerable<TargetingTypeInfo> whateverTargetingTypeInfos,
			IEnumerable<TargetingTypePortfolioCompositionInfo> whateverPortfolioCompositionInfos,
			TaxonomyRepository taxonomyRepository
		)
		{
			// get only targeting types that belong to the given group
			var ownedTargetingTypeInfos = whateverTargetingTypeInfos.Where(x => x.TargetingTypeGroupId == targetingTypeGroupInfo.Id);

			var targetingTypes = ownedTargetingTypeInfos
				.Select(x => this.DeserializeTargetingType(x, whateverPortfolioCompositionInfos, taxonomyRepository));

			var result = new TargetingTypeGroup(
				targetingTypeGroupInfo.Id,
				targetingTypeGroupInfo.Name,
				targetingTypeGroupInfo.BenchmarkIdOpt,
				targetingTypes
			);
			return result;
		}

		public TargetingType DeserializeTargetingType(
			TargetingTypeInfo targetingTypeInfo,
			IEnumerable<TargetingTypePortfolioCompositionInfo> whateverCompositionInfos,
			TaxonomyRepository taxonomyRepository
		)
		{
			var taxonomy = taxonomyRepository.GetTaxonomy(targetingTypeInfo.TaxonomyId);
			
			var portfolios = whateverCompositionInfos
				.Where(x => x.TargetingTypeId == targetingTypeInfo.Id)
				.Select(x => new Portfolio(x.PortfolioId));

			var result = new TargetingType(
				targetingTypeInfo.Id,
				targetingTypeInfo.Name,
				targetingTypeInfo.BenchmarkIdOpt,
				taxonomy,
				portfolios
			);

			return result;
		}
	}
}
