using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations for Asset-Allocation Gadget
    /// </summary>
    public static class AssetAllocationCalculations
    {
        /// <summary>
        /// Static Method calculating asset allocations
        /// </summary>
        /// <param name="dimensionPortfolioHoldingsData">Collection GF_PORTFOLIO_HOLDINGS retrieved from Dimension</param>
        /// <param name="portfolioSelectionData">Data of Currently selected Portfolio</param>
        /// <returns>Collection of Asset Allocation Data</returns>
        public static List<AssetAllocationData> CalculateAssetAllocationValues(List<DimensionEntitiesService.GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData, PortfolioSelectionData portfolioSelectionData)
        {
            try
            {
                decimal? modelWeight = 0;
                decimal? benchmarkWeight = 0;
                decimal? activePosition = 0;
                decimal? portfolioWeight = 0;
                decimal? sumDirtyValuePC = 0;
                decimal? sumModelWeight = 0;

                List<AssetAllocationData> result = new List<AssetAllocationData>();

                if ((dimensionPortfolioHoldingsData == null) || (portfolioSelectionData == null))
                    throw new ArgumentNullException();

                List<string> countryNames = dimensionPortfolioHoldingsData.Select(a => a.COUNTRYNAME).Distinct().ToList();

                foreach (string item in countryNames)
                {
                    sumDirtyValuePC = dimensionPortfolioHoldingsData.Sum(a => a.DIRTY_VALUE_PC);
                    sumModelWeight = dimensionPortfolioHoldingsData.Sum(a => a.ASH_EMM_MODEL_WEIGHT);

                    //if sum of DirtyValuePC or ModelWeight is zero then return emtpy set
                    if ((sumDirtyValuePC == 0) || (sumModelWeight == 0))
                        return result;

                    modelWeight = dimensionPortfolioHoldingsData.Where(a => a.COUNTRYNAME == item).Sum(a => a.ASH_EMM_MODEL_WEIGHT) / sumModelWeight;
                    benchmarkWeight = dimensionPortfolioHoldingsData.Where(a => a.COUNTRYNAME == item).Sum(a => a.BENCHMARK_WEIGHT);
                    portfolioWeight = dimensionPortfolioHoldingsData.Where(a => a.COUNTRYNAME == item).Sum(a => a.DIRTY_VALUE_PC) / sumDirtyValuePC;
                    activePosition = portfolioWeight - benchmarkWeight;

                    AssetAllocationData data = new AssetAllocationData();
                    data.BenchmarkWeight = benchmarkWeight;
                    data.Country = item;
                    data.ModelWeight = modelWeight;
                    data.PortfolioWeight = portfolioWeight;
                    data.PortfolioId = portfolioSelectionData.PortfolioId;
                    data.ActivePosition = activePosition;
                    result.Add(data);
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
    }
}