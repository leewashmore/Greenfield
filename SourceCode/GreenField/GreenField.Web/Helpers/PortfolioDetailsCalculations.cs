using System;
using System.Collections.Generic;
using System.Linq;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using System.Diagnostics;
using GreenField.DAL;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations for Portfolio Details UI
    /// </summary>
    public static class PortfolioDetailsCalculations
    {
        /// <summary>
        /// Method to add Securites only present in Benchmark_Holdings to result set
        /// </summary>
        /// <param name="result">Collection of PortfolioDetailsData containing data of Securities held by Portfolio</param>
        /// <param name="onlyBenchmarkSecurities">Collection of GF_BENCHMARK_HOLDINGS, contains securities only held by Benchmark & not by Portfolio</param>
        /// <returns>Collection of PortfolioDetailsData</returns>
        public static List<PortfolioDetailsData> AddBenchmarkSecurities(List<PortfolioDetailsData> result, List<GF_BENCHMARK_HOLDINGS> onlyBenchmarkSecurities, Boolean isFiltered, decimal? sumBenchmarkWeight)
        {
            if (onlyBenchmarkSecurities == null)
            {
                return result;
            }
            if (onlyBenchmarkSecurities.Count == 0)
            {
                return result;
            }
            if (result == null)
            {
                return new List<PortfolioDetailsData>();
            }
                        
            Debug.WriteLine(onlyBenchmarkSecurities.Count());
            foreach (GF_BENCHMARK_HOLDINGS item in onlyBenchmarkSecurities)
            {
                PortfolioDetailsData benchmarkResult = new PortfolioDetailsData();
                benchmarkResult.AsecSecShortName = item.ASEC_SEC_SHORT_NAME;
                benchmarkResult.IssueName = item.ISSUE_NAME;
                benchmarkResult.Ticker = item.TICKER;
                benchmarkResult.ProprietaryRegionCode = item.ASHEMM_PROP_REGION_CODE;
                benchmarkResult.IsoCountryCode = item.ISO_COUNTRY_CODE;
                benchmarkResult.SectorName = item.GICS_SECTOR_NAME;
                benchmarkResult.IndustryName = item.GICS_INDUSTRY_NAME;
                benchmarkResult.SubIndustryName = item.GICS_SUB_INDUSTRY_NAME;
                benchmarkResult.MarketCapUSD = item.MARKET_CAP_IN_USD;
                benchmarkResult.SecurityType = item.SECURITY_TYPE;
                benchmarkResult.BalanceNominal = item.BALANCE_NOMINAL;
                benchmarkResult.DirtyValuePC = item.DIRTY_VALUE_PC;
              //  benchmarkResult.BenchmarkWeight = item.BENCHMARK_WEIGHT;
                benchmarkResult.AshEmmModelWeight = item.ASH_EMM_MODEL_WEIGHT;
                benchmarkResult.Type = "BENCHMARK";
                benchmarkResult.IssuerId = item.ISSUER_ID;
                if (isFiltered)
                {
                    if (sumBenchmarkWeight != 0)
                    {
                        //benchmarkResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                        //            Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                        //            Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault().BENCHMARK_WEIGHT) / sumBenchmarkWeight;
                        benchmarkResult.BenchmarkWeight = (item.BENCHMARK_WEIGHT== null?0:item.BENCHMARK_WEIGHT) *100 / sumBenchmarkWeight;

                       
                    }
                    else
                    {
                        benchmarkResult.BenchmarkWeight = 0;
                    }
                }
                else
                {
                    //benchmarkResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                    //                Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                    //                Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault().BENCHMARK_WEIGHT);
                    benchmarkResult.BenchmarkWeight = (item.BENCHMARK_WEIGHT == null ? 0 : item.BENCHMARK_WEIGHT);

                }
                result.Add(benchmarkResult);
            }
            return result;
        }

        /// <summary>
        /// Method to calculate the Portfolio Weight & ActivePosition
        /// </summary>
        /// <param name="portfolioDetailsData">Collection of PortfolioDetailsData</param>
        /// <returns>List of PortfolioDetailsData</returns>
        public static List<PortfolioDetailsData> CalculatePortfolioDetails(List<PortfolioDetailsData> portfolioDetailsData)
        {
            if (portfolioDetailsData == null)
            {
                throw new InvalidOperationException();
            }
            if (portfolioDetailsData.Count == 0)
            {
                return new List<PortfolioDetailsData>();
            }
            decimal? sumDirtyValuePC = 0;
            decimal? sumModelWeight = 0;

            sumDirtyValuePC = portfolioDetailsData.Sum(a => a.DirtyValuePC);
            sumModelWeight = portfolioDetailsData.Sum(a => a.AshEmmModelWeight);

            //Removed for Demo
            //if (sumModelWeight == 0 || sumDirtyValuePC == 0)
            //    return new List<PortfolioDetailsData>();

            foreach (PortfolioDetailsData item in portfolioDetailsData)
            {
                item.PortfolioWeight = item.DirtyValuePC / sumDirtyValuePC;
                item.RePortfolioWeight = item.PortfolioWeight;
                item.ReBenchmarkWeight = item.BenchmarkWeight;
                item.ReAshEmmModelWeight = item.AshEmmModelWeight;
                item.ActivePosition = item.PortfolioWeight - item.BenchmarkWeight;
                //item.AshEmmModelWeight = item.AshEmmModelWeight / sumModelWeight;
            }
            return portfolioDetailsData;
        }

        /// <summary>
        /// Add Data returned from View(GF_PORTFOLIO_HOLDINGS) to resultSet
        /// </summary>
        /// <param name="dimensionPortfolioHoldingsData">List of type GF_PORTFOLIO_HOLDINGS returned from GF_PORTFOLIO_LTHOLDINGS</param>
        /// <param name="dimensionBenchmarkHoldingsData">List of type GF_BENCHMARK_HOLDINGS returned from GF_BENCHMARK_HOLDINGS</param>
        /// <returns>List of PortfolioDetailsData</returns>
        public static List<PortfolioDetailsData> AddPortfolioSecurities(List<GF_PORTFOLIO_HOLDINGS> dimensionPortfolioHoldingsData, List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData,Boolean isFiltered)
        {

            List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();

            if (dimensionPortfolioHoldingsData == null)
            {
                return result;
            }
            if (dimensionPortfolioHoldingsData.Count == 0)
            {
                return result;
            }
            if (dimensionBenchmarkHoldingsData == null)
            {
                return result;
            }

            decimal? sumBenchmarkWeight = 0;
            sumBenchmarkWeight = dimensionBenchmarkHoldingsData.Sum(a => a.BENCHMARK_WEIGHT);
            foreach (GF_PORTFOLIO_HOLDINGS item in dimensionPortfolioHoldingsData)
            {
                PortfolioDetailsData portfolioResult = new PortfolioDetailsData();
                portfolioResult.AsecSecShortName = item.ASEC_SEC_SHORT_NAME;
                portfolioResult.IssueName = item.ISSUE_NAME; 
                portfolioResult.Ticker = item.TICKER;
                portfolioResult.ProprietaryRegionCode = item.ASHEMM_PROP_REGION_CODE;
                portfolioResult.IsoCountryCode = item.ISO_COUNTRY_CODE;
                portfolioResult.SectorName = item.GICS_SECTOR_NAME;
                portfolioResult.IndustryName = item.GICS_INDUSTRY_NAME;
                portfolioResult.SubIndustryName = item.GICS_SUB_INDUSTRY_NAME;
                portfolioResult.MarketCapUSD = item.MARKET_CAP_IN_USD;
                portfolioResult.SecurityType = item.SECURITY_TYPE;
                portfolioResult.BalanceNominal = item.BALANCE_NOMINAL;
                portfolioResult.DirtyValuePC = item.DIRTY_VALUE_PC;
                portfolioResult.PortfolioId = item.PORTFOLIO_ID;
                if (isFiltered)
                {
                    if (sumBenchmarkWeight != 0)
                    {
                        portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault().BENCHMARK_WEIGHT) *100 / sumBenchmarkWeight;

                    }
                    else
                    {
                        portfolioResult.BenchmarkWeight = 0;
                    }
                }
                else
                {
                    portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault().BENCHMARK_WEIGHT);
                }
                //portfolioResult.AshEmmModelWeight = item.ASH_EMM_MODEL_WEIGHT;
               

                portfolioResult.IssuerId = item.ISSUER_ID;
                result.Add(portfolioResult);
            }
            return result;
        }

        /// Add Data returned from View(GF_PORTFOLIO_HOLDINGS) to resultSet
        /// </summary>
        /// <param name="dimensionPortfolioHoldingsData">List of type GF_PORTFOLIO_HOLDINGS returned from GF_PORTFOLIO_LTHOLDINGS</param>
        /// <param name="dimensionBenchmarkHoldingsData">List of type GF_BENCHMARK_HOLDINGS returned from GF_BENCHMARK_HOLDINGS</param>
        /// <returns>List of PortfolioDetailsData</returns>
        public static List<PortfolioDetailsData> AddCompositePortfolioSecurities(List<GF_COMPOSITE_LTHOLDINGS> compositeHoldingsData, List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData, Boolean isFiltered)
        {

            List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();

            if (compositeHoldingsData == null)
            {
                return result;
            }
            if (compositeHoldingsData.Count == 0)
            {
                return result;
            }
            if (dimensionBenchmarkHoldingsData == null)
            {
                return result;
            }

            decimal? sumBenchmarkWeight = 0;
            sumBenchmarkWeight = dimensionBenchmarkHoldingsData.Sum(a => a.BENCHMARK_WEIGHT);
            foreach (GF_COMPOSITE_LTHOLDINGS item in compositeHoldingsData)
            {
                PortfolioDetailsData portfolioResult = new PortfolioDetailsData();
                portfolioResult.AsecSecShortName = item.ASEC_SEC_SHORT_NAME;
                portfolioResult.IssueName = item.ISSUE_NAME;
                portfolioResult.Ticker = item.TICKER;
                portfolioResult.ProprietaryRegionCode = item.ASHEMM_PROP_REGION_CODE;
                portfolioResult.IsoCountryCode = item.ISO_COUNTRY_CODE;
                portfolioResult.SectorName = item.GICS_SECTOR_NAME;
                portfolioResult.IndustryName = item.GICS_INDUSTRY_NAME;
                portfolioResult.SubIndustryName = item.GICS_SUB_INDUSTRY_NAME;
                portfolioResult.MarketCapUSD = item.MARKET_CAP_IN_USD;
                portfolioResult.SecurityType = item.SECURITY_TYPE;
                portfolioResult.BalanceNominal = item.BALANCE_NOMINAL;
                portfolioResult.DirtyValuePC = item.DIRTY_VALUE_PC;
                portfolioResult.PortfolioId = item.PORTFOLIO_ID;
                if (isFiltered)
                {
                    if (sumBenchmarkWeight != 0)
                    {
                        portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault().BENCHMARK_WEIGHT) * 100 / sumBenchmarkWeight;

                    }
                    else
                    {
                        portfolioResult.BenchmarkWeight = 0;
                    }
                }
                else
                {
                    portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                                    Where(a => a.ASEC_SEC_SHORT_NAME == portfolioResult.AsecSecShortName).FirstOrDefault().BENCHMARK_WEIGHT);
                }
                //portfolioResult.AshEmmModelWeight = item.ASH_EMM_MODEL_WEIGHT;


                portfolioResult.IssuerId = item.ISSUER_ID;
                Debug.Print(item.ISSUER_ID);
                result.Add(portfolioResult);
            }
            return result;
        }


        /// <summary>
        /// Add Data returned from View(GF_PORTFOLIO_LTHOLDINGS) to resultSet
        /// </summary>
        /// <param name="dimensionPortfolioLTHoldingsData">List of type GF_PORTFOLIO_LTHOLDINGS returned from GF_PORTFOLIO_LTHOLDINGS</param>
        /// <param name="dimensionBenchmarkHoldingsData">List of type GF_BENCHMARK_HOLDINGS returned from GF_BENCHMARK_HOLDINGS</param>
        /// <returns>List of PortfolioDetailsData</returns>
        public static List<PortfolioDetailsData> AddPortfolioLTSecurities(List<GF_PORTFOLIO_LTHOLDINGS> dimensionPortfolioLTHoldingsData, List<GF_BENCHMARK_HOLDINGS> dimensionBenchmarkHoldingsData,Boolean isFiltered)
        {
            List<PortfolioDetailsData> result = new List<PortfolioDetailsData>();

            if (dimensionPortfolioLTHoldingsData == null)
            {
                return result;
            }
            if (dimensionPortfolioLTHoldingsData.Count == 0)
            {
                return result;
            }
            if (dimensionBenchmarkHoldingsData == null)
            {
                return result;
            }
            decimal? sumBenchmarkWeight = 0;
            sumBenchmarkWeight = dimensionBenchmarkHoldingsData.Sum(a => a.BENCHMARK_WEIGHT);
            foreach (GF_PORTFOLIO_LTHOLDINGS item in dimensionPortfolioLTHoldingsData)
            {
                PortfolioDetailsData portfolioResult = new PortfolioDetailsData();
                portfolioResult.AsecSecShortName = item.ASEC_SEC_SHORT_NAME;
                portfolioResult.IssueName = item.ISSUE_NAME;
                portfolioResult.Ticker = item.TICKER;
                portfolioResult.PfcHoldingPortfolio = item.A_PFCHOLDINGS_PORLT;
                portfolioResult.PortfolioId = item.PORTFOLIO_ID;

                portfolioResult.PortfolioPath = item.PORPATH;
                portfolioResult.ProprietaryRegionCode = item.ASHEMM_PROP_REGION_CODE;
                portfolioResult.IsoCountryCode = item.ISO_COUNTRY_CODE;
                portfolioResult.SectorName = item.GICS_SECTOR_NAME;
                portfolioResult.IndustryName = item.GICS_INDUSTRY_NAME;
                portfolioResult.SubIndustryName = item.GICS_SUB_INDUSTRY_NAME;
                portfolioResult.MarketCapUSD = item.MARKET_CAP_IN_USD;
                portfolioResult.SecurityType = item.SECURITY_TYPE;
                portfolioResult.BalanceNominal = item.BALANCE_NOMINAL;
                portfolioResult.DirtyValuePC = item.DIRTY_VALUE_PC;

                if (isFiltered)
                {
                    if (sumBenchmarkWeight != 0)
                    {
                        portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                            Where(a => a.ISSUE_NAME == portfolioResult.IssueName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                            Where(a => a.ISSUE_NAME == portfolioResult.IssueName).FirstOrDefault().BENCHMARK_WEIGHT) * 100/ sumBenchmarkWeight;

                    }
                    else
                    {
                        portfolioResult.BenchmarkWeight = 0;
                    }
                }
                else
                {
                    portfolioResult.BenchmarkWeight = ((dimensionBenchmarkHoldingsData.
                            Where(a => a.ISSUE_NAME == portfolioResult.IssueName).FirstOrDefault() == null) ? 0 : dimensionBenchmarkHoldingsData.
                            Where(a => a.ISSUE_NAME == portfolioResult.IssueName).FirstOrDefault().BENCHMARK_WEIGHT);
                }

                
                portfolioResult.AshEmmModelWeight = item.ASH_EMM_MODEL_WEIGHT;
                portfolioResult.IssuerId = item.ISSUER_ID;
                result.Add(portfolioResult);
            }
            return result;
        }
    }
}