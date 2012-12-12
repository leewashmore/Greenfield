using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core.Overlaying
{
    /// <summary>
    /// Extracts ISO country codes for ALL overlay portfolios at once.
    /// </summary>
    public class CombinedCountryIsoCodesExtractor
    {
        private CountryIsoCodesExtractor extractor;

        public CombinedCountryIsoCodesExtractor(CountryIsoCodesExtractor extractor)
        {
            this.extractor = extractor;
        }

        public IEnumerable<String> ExtractCodes(
			IEnumerable<String> portfolioIdList,
			ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository,
			ISecurityIdToPortfolioIdResolver securityToPortfolioResolver,
			SecurityRepository securityRepository)
        {
			var allIsoCodes = new List<String>();
            foreach (var portfolioId in portfolioIdList)
            {
                var isoCodes = this.extractor.ExtractCodes(
					securityRepository,
					portfolioId,
					portfolioSecurityTargetRepository,
					securityToPortfolioResolver
				);
                allIsoCodes.AddRange(isoCodes);
            }
            var result = allIsoCodes.Distinct().ToArray();
            return result;
        }
    }
}
