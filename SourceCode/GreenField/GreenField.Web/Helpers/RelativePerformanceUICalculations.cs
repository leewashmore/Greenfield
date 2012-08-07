using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculation logic for RelativePerformanceUI gadget
    /// </summary>
    public static class RelativePerformanceUICalculations
    {
        /// <summary>
        /// Method to calculate RelativePerformanceUI Data
        /// </summary>
        /// <param name="dimensionDailyPerfData">Collection of type GF_PERF_DAILY_ATTRIBUTION retrieved from Dimension</param>
        /// <returns>List of RelativePerformanceUIData</returns>
        public static List<RelativePerformanceUIData> CalculateRelativePerformanceUIData(List<GF_PERF_DAILY_ATTRIBUTION> dimensionDailyPerfData,
            GF_PERF_DAILY_ATTRIBUTION dimensionBenchmarkReturnData)
        {
            try
            {
                if (dimensionDailyPerfData == null && dimensionBenchmarkReturnData == null)
                    return new List<RelativePerformanceUIData>();

                List<RelativePerformanceUIData> result = new List<RelativePerformanceUIData>();

                RelativePerformanceUIData data = new RelativePerformanceUIData();
                if (dimensionDailyPerfData != null)
                {
                    if (dimensionDailyPerfData.Count != 0)
                    {
                        foreach (GF_PERF_DAILY_ATTRIBUTION item in dimensionDailyPerfData)
                        {
                            data = new RelativePerformanceUIData();
                            data.EffectiveDate = Convert.ToDateTime(item.TO_DATE);
                            if (item.NODE_NAME.ToUpper().Trim() == "SECURITY ID")
                            {
                                data.EntityType = item.NODE_NAME;
                                data.EntityName = item.AGG_LVL_1_LONG_NAME;
                                data.QTDReturn = Convert.ToDecimal(item.ADJ_RTN_POR_RC_TWR_QTD) * 100;
                                data.MTDReturn = Convert.ToDecimal(item.ADJ_RTN_POR_RC_TWR_MTD) * 100;
                                data.YTDReturn = Convert.ToDecimal(item.ADJ_RTN_POR_RC_TWR_YTD) * 100;
                                data.OneYearReturn = Convert.ToDecimal(item.ADJ_RTN_POR_RC_TWR_1Y) * 100;
                                data.SortId = 1;
                            }
                            else
                            {
                                data.EntityType = item.NODE_NAME;
                                data.EntityName = item.AGG_LVL_1_LONG_NAME;
                                data.MTDReturn = Convert.ToDecimal(item.BM1_RC_TWR_MTD) * 100;
                                data.QTDReturn = Convert.ToDecimal(item.BM1_RC_TWR_QTD) * 100;
                                data.YTDReturn = Convert.ToDecimal(item.BM1_RC_TWR_YTD) * 100;
                                data.OneYearReturn = Convert.ToDecimal(item.BM1_RC_TWR_1Y) * 100;
                                if (item.NODE_NAME.ToUpper().Trim() == "COUNTRY")
                                    data.SortId = 3;
                                else
                                    data.SortId = 4;
                            }
                            result.Add(data);
                        }

                    }
                }

                if (dimensionBenchmarkReturnData != null)
                {
                    //Adding Returns for Benchmark
                    if (data != null)
                    {
                        data = new RelativePerformanceUIData();
                        data.EntityType = "BENCHMARK";
                        data.EntityName = Convert.ToString(dimensionBenchmarkReturnData.BMNAME);
                        data.MTDReturn = Convert.ToDecimal(dimensionBenchmarkReturnData.BM1_TOP_RC_TWR_MTD) * 100;
                        data.QTDReturn = Convert.ToDecimal(dimensionBenchmarkReturnData.BM1_TOP_RC_TWR_QTD) * 100;
                        data.YTDReturn = Convert.ToDecimal(dimensionBenchmarkReturnData.BM1_TOP_RC_TWR_YTD) * 100;
                        data.OneYearReturn = Convert.ToDecimal(dimensionBenchmarkReturnData.BM1_TOP_RC_TWR_1Y) * 100;
                        data.SortId = 2;
                        result.Add(data);
                    }
                }

                //To be Removed
                if (result != null)
                {
                    RelativePerformanceUIData removeSecurity = result.Where(a => a.EntityType.ToUpper().Trim() == "SECURITY ID").FirstOrDefault();
                    if (removeSecurity != null)
                        result.Remove(removeSecurity);
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