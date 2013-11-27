using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;

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
        public static List<AssetAllocationData> CalculateAssetAllocationValues(List<GreenField.DAL.GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData, List<GreenField.DAL.GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData, PortfolioSelectionData portfolioSelectionData)
        {
            try
            {
                decimal? modelWeight = 0;
                decimal? benchmarkWeight = 0;
                decimal? activePosition = 0;
                decimal? modelWeightCash = 0;
                decimal? portfolioWeightCash = 0;
                decimal? benchmarkWeightCash = 0;
                decimal? portfolioWeight = 0;
                decimal? sumDirtyValuePC = 0;
                decimal? sumModelWeight = 0;
                decimal? sumBenchmarkWeight = 0;
                List<AssetAllocationData> result = new List<AssetAllocationData>();

                if ((dimensionPortfolioHoldingsData == null) || (portfolioSelectionData == null))
                {
                    throw new ArgumentNullException();
                }
                List<string> countryNames = dimensionPortfolioHoldingsData.Select(a => a.COUNTRYNAME).Distinct().ToList();
                if (countryNames.Count == 0)
                {
                    throw new InvalidOperationException();
                }
                foreach (string item in countryNames)
                {
                    sumDirtyValuePC = dimensionPortfolioHoldingsData.Sum(a => Convert.ToDecimal(a.DIRTY_VALUE_PC));
                    sumBenchmarkWeight = dimensionBenchmarkHoldingsData.Sum(a => Convert.ToDecimal(a.BENCHMARK_WEIGHT));

                    //if sum of DirtyValuePC or ModelWeight is zero then return empty set
                    if ((sumDirtyValuePC == 0) || (sumBenchmarkWeight == 0) || (sumModelWeight == 0))
                    {
                        return result;
                    }
                    modelWeight = dimensionPortfolioHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).
                        Sum(a => Convert.ToDecimal(a.ASH_EMM_MODEL_WEIGHT)) / sumModelWeight;
                    portfolioWeight = dimensionPortfolioHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).
                        Sum(a => Convert.ToDecimal(a.DIRTY_VALUE_PC)) / sumDirtyValuePC;
                    modelWeightCash = modelWeightCash + dimensionPortfolioHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH")).
                        Sum(a => Convert.ToDecimal(a.ASH_EMM_MODEL_WEIGHT)) / sumModelWeight;
                    portfolioWeightCash = portfolioWeightCash + dimensionPortfolioHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH")).
                        Sum(a => Convert.ToDecimal(a.DIRTY_VALUE_PC)) / sumDirtyValuePC;
                    benchmarkWeight = dimensionBenchmarkHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).
                        Sum(a => Convert.ToDecimal(a.BENCHMARK_WEIGHT));
                    benchmarkWeightCash = benchmarkWeightCash + dimensionBenchmarkHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH")).
                        Sum(a => Convert.ToDecimal(a.BENCHMARK_WEIGHT));
                    activePosition = modelWeight - benchmarkWeight;
                    AssetAllocationData data = new AssetAllocationData();
                    data.BenchmarkWeight = benchmarkWeight;
                    data.Country = item;
                    data.ModelWeight = modelWeight;
                    data.PortfolioWeight = portfolioWeight;
                    data.PortfolioId = portfolioSelectionData.PortfolioId;
                    data.ActivePosition = activePosition;
                    result.Add(data);
                }
                if (dimensionPortfolioHoldingsData.Any(a => a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH"))
                {
                    AssetAllocationData dataCash = new AssetAllocationData();
                    dataCash.BenchmarkWeight = benchmarkWeight;
                    dataCash.Country = "CASH";
                    dataCash.ModelWeight = modelWeightCash;
                    dataCash.PortfolioWeight = portfolioWeightCash;
                    dataCash.PortfolioId = portfolioSelectionData.PortfolioId;
                    dataCash.ActivePosition = modelWeightCash - benchmarkWeightCash;
                    result.Add(dataCash);
                }
                return result.OrderBy(a => a.Country).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Static Method calculating asset allocations-LookThru View
        /// </summary>
        /// <param name="dimensionPortfolioLTHoldingsData">Collection GF_PORTFOLIO_LTHOLDINGS retrieved from Dimension</param>
        /// <param name="portfolioSelectionData">Data of Currently selected Portfolio</param>
        /// <returns>Collection of Asset Allocation Data</returns>
        public static List<AssetAllocationData> CalculateAssetAllocationValuesLT(List<GreenField.DAL.GF_PORTFOLIO_LTHOLDINGS> dimensionPortfolioLTHoldingsData, List<GreenField.DAL.GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData, PortfolioSelectionData portfolioSelectionData)
        {
            try
            {
                decimal? modelWeight = 0;
                decimal? benchmarkWeight = 0;
                decimal? activePosition = 0;
                decimal? modelWeightCash = 0;
                decimal? portfolioWeightCash = 0;
                decimal? benchmarkWeightCash = 0;
                decimal? portfolioWeight = 0;
                decimal? sumDirtyValuePC = 0;
                decimal? sumModelWeight = 0;
                decimal? sumBenchmarkWeight = 0;
                List<AssetAllocationData> result = new List<AssetAllocationData>();

                if ((dimensionPortfolioLTHoldingsData == null) || (portfolioSelectionData == null))
                {
                    throw new ArgumentNullException();
                }
                List<string> countryNames = dimensionPortfolioLTHoldingsData.Select(a => a.COUNTRYNAME).Distinct().ToList();
                if (countryNames.Count == 0)
                {
                    throw new InvalidOperationException();
                }
                foreach (string item in countryNames)
                {
                    sumDirtyValuePC = dimensionPortfolioLTHoldingsData.Sum(a => Convert.ToDecimal(a.DIRTY_VALUE_PC));
                    sumBenchmarkWeight = dimensionBenchmarkHoldingsData.Sum(a => Convert.ToDecimal(a.BENCHMARK_WEIGHT));
                    //if sum of DirtyValuePC or ModelWeight is zero then return empty set
                    if ((sumDirtyValuePC == 0) || (sumBenchmarkWeight == 0) || (sumModelWeight == 0))
                    {
                        return result;
                    }
                    modelWeight = dimensionPortfolioLTHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).Sum(a => Convert.ToDecimal(a.ASH_EMM_MODEL_WEIGHT)) / sumModelWeight;
                    portfolioWeight = dimensionPortfolioLTHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).Sum(a => Convert.ToDecimal(a.DIRTY_VALUE_PC)) / sumDirtyValuePC;
                    modelWeightCash = modelWeightCash + dimensionPortfolioLTHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH")).Sum(a => Convert.ToDecimal(a.ASH_EMM_MODEL_WEIGHT)) / sumModelWeight;
                    portfolioWeightCash = portfolioWeightCash + dimensionPortfolioLTHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH")).Sum(a => Convert.ToDecimal(a.DIRTY_VALUE_PC)) / sumDirtyValuePC;

                    benchmarkWeight = dimensionBenchmarkHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() != "CASH")).Sum(a => Convert.ToDecimal(a.BENCHMARK_WEIGHT));
                    benchmarkWeightCash = benchmarkWeightCash + dimensionBenchmarkHoldingsData.
                        Where(a => (a.COUNTRYNAME == item) && (a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH")).Sum(a => Convert.ToDecimal(a.BENCHMARK_WEIGHT));
                    activePosition = modelWeight - benchmarkWeight;

                    //adding values which are not cash
                    AssetAllocationData data = new AssetAllocationData();
                    data.BenchmarkWeight = benchmarkWeight;
                    data.Country = item;
                    data.ModelWeight = modelWeight;
                    data.PortfolioWeight = portfolioWeight;
                    data.PortfolioId = portfolioSelectionData.PortfolioId;
                    data.ActivePosition = activePosition;
                    result.Add(data);
                }

                //inclusing cash values
                if (dimensionPortfolioLTHoldingsData.Any(a => a.SECURITYTHEMECODE.ToUpper().Trim() == "CASH"))
                {
                    AssetAllocationData dataCash = new AssetAllocationData();
                    dataCash.BenchmarkWeight = benchmarkWeight;
                    dataCash.Country = "CASH";
                    dataCash.ModelWeight = modelWeightCash;
                    dataCash.PortfolioWeight = portfolioWeightCash;
                    dataCash.PortfolioId = portfolioSelectionData.PortfolioId;
                    dataCash.ActivePosition = modelWeightCash - benchmarkWeightCash;
                    result.Add(dataCash);
                }
                return result.OrderBy(a => a.Country).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }        
    }
}