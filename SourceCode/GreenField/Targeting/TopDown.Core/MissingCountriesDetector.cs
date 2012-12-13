using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPortfolios;
using Aims.Core;

namespace TopDown.Core
{
    public class MissingCountriesDetector
    {
        private UnknownCountryIsoCodesDetector unknownIsoCodesDetector;
        
        private ManagingTaxonomies.CountryIsoCodesExtractor taxonomyIsoCodeExtractor;
        private Overlaying.CombinedCountryIsoCodesExtractor overlayIsoCodeExtractor;
        private ManagingBenchmarks.CountryIsoCodesExtractor benchmarkingIsoCodeExtractor;

        public MissingCountriesDetector(
            UnknownCountryIsoCodesDetector unknownIsoCodesDetector,
			ManagingTaxonomies.CountryIsoCodesExtractor taxonomyIsoCodeExtractor,
            Overlaying.CombinedCountryIsoCodesExtractor overlayIsoCodeExtractor,
			ManagingBenchmarks.CountryIsoCodesExtractor benchmarkingIsoCodeExtractor            
        )
        {
            this.unknownIsoCodesDetector = unknownIsoCodesDetector;
            this.taxonomyIsoCodeExtractor = taxonomyIsoCodeExtractor;
            this.benchmarkingIsoCodeExtractor = benchmarkingIsoCodeExtractor;
            this.overlayIsoCodeExtractor = overlayIsoCodeExtractor;
        }

		public IEnumerable<String> FindMissingCountries(
            Taxonomy taxonomy,
            IEnumerable<BenchmarkSumByIsoInfo> benchmarks,
            IEnumerable<String> overlaySecurityIds,
            ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository,
            ISecurityIdToPortfolioIdResolver securityToPortfolioResolver,
            SecurityRepository securityRepository
        )
        {
            var knownIsoCodes = this.taxonomyIsoCodeExtractor.ExtractCodes(taxonomy);
            var whateverIsoCodesFromBenchmarks = this.benchmarkingIsoCodeExtractor.ExtractCodes(benchmarks);
            var whateverIsoCodesFromOverlay = this.overlayIsoCodeExtractor.ExtractCodes(
                overlaySecurityIds,
                portfolioSecurityTargetRepository,
                securityToPortfolioResolver,
                securityRepository
            );
            var whateverIsoCodes = whateverIsoCodesFromBenchmarks.Union(whateverIsoCodesFromOverlay).ToArray();

            var unknownIsoCodes = this.unknownIsoCodesDetector.FindUnknownCodes(knownIsoCodes, whateverIsoCodes);
            return unknownIsoCodes;
        }
    }
}
