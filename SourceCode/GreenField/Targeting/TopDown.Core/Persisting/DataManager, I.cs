using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Persisting;

namespace TopDown.Core.Persisting
{
    public interface IDataManager : Aims.Core.Persisting.IDataManager
    {
        IEnumerable<BasketInfo> GetAllBaskets();
		IEnumerable<UsernameBottomUpPortfolioInfo> GetUsernameBottomUpPortfolios(String username);

        DateTime GetLastestDateWhichBenchmarkDataIsAvialableOn();
        IEnumerable<BenchmarkInfo> GetBenchmarks(DateTime date);
		IEnumerable<CountryBasketInfo> GetAllCountryBaskets();
		IEnumerable<RegionBasketInfo> GetAllRegionBaskets();

        // P-S-T
        IEnumerable<BuPortfolioSecurityTargetInfo> GetAllPortfolioSecurityTargets();
		IEnumerable<BuPortfolioSecurityTargetInfo> GetPortfolioSecurityTargets(String bottomUpPortfolioId);
        Int32 InsertPortfolioSecurityTarget(BuPortfolioSecurityTargetInfo pstInfo);
        Int32 UpdatePortfolioSecurityTarget(BuPortfolioSecurityTargetInfo pstInfo);
        Int32 DeletePortfolioSecurityTarget(String portfolioId, String securityId);
        BuPortfolioSecurityTargetChangesetInfo GetLatestPortfolioSecurityTargetChangeSet();
        Int32 InsertPortfolioSecurityTargetChangeset(BuPortfolioSecurityTargetChangesetInfo changesetInfo);
        Int32 InsertPortfolioSecurityTargetChange(BuPortfolioSecurityTargetChangeInfo changeInfo);
        IEnumerator<Int32> ReservePortfolioSecurityTargetChangesetIds(Int32 howMany);
        IEnumerator<Int32> ReservePortfolioSecurityTargetChangeIds(Int32 howMany);
        Int32 CreateBasketCountry(Int32 id, String isoCode);
        Int32 CreateBasketAsCountry(Int32 id);
        Int32 UpdateTaxonomy(String taxonomy, Int32 id);

        // B, T, TT, TTG
		IEnumerator<Int32> ReserveBasketIds(Int32 howMany);
        IEnumerable<TargetingTypeGroupInfo> GetAllTargetingTypeGroups();
        IEnumerable<TargetingTypeInfo> GetAllTargetingTypes();
        IEnumerable<TargetingTypePortfolioInfo> GetAllTargetingTypePortfolio();
        IEnumerable<TaxonomyInfo> GetAllTaxonomies();

        // TT-B-Bv
        IEnumerable<TargetingTypeBasketBaseValueInfo> GetTargetingTypeBasketBaseValues(Int32 targetingTypeId);
        TargetingTypeBasketBaseValueChangesetInfo GetLatestTargetingTypeBasketBaseValueChangeset();
        IEnumerator<Int32> ReserveTargetingTypeBasketBaseValueChangesetIds(Int32 howMany);
        Int32 InsertTargetingTypeBasketBaseValueChangeset(TargetingTypeBasketBaseValueChangesetInfo changesetInfo);
        IEnumerator<Int32> ReserveTargetingTypeBasketBaseValueChangeIds(Int32 howMany);
        Int32 InsertTargetingTypeBasketBaseValueChange(TargetingTypeBasketBaseValueChangeInfo changeInfo);
        Int32 InsertTargetingTypeBasketBaseValue(TargetingTypeBasketBaseValueInfo info);
        Int32 UpdateTargetingTypeBasketBaseValue(TargetingTypeBasketBaseValueInfo info);
        Int32 DeleteTargetingTypeBasketBaseValue(Int32 targetingTypeId, Int32 basketId);
        

		// TT-B-P-T
        TargetingTypeBasketPortfolioTargetChangesetInfo GetLatestTargetingTypeBasketPortfolioTargetChangeset();
        IEnumerator<Int32> ReserveTargetingTypeBasketPortfolioTargetChangesetIds(Int32 howMany);
        Int32 InsertTargetingTypeBasketPortfolioTargetChangeset(TargetingTypeBasketPortfolioTargetChangesetInfo changesetInfo);
        IEnumerator<Int32> ReserveTargetingTypeBasketPortfolioTargetChangeIds(Int32 howMany);
		Int32 InsertTargetingTypeBasketPortfolioTargetChange(TargetingTypeBasketPortfolioTargetChangeInfo changeInfo);
		Int32 UpdateTargetingTypeBasketPortfolioTarget(TargetingTypeBasketPortfolioTargetInfo info);
		Int32 InsertTargetingTypeBasketPortfolioTarget(TargetingTypeBasketPortfolioTargetInfo info);
        Int32 DeleteTargetingTypeBasketPortfolioTarget(Int32 targetingTypeId, Int32 basketId, String portfolioId);
		IEnumerable<TargetingTypeBasketPortfolioTargetInfo> GetTargetingTypeBasketPortfolioTarget(Int32 targetingType, String portfolioId);

        // BgaP-S-F
        IEnumerable<BgaPortfolioSecurityFactorInfo> GetBgaPortfolioSecurityFactors(String broadGlobalActivePorfolioId);
        BgaPortfolioSecurityFactorChangesetInfo GetLatestBgaPortfolioSecurityFactorChangeset();
        IEnumerator<Int32> ReserveBgaPortfolioSecurityFactorChangesetIds(Int32 howMany);
        Int32 InsertBgaPortfolioSecurityFactorChangeset(BgaPortfolioSecurityFactorChangesetInfo changesetInfo);
        IEnumerator<Int32> ReserveBgaPortfolioSecurityFactorChangeIds(Int32 howMany);
        Int32 InsertBgaPortfolioSecurityFactorChange(BgaPortfolioSecurityFactorChangeInfo changeInfo);
        Int32 InsertBgaPortfolioSecurityFactor(BgaPortfolioSecurityFactorInfo info);
        Int32 UpdateBgaPortfolioSecurityFactor(BgaPortfolioSecurityFactorInfo info);
        Int32 DeleteBgaPortfolioSecurityFactor(String broadGlobalActivePorfolioId, String securityId);
        


        // TTG-B-S-Bv

        IEnumerable<TargetingTypeGroupBasketSecurityBaseValueChangeInfo> GetTargetingTypeGroupBasketSecurityBaseValueChanges(int targetingTypeGroupId, int basketId, string securityId);
        IEnumerable<TargetingTypeGroupBasketSecurityBaseValueChangesetInfo> GetTargetingTypeGroupBasketSecurityBaseValueChangesets(int[] changesetIds);
        TargetingTypeGroupBasketSecurityBaseValueChangesetInfo GetLatestTargetingTypeGroupBasketSecurityBaseValueChangeset();
        IEnumerator<Int32> ReserveTargetingTypeGroupBasketSecurityBaseValueChangesetIds(Int32 howMany);
        IEnumerator<Int32> ReserveTargetingTypeGroupBasketSecurityBaseValueChangeIds(Int32 howMany);
        Int32 InsertTargetingTypeGroupBasketSecurityBaseValueChangeset(TargetingTypeGroupBasketSecurityBaseValueChangesetInfo changesetInfo);
        Int32 InsertTargetingTypeGroupBasketSecurityBaseValueChange(TargetingTypeGroupBasketSecurityBaseValueChangeInfo changeInfo);
        Int32 InsertTargetingTypeGroupBasketSecurityBaseValue(TargetingTypeGroupBasketSecurityBaseValueInfo info);
        Int32 UpdateTargetingTypeGroupBasketSecurityBaseValue(TargetingTypeGroupBasketSecurityBaseValueInfo info);
        Int32 DeleteTargetingTypeGroupBasketSecurityBaseValue(Int32 targetingTypeGroupId, Int32 basketId, String securityId);

        IEnumerable<TargetingTypeGroupBasketSecurityBaseValueInfo> GetTargetingTypeGroupBasketSecurityBaseValues(Int32 targetingTypeGroupId, Int32 basketId);
        IEnumerable<TargetingTypeGroupBasketSecurityBaseValueInfo> GetTargetingTypeGroupBasketSecurityBaseValues();
        
        // B-P-S-T
		BasketPortfolioSecurityTargetChangesetInfo GetLatestBasketPortfolioSecurityTargetChangeset();
        IEnumerator<Int32> ReserveBasketPortfolioSecurityTargetChangesetIds(Int32 howMany);
        Int32 InsertBasketPortfolioSecurityTargetChangeset(BasketPortfolioSecurityTargetChangesetInfo changesetInfo);
        IEnumerator<Int32> ReserveBasketPortfolioSecurityTargetChangeIds(Int32 howMany);
        Int32 InsertBasketPortfolioSecurityTargetChange(BasketPortfolioSecurityTargetChangeInfo changeInfo);
        IEnumerable<BasketPortfolioSecurityTargetChangeInfo> GetBasketPortfolioSecurityTargetChanges(Int32 basketId, String broadGlbalActivePortfolioId, String securityId);
        IEnumerable<BasketPortfolioSecurityTargetChangesetInfo> GetBasketPortfolioSecurityTargetChangesets(IEnumerable<Int32> changesetIds);
        Int32 InsertBasketPortfolioSecurityTarget(BasketPortfolioSecurityTargetInfo info);
        Int32 UpdateBasketPortfolioSecurityTarget(BasketPortfolioSecurityTargetInfo info);
        Int32 DeleteBasketPortfolioSecurityTarget(Int32 basketId, String portfolioId, String securityId);
		IEnumerable<BasketPortfolioSecurityTargetInfo> GetBasketProtfolioSecurityTargets(Int32 basketId, IEnumerable<String> broadGlobalActivePortfolioIds);
        IEnumerable<BasketPortfolioSecurityTargetInfo> GetBasketProtfolioSecurityTargets();




        IEnumerator<Int32> ReserveTargetingComputationIds(Int32 howMany);
        Int32 InsertTargetingComputation(TargetingCalculationInfo targetingComputationInfo);

        IEnumerable<TargetingCalculationInfo> Get2MostRecentTargetingCalculations();
        
        /// <summary>
        /// Gets a targeting calculation with pre-populated STARTED_ON field with the current time according to the database server.
        /// </summary>
        TargetingCalculationInfo GetTargetingCalculation(Int32 calculationId);

        /// <summary>
        /// Saves the targeting calculation popularing the FINISHED_ON date with the current time according to the database server.
        /// </summary>
        Int32 FinishTargetingCalculationUnsafe(Int32 calculationId, Int32 status, String log);
		Int32 DeleteAllBgaPortfolioSecurityTargets();
        Int32 DeleteBgaPortfolioSecurityTargets(IEnumerable<BgaPortfolioSecurityTargetInfo> result);
		void InsertBgaPortfolioSecurityTargets(IEnumerable<BgaPortfolioSecurityTargetInfo> result);
		IEnumerable<CalculationWithChangesets> GetAllCalculationWithChangesets(Int32 howMany);

        Int32 StartTargetingCalculation(Int32 calculationId);

        
    }
}