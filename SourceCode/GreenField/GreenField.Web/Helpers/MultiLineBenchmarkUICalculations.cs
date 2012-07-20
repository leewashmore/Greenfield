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
        public static List<BenchmarkChartReturnData> RetrieveBenchmarkChartData(List<GF_PERF_DAILY_ATTRIBUTION> countrySectorReturns, List<GF_PERF_DAILY_ATTRIBUTION> benchmarkReturns)
        {
            try
            {
                //Arguement null Exception
                if (benchmarkReturns == null)
                    throw new InvalidOperationException();

                if (benchmarkReturns.Count == 0)
                    return new List<BenchmarkChartReturnData>();

                List<BenchmarkChartReturnData> result = new List<BenchmarkChartReturnData>();

                BenchmarkChartReturnData data = new BenchmarkChartReturnData();
                //Returns for Country/Sector
                if (countrySectorReturns != null)
                {
                    if (countrySectorReturns.Count != 0)
                    {
                        foreach (GF_PERF_DAILY_ATTRIBUTION item in countrySectorReturns)
                        {
                            data = new BenchmarkChartReturnData();
                            data.Name = (item.BMNAME + " " + Convert.ToString(item.AGG_LVL_1_LONG_NAME));
                            data.Type = (item.NODE_NAME.ToUpper().Trim() == "COUNTRY") ? "COUNTRY INDEX" : "SECTOR";
                            data.FromDate = (DateTime)item.TO_DATE;
                            data.OneD = Convert.ToDecimal(item.BM1_RC_TWR_1D);
                            data.WTD = Convert.ToDecimal(item.BM1_RC_TWR_1W);
                            data.MTD = Convert.ToDecimal(item.BM1_RC_TWR_MTD);
                            data.QTD = Convert.ToDecimal(item.BM1_RC_TWR_QTD);
                            data.YTD = Convert.ToDecimal(item.BM1_RC_TWR_YTD);
                            data.OneY = Convert.ToDecimal(item.BM1_RC_TWR_1Y);
                            result.Add(data);
                        }
                    }
                }

                //Returns for Benchmark
                if (benchmarkReturns != null)
                {
                    if (benchmarkReturns.Count != 0)
                    {
                        foreach (GF_PERF_DAILY_ATTRIBUTION item in benchmarkReturns)
                        {
                            data = new BenchmarkChartReturnData();
                            data.Name = Convert.ToString(item.BMNAME);
                            data.Type = "BENCHMARK";
                            data.FromDate = Convert.ToDateTime(item.TO_DATE);
                            data.OneD = Convert.ToDecimal(item.BM1_TOP_RC_TWR_1D);
                            data.WTD = Convert.ToDecimal(item.BM1_TOP_RC_TWR_1W);
                            data.MTD = Convert.ToDecimal(item.BM1_TOP_RC_TWR_MTD);
                            data.QTD = Convert.ToDecimal(item.BM1_TOP_RC_TWR_QTD);
                            data.YTD = Convert.ToDecimal(item.BM1_TOP_RC_TWR_YTD);
                            data.OneY = Convert.ToDecimal(item.BM1_TOP_RC_TWR_1Y);
                            result.Add(data);
                        }
                    }
                }

                return result.OrderBy(a => a.Type).ToList();
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
        /// <param name="sectorCountryReturn">Collection of GF_PERF_DAILY_ATTRIBUTION retrieved from Dimension for the selected security, portfolio 
        /// and the associated Index for a specified date</param>
        /// <returns>Collection of BenchmarkGridReturnData</returns>
        public static List<BenchmarkGridReturnData> RetrieveBenchmarkGridData(List<GF_PERF_DAILY_ATTRIBUTION> sectorCountryReturn, List<GF_PERF_DAILY_ATTRIBUTION> benchmarkReturn)
        {
            try
            {
                List<BenchmarkGridReturnData> result = new List<BenchmarkGridReturnData>();

                //Arguement null exception
                if (benchmarkReturn == null)
                    return result;

                if (benchmarkReturn.Count == 0)
                    return result;

                #region Dates
                int numberOfDays = 0;
                DateTime lastDayPreviousMonth;
                DateTime currentDate = DateTime.Today;

                DateTime endDatePreviousYear = new DateTime(currentDate.Year - 1, 12, 31);
                DateTime endDateTwoPreviousYear = new DateTime(currentDate.Year - 2, 12, 31);
                DateTime endDateThreePreviousYear = new DateTime(currentDate.Year - 3, 12, 31);

                if (currentDate.Month == 1)
                    lastDayPreviousMonth = new DateTime(currentDate.Year - 1, 12, 1);
                else
                {
                    numberOfDays = DateTime.DaysInMonth(currentDate.Year, currentDate.Month - 1);
                    lastDayPreviousMonth = new DateTime(currentDate.Year, currentDate.Month - 1, numberOfDays);
                }

                #endregion

                BenchmarkGridReturnData data = new BenchmarkGridReturnData();

                //Adding details for Country/Sector
                data.Name = Convert.ToString(sectorCountryReturn.Select(a => a.BMNAME).FirstOrDefault()) + " " +
                    Convert.ToString(sectorCountryReturn.Select(a => a.AGG_LVL_1_LONG_NAME).FirstOrDefault());

                data.Type = Convert.ToString(sectorCountryReturn.Select(a => a.NODE_NAME).FirstOrDefault());
                data.MTD = Convert.ToDecimal((sectorCountryReturn.
                    Where(a => a.TO_DATE == lastDayPreviousMonth).Select(a => a.BM1_RC_TWR_MTD).FirstOrDefault()));
                data.QTD = Convert.ToDecimal((sectorCountryReturn.
                    Where(a => a.TO_DATE == lastDayPreviousMonth).Select(a => a.BM1_RC_TWR_QTD).FirstOrDefault()));
                data.YTD = Convert.ToDecimal((sectorCountryReturn.
                    Where(a => a.TO_DATE == lastDayPreviousMonth).Select(a => a.BM1_RC_TWR_YTD).FirstOrDefault()));
                data.PreviousYearReturn = Convert.ToDecimal((sectorCountryReturn.
                    Where(a => a.TO_DATE == endDatePreviousYear).Select(a => a.BM1_RC_TWR_YTD).FirstOrDefault()));
                data.TwoPreviousYearReturn = Convert.ToDecimal((sectorCountryReturn.
                    Where(a => a.TO_DATE == endDateTwoPreviousYear).Select(a => a.BM1_RC_TWR_YTD).FirstOrDefault()));
                data.ThreePreviousYearReturn = Convert.ToDecimal((sectorCountryReturn.
                    Where(a => a.TO_DATE == endDateThreePreviousYear).Select(a => a.BM1_RC_TWR_YTD).FirstOrDefault()));

                result.Add(data);

                data = new BenchmarkGridReturnData();

                //Adding details for Benchmark
                data.Name = Convert.ToString(benchmarkReturn.Select(a => a.BMNAME).FirstOrDefault());
                data.Type = "BENCHMARK";
                data.MTD = Convert.ToDecimal((benchmarkReturn.Where(a=>a.TO_DATE==lastDayPreviousMonth).Select(a=>a.BM1_TOP_RC_TWR_MTD).FirstOrDefault()));
                data.QTD = Convert.ToDecimal((benchmarkReturn.
                    Where(a => a.TO_DATE == lastDayPreviousMonth).Select(a => a.BM1_TOP_RC_TWR_QTD).FirstOrDefault()));
                data.YTD = Convert.ToDecimal((benchmarkReturn.
                    Where(a => a.TO_DATE == lastDayPreviousMonth).Select(a => a.BM1_TOP_RC_TWR_YTD).FirstOrDefault()));
                data.PreviousYearReturn = Convert.ToDecimal((benchmarkReturn.
                    Where(a => a.TO_DATE == endDatePreviousYear).Select(a => a.BM1_TOP_RC_TWR_YTD).FirstOrDefault()));
                data.TwoPreviousYearReturn = Convert.ToDecimal((benchmarkReturn.
                    Where(a => a.TO_DATE == endDateTwoPreviousYear).Select(a => a.BM1_TOP_RC_TWR_YTD).FirstOrDefault()));
                data.ThreePreviousYearReturn = Convert.ToDecimal((benchmarkReturn.
                    Where(a => a.TO_DATE == endDateThreePreviousYear).Select(a => a.BM1_TOP_RC_TWR_YTD).FirstOrDefault()));
                result.Add(data);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        public static List<DateTime> CalculateEndDates()
        {
            try
            {
                List<DateTime> result = new List<DateTime>();
                DateTime startDate = DateTime.Today;
                int currentMonth = DateTime.Today.Month;
                int currentYear = DateTime.Today.Year;

                int numberOfDays = 0;
                DateTime lastDay;

                DateTime addDate = DateTime.Today;
                //Find last dates of months in current year

                for (int i = 1; i < currentMonth; i++)
                {
                    numberOfDays = DateTime.DaysInMonth(currentYear, i);
                    lastDay = new DateTime(currentYear, i, numberOfDays);
                    result.Add(lastDay);
                }

                for (int i = 12; i > currentMonth - 1; i--)
                {
                    numberOfDays = DateTime.DaysInMonth(currentYear - 1, i);
                    lastDay = new DateTime(currentYear - 1, i, numberOfDays);
                    result.Add(lastDay);
                }
                return result.OrderBy(a => a.Date).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
    }
}