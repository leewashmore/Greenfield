using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;
using GreenField.Web.Services;
using GreenField.Web.DimensionEntitiesService;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations for Multi-Line Benchmark UI
    /// </summary>
    public static class MultiLineBenchmarkUICalculations
    {
        /// <summary>
        /// Calculations for Multi-LineBenchmarkUI chart
        /// </summary>
        /// <param name="dimensionMonthlyPerfData">Collection of GF_PERF_MOTHLY_ATTRIBUTION retrieved from Dimension for the selected security & portfolio for a specified date </param>
        /// <returns>List of type BenchmarkChartReturnData</returns>
        public static List<BenchmarkChartReturnData> RetrieveBenchmarkChartData(List<GF_PERF_DAILY_ATTRIBUTION> dimensionDailyPerfData)
        {
            try
            {
                //Arguement null Exception
                if (dimensionDailyPerfData == null)
                    throw new InvalidOperationException();

                List<BenchmarkChartReturnData> result = new List<BenchmarkChartReturnData>();

                BenchmarkChartReturnData data = new BenchmarkChartReturnData();

                foreach (GF_PERF_DAILY_ATTRIBUTION item in dimensionDailyPerfData)
                {
                    data = new BenchmarkChartReturnData();
                    data.Name = (item.NODE_NAME.ToUpper().Trim() == "COUNTRY") ? (item.BMNAME + " " + item.AGG_LVL_1_LONG_NAME) : (item.BMNAME);
                    data.Type = (item.NODE_NAME.ToUpper().Trim() == "COUNTRY") ? "COUNTRY INDEX" : "BENCHMARK";
                    data.FromDate = (DateTime)item.TO_DATE;
                    data.OneD = Convert.ToDecimal(item.BM1_RC_AVG_WGT_1D);
                    data.WTD = Convert.ToDecimal(item.BM1_RC_AVG_WGT_1W);
                    data.MTD = Convert.ToDecimal(item.BM1_RC_AVG_WGT_MTD);
                    data.QTD = Convert.ToDecimal(item.BM1_RC_AVG_WGT_QTD);
                    data.YTD = Convert.ToDecimal(item.BM1_RC_AVG_WGT_YTD);
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

        /// <summary>
        /// Calculations for MultiLineBenchmarkUI Grid Data
        /// </summary>
        /// <param name="dimensionDailyPerfData">Collection of GF_PERF_DAILY_ATTRIBUTION retrieved from Dimension for the selected security, portfolio 
        /// and the associated Index for a specified date</param>
        /// <returns>Collection of BenchmarkGridReturnData</returns>
        public static List<BenchmarkGridReturnData> RetrieveBenchmarkGridData(List<GF_PERF_DAILY_ATTRIBUTION> dimensionDailyPerfData)
        {
            try
            {
                List<BenchmarkGridReturnData> result = new List<BenchmarkGridReturnData>();

                //Arguement null exception
                if (dimensionDailyPerfData == null)
                    throw new InvalidOperationException();

                #region Dates

                DateTime firstDayPreviousMonth;
                DateTime firstDayCurrentMonth;
                DateTime currentDate = DateTime.Today;

                DateTime startDatePreviousYear = new DateTime(currentDate.Year - 1, 12, 1);
                DateTime endDatePreviousYear = new DateTime(currentDate.Year - 1, 12, 31);

                DateTime startDateTwoPreviousYear = new DateTime(currentDate.Year - 2, 12, 1);
                DateTime endDateTwoPreviousYear = new DateTime(currentDate.Year - 2, 12, 31);

                DateTime startDateThreePreviousYear = new DateTime(currentDate.Year - 3, 12, 1);
                DateTime endDateThreePreviousYear = new DateTime(currentDate.Year - 3, 12, 31);

                if (currentDate.Month == 1)
                    firstDayPreviousMonth = new DateTime(currentDate.Year - 1, 12, 1);
                else
                    firstDayPreviousMonth = new DateTime(currentDate.Year, currentDate.Month - 1, 1);

                firstDayCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                DateTime startDate = firstDayPreviousMonth;
                DateTime endDate = firstDayCurrentMonth;

                #endregion

                BenchmarkGridReturnData data = new BenchmarkGridReturnData();

                //Adding details for Benchmark
                data.Name = dimensionDailyPerfData.Select(a => a.BMNAME).First();
                data.Type = "BENCHMARK";
                data.MTD = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "SECURITY ID" && (a.TO_DATE > startDate && a.TO_DATE < endDate)).ToList()).Select(a => a.POR_RC_AVG_WGT_MTD).First());
                data.QTD = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "SECURITY ID" && (a.TO_DATE > startDate && a.TO_DATE < endDate)).ToList()).Select(a => a.POR_RC_AVG_WGT_QTD).First());
                data.YTD = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "SECURITY ID" && (a.TO_DATE > startDate && a.TO_DATE < endDate)).ToList()).Select(a => a.POR_RC_AVG_WGT_YTD).First());
                data.PreviousYearReturn = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "SECURITY ID" && (a.TO_DATE > startDatePreviousYear && a.TO_DATE < endDatePreviousYear)).ToList()).
                    Select(a => a.POR_RC_AVG_WGT_YTD).First());
                data.TwoPreviousYearReturn = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "SECURITY ID" && (a.TO_DATE > startDateTwoPreviousYear && a.TO_DATE < endDateTwoPreviousYear)).ToList()).
                    Select(a => a.POR_RC_AVG_WGT_YTD).First());
                data.ThreePreviousYearReturn = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "SECURITY ID" && (a.TO_DATE > startDateThreePreviousYear && a.TO_DATE < endDateThreePreviousYear)).ToList()).
                    Select(a => a.POR_RC_AVG_WGT_YTD).First());

                result.Add(data);

                data = null;

                //Adding details for Country
                data.Name = dimensionDailyPerfData.Select(a => a.BMNAME).First() + " " +
                    (dimensionDailyPerfData.Where(a => a.NODE_NAME == "COUNTRY").ToList()).Select(a => a.AGG_LVL_1_LONG_NAME).First();
                data.Type = "COUNTRY INDEX";
                data.MTD = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "COUNTRY" && (a.TO_DATE > startDate && a.TO_DATE < endDate)).ToList()).Select(a => a.POR_RC_AVG_WGT_MTD).First());
                data.QTD = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "COUNTRY" && (a.TO_DATE > startDate && a.TO_DATE < endDate)).ToList()).Select(a => a.POR_RC_AVG_WGT_QTD).First());
                data.YTD = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "COUNTRY" && (a.TO_DATE > startDate && a.TO_DATE < endDate)).ToList()).Select(a => a.POR_RC_AVG_WGT_YTD).First());
                data.PreviousYearReturn = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "COUNTRY" && (a.TO_DATE > startDatePreviousYear && a.TO_DATE < endDatePreviousYear)).ToList()).
                    Select(a => a.POR_RC_AVG_WGT_YTD).First());
                data.TwoPreviousYearReturn = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "COUNTRY" && (a.TO_DATE > startDateTwoPreviousYear && a.TO_DATE < endDateTwoPreviousYear)).ToList()).
                    Select(a => a.POR_RC_AVG_WGT_YTD).First());
                data.ThreePreviousYearReturn = Convert.ToDecimal((dimensionDailyPerfData.
                    Where(a => a.NODE_NAME.ToUpper() == "COUNTRY" && (a.TO_DATE > startDateThreePreviousYear && a.TO_DATE < endDateThreePreviousYear)).ToList()).
                    Select(a => a.POR_RC_AVG_WGT_YTD).First());
                result.Add(data);

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