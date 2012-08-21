using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Class to calculate values of EPS & NetIncome
    /// </summary>
    public static class ConsensusEstimateCalculations
    {
        /// <summary>
        /// Method to fetch the Values of EPS/EPSREP/EBG
        /// </summary>
        /// <param name="medianData">Collection of type ConsensusEstimateMedian returned from DB</param>
        /// <param name="periodType">Selected PeriodType</param>
        /// <returns>Collection of type ConsensusEstimateMedian</returns>
        public static List<ConsensusEstimateMedian> CalculateEPSValues(List<ConsensusEstimateMedian> medianData, FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL)
        {
            try
            {
                List<ConsensusEstimateMedian> localMedianData = new List<ConsensusEstimateMedian>(medianData);
                ConsensusEstimateMedian addData;

                List<int> periodYear = medianData.Select(a => a.PeriodYear).ToList();
                List<string> periodTypeList = medianData.Select(a => a.PeriodType).ToList();

                List<string> quartersList = new List<string>() { "Q1", "Q2", "Q3", "Q4" };

                List<int> EstimateID = new List<int>() { 8, 9, 5 };

                if (periodType.Equals(FinancialStatementPeriodType.ANNUAL))
                {
                    foreach (int item in periodYear)
                    {
                        if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 8 && a.PeriodType == "A").FirstOrDefault() == null)
                        {
                            if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 9 && a.PeriodType == "A").FirstOrDefault() != null)
                            {
                                addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 9 && a.PeriodType == "A").FirstOrDefault();
                                addData.EstimateId = 8;
                                addData.Description = "Net Income (Pre Exceptional)";
                                localMedianData.Add(addData);
                            }

                            if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 5 && a.PeriodType == "A").FirstOrDefault() != null)
                            {
                                addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 5 && a.PeriodType == "A").FirstOrDefault();
                                addData.EstimateId = 8;
                                addData.Description = "Net Income (Pre Exceptional)";
                                localMedianData.Add(addData);
                            }
                        }
                    }
                }

                else if (periodType.Equals(FinancialStatementPeriodType.QUARTERLY))
                {
                    foreach (int item in periodYear)
                    {
                        foreach (string quarterValue in quartersList)
                        {
                            if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 11 && a.PeriodType == quarterValue).FirstOrDefault() == null)
                            {
                                if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 13 && a.PeriodType == quarterValue).FirstOrDefault() != null)
                                {
                                    addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 13 && a.PeriodType == quarterValue).FirstOrDefault();
                                    addData.EstimateId = 11;
                                    addData.Description = "Net Income (Pre Exceptional)";
                                    localMedianData.Add(addData);
                                }

                                if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 12 && a.PeriodType == quarterValue).FirstOrDefault() != null)
                                {
                                    addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 12 && a.PeriodType == quarterValue).FirstOrDefault();
                                    addData.EstimateId = 11;
                                    addData.Description = "Net Income (Pre Exceptional)";
                                    localMedianData.Add(addData);
                                }
                            }
                        }
                    }
                }

                List<ConsensusEstimateMedian> result = new List<ConsensusEstimateMedian>(localMedianData.Where(a => a.EstimateId != 9).ToList());
                result = new List<ConsensusEstimateMedian>(result.Where(a => a.EstimateId != 5).ToList());

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }


        /// <summary>
        /// Method to fetch the Values of NetIncome- NTP/NTPREP/NBG
        /// </summary>
        /// <param name="medianData">Collection of type ConsensusEstimateMedian returned from DB</param>
        /// <param name="periodType">Selected PeriodType</param>
        /// <returns>Collection of type ConsensusEstimateMedian</returns>
        public static List<ConsensusEstimateMedian> CalculateNetIncomeValues(List<ConsensusEstimateMedian> medianData, FinancialStatementPeriodType periodType = FinancialStatementPeriodType.ANNUAL)
        {
            try
            {
                List<ConsensusEstimateMedian> localMedianData = new List<ConsensusEstimateMedian>(medianData);
                ConsensusEstimateMedian addData;

                List<int> periodYear = medianData.Select(a => a.PeriodYear).ToList();
                List<string> periodTypeList = medianData.Select(a => a.PeriodType).ToList();

                List<string> quartersList = new List<string>() { "Q1", "Q2", "Q3", "Q4" };

                List<int> EstimateID = new List<int>() { 11, 13, 12 };

                if (periodType.Equals(FinancialStatementPeriodType.ANNUAL))
                {
                    foreach (int item in periodYear)
                    {
                        if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 11 && a.PeriodType == "A").FirstOrDefault() == null)
                        {
                            if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 13 && a.PeriodType == "A").FirstOrDefault() != null)
                            {
                                addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 13 && a.PeriodType == "A").FirstOrDefault();
                                addData.EstimateId = 11;
                                addData.Description = "Earnings Per Share (Pre Exceptional)";
                                localMedianData.Add(addData);
                            }

                            if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 12 && a.PeriodType == "A").FirstOrDefault() != null)
                            {
                                addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 12 && a.PeriodType == "A").FirstOrDefault();
                                addData.EstimateId = 11;
                                addData.Description = "Earnings Per Share (Pre Exceptional)";
                                localMedianData.Add(addData);
                            }
                        }
                    }
                }

                else if (periodType.Equals(FinancialStatementPeriodType.QUARTERLY))
                {
                    foreach (int item in periodYear)
                    {
                        foreach (string quarterValue in quartersList)
                        {
                            if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 11 && a.PeriodType == quarterValue).FirstOrDefault() == null)
                            {
                                if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 13 && a.PeriodType == quarterValue).FirstOrDefault() != null)
                                {
                                    addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 13 && a.PeriodType == quarterValue).FirstOrDefault();
                                    addData.EstimateId = 11;
                                    addData.Description = "Earnings Per Share (Pre Exceptional)";
                                    localMedianData.Add(addData);
                                }

                                if (medianData.Where(a => a.PeriodYear == item && a.EstimateId == 12 && a.PeriodType == quarterValue).FirstOrDefault() != null)
                                {
                                    addData = medianData.Where(a => a.PeriodYear == item && a.EstimateId == 12 && a.PeriodType == quarterValue).FirstOrDefault();
                                    addData.EstimateId = 11;
                                    addData.Description = "Earnings Per Share (Pre Exceptional)";
                                    localMedianData.Add(addData);
                                }
                            }
                        }
                    }
                }
                                
                List<ConsensusEstimateMedian> result = new List<ConsensusEstimateMedian>(localMedianData.Where(a => a.EstimateId != 12).ToList());
                result = new List<ConsensusEstimateMedian>(result.Where(a => a.EstimateId != 13).ToList());

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