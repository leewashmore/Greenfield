using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.ManagingTargetingTypes
{
    public class InfoDeserializer
    {
        public TargetingType DeserializeToTargetingType(
            TargetingTypeInfo targetingTypeInfo,
            TaxonomyRepository taxonomyRepository,
            IEnumerable<TargetingTypePortfolioInfo> whateverPortfolioCompositionInfos,
			PortfolioRepository portfolioRepository
        )
        {
            var taxonomy = taxonomyRepository.GetTaxonomy(targetingTypeInfo.TaxonomyId);

			var owned = whateverPortfolioCompositionInfos
				.Where(x => x.TargetingTypeId == targetingTypeInfo.Id);
			
			var broadGlobalActivePortfolios = owned
				.Select(x => portfolioRepository.FindBroadGlobalActivePortfolio(x.PortfolioId))
				.Where(x => x != null);

			var bottomUpPortfolios = this.GetBottomUpPortfolios(owned, portfolioRepository);

            var result = new TargetingType(
                targetingTypeInfo.Id,
                targetingTypeInfo.Name,
                targetingTypeInfo.TargetingTypeGroupId,
                targetingTypeInfo.BenchmarkIdOpt,
                taxonomy,
                broadGlobalActivePortfolios,
				bottomUpPortfolios
            );
            
            return result;
        }

		protected IEnumerable<BottomUpPortfolio> GetBottomUpPortfolios(
			IEnumerable<TargetingTypePortfolioInfo> portfolioInfos,
			PortfolioRepository portfolioRepository
		)
		{
			var result = new List<BottomUpPortfolio>();
			foreach (var portfolioInfo in portfolioInfos)
			{
				var bottomUpPorfolioOpt = portfolioRepository.FindBottomUpPortfolio(portfolioInfo.PortfolioId);
				if (bottomUpPorfolioOpt != null)
				{
					result.Add(bottomUpPorfolioOpt);
				}
			}
			return result;
		}
    }
}
