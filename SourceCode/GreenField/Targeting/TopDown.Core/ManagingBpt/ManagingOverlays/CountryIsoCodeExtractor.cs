using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPortfolios;

namespace TopDown.Core.Overlaying
{
    /// <summary>
    /// Extracts ISO country code for a single portfolio.
    /// </summary>
    public class CountryIsoCodesExtractor
    {
		public IEnumerable<String> ExtractCodes(
			SecurityRepository securityRepository,
			String portfolioId,
			ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository,
			ISecurityIdToPortfolioIdResolver securityToPortfolioResolver
		)
        {
            var traverser = new TargetsTraverser<Object>(
                (whatever, target) => target,   // we don't change targets
                (whaterer, target) => whaterer  // we don't change the state
            );
            var flatTargets = traverser.Traverse(
				portfolioId,
                portfolioSecurityTargetRepository,
				new Object(),
				new HashSet<String>(),
				securityToPortfolioResolver
			);
            
            var results = flatTargets.GroupBy(x => securityRepository
                .GetSecurity(x.SecurityId)
                .AsCompanySecurity() // <---- at this level (very bottom) all securities have to be company securities (there must be no funds at this level)
                .Country.IsoCode
            ).Select(x => x.Key).ToArray();

            return results;
        }
    }
}
