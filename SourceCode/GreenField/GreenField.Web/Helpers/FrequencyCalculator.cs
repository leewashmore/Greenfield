using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Class to Calculate Dates to Plot the chart according to the Interval Selected
    /// </summary>
    public static class FrequencyCalculator
    {
        /// <summary>
        /// Method to calculate Dates on which the chart should be plotted 
        /// </summary>
        /// <param name="objEndDates">The dates for which data is present</param>
        /// <param name="startDate">Start Date for the Chart</param>
        /// <param name="endDate">End Date for the Chart</param>
        /// <param name="FrequencyInterval">Selected Frequency Interval</param>
        /// <returns>List of DateTime</returns>
        public static List<DateTime> RetrieveDatesAccordingToFrequency(List<DateTime> objEndDates, DateTime startDate, DateTime endDate, string FrequencyInterval)
        {
            try
            {
                //List<PricingReferenceData> resultFrequency = new List<PricingReferenceData>();
                List<DateTime> EndDates = new List<DateTime>();
                DateTime chartStartDate = startDate;
                DateTime chartEndDate = endDate;
                TimeSpan timeSpan = chartEndDate - chartStartDate;

                switch (FrequencyInterval)
                {
                    case ("Weekly"):
                        {
                            #region CalculateWeeksBetweenDates

                            int totalWeeks = timeSpan.Days / 7;

                            #endregion

                            #region calculating LastDayOfAllWeeks

                            DateTime endDay = (chartStartDate.AddDays(5 - (int)chartStartDate.DayOfWeek));
                            GetEndDatesForEachWeek(endDay, totalWeeks, ref EndDates);
                            #endregion
                            break;
                        }

                    case ("Monthly"):
                        {
                            #region CalculateMonthsBetweenDates

                            int totalMonths = ((chartEndDate.Year - chartStartDate.Year) * 12) + chartEndDate.Month - chartStartDate.Month;
                            int totalYear = (chartEndDate.Year - chartStartDate.Year);

                            #endregion

                            #region calculating LastDayOfAllMonths

                            int month = chartStartDate.Month;
                            int year = chartStartDate.Year;

                            int monthsLeftInCurrentYear = 12 - month;

                            for (int i = 0; i <= monthsLeftInCurrentYear; i++)
                            {
                                GetEndDatesForEachMonth(ref year, ref month, ref EndDates);
                            }

                            for (int i = 0; i < totalYear - 1; i++)
                            {
                                month = 1;
                                year++;

                                while (month <= 12)
                                {
                                    GetEndDatesForEachMonth(ref year, ref month, ref EndDates);
                                }
                            }

                            int totalMonthsLeft = totalMonths - monthsLeftInCurrentYear - 12 * (totalYear - 1);

                            if (totalMonthsLeft > 0)
                            {
                                for (int i = 0; i < 1; i++)
                                {
                                    year++;
                                    month = 1;
                                    while (month <= totalMonthsLeft)
                                    {
                                        GetEndDatesForEachMonth(ref year, ref month, ref EndDates);
                                    }

                                }
                            }

                            #endregion
                            break;
                        }

                    case ("Quarterly"):
                        {
                            int startDateQuarter = GetQuarter(startDate.Month);
                            DateTime lastDate = startDate;

                            #region CalculateQuartersBetweenDates

                            int totalMonths = ((chartEndDate.Year - chartStartDate.Year) * 12) + chartEndDate.Month - chartStartDate.Month;

                            #endregion

                            #region CalculatingQuarters

                            switch (startDateQuarter)
                            {
                                case (1):
                                    {
                                        lastDate = new DateTime(chartStartDate.Year, 3, 31);
                                        GetEndDatesForEachQuarter(lastDate, chartEndDate, ref EndDates);
                                        break;
                                    }
                                case (2):
                                    {
                                        lastDate = new DateTime(chartStartDate.Year, 6, 30);
                                        GetEndDatesForEachQuarter(lastDate, chartEndDate, ref EndDates);
                                        break;
                                    }
                                case (3):
                                    {
                                        lastDate = new DateTime(chartStartDate.Year, 9, 30);
                                        GetEndDatesForEachQuarter(lastDate, chartEndDate, ref EndDates);
                                        break;
                                    }

                                case (4):
                                    {
                                        lastDate = new DateTime(chartStartDate.Year, 12, 31);
                                        GetEndDatesForEachQuarter(lastDate, chartEndDate, ref EndDates);
                                        break;
                                    }
                            }

                            #endregion

                            break;
                        }

                    case ("Half-Yearly"):
                        {
                            int startDateSemiAnnually = GetHalfYearly(startDate.Month);
                            DateTime lastDate = startDate;

                            switch (startDateSemiAnnually)
                            {
                                case (1):
                                    {
                                        lastDate = new DateTime(chartStartDate.Year, 6, 30);
                                        GetEndDatesForEachHalfYear(lastDate, chartEndDate, ref EndDates);
                                        break;
                                    }

                                case (2):
                                    {
                                        lastDate = new DateTime(chartStartDate.Year, 12, 31);
                                        GetEndDatesForEachHalfYear(lastDate, chartEndDate, ref EndDates);
                                        break;
                                    }
                            }

                            break;
                        }

                    case ("Yearly"):
                        {
                            int totalYearBetweenDates = chartEndDate.Year - chartStartDate.Year;
                            DateTime lastDate = new DateTime(chartStartDate.Year, 12, 31);
                            GetEndDatesForEachYear(lastDate, chartEndDate, ref EndDates);
                            break;
                        }

                    default:
                        {
                            return objEndDates;
                        }
                }


                return EndDates;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #region HelperMethods

        /// <summary>
        /// This method calculates the quarter in which a month falls.
        /// </summary>
        /// <param name="nMonth"></param>
        /// <returns>Quarter</returns>
        private static int GetQuarter(int nMonth)
        {
            if (nMonth <= 3)
                return 1;
            if (nMonth <= 6)
                return 2;
            if (nMonth <= 9)
                return 3;
            return 4;
        }

        /// <summary>
        ///  This method calculates the Half Year in which a month falls   
        /// </summary>
        /// <param name="nMonth"></param>
        /// <returns>Half Year</returns> 
        private static int GetHalfYearly(int nMonth)
        {
            if (nMonth <= 7)
                return 1;
            else if (nMonth <= 12)
                return 2;
            return 0;
        }

        /// <summary>
        /// This method calculates the end dates for each week.
        /// </summary>
        /// <param name="endDay"></param>
        /// <param name="totalWeeks"></param>
        /// <param name="EndDates">A list of datetime that stores the enddates.</param>
        private static void GetEndDatesForEachWeek(DateTime endDay, int totalWeeks, ref List<DateTime> EndDates)
        {

            for (int i = 0; i <= totalWeeks; i++)
            {
                DateTime lastDaysAllWeek = endDay.AddDays(7 * i);
                EndDates.Add(lastDaysAllWeek);
            }
        }

        /// <summary>
        /// This method calculates the end dates for each quarter.
        /// </summary>
        /// <param name="lastDate"></param>
        /// <param name="chartEndDate"></param>
        /// <param name="EndDates">A list of datetime that stores the enddates.</param>
        private static void GetEndDatesForEachQuarter(DateTime lastDate, DateTime chartEndDate, ref List<DateTime> EndDates)
        {

            while (lastDate <= chartEndDate)
            {
                EndDates.Add(lastDate);
                if (lastDate.Month == 3 || lastDate.Month == 6)
                    lastDate = lastDate.AddMonths(3);
                else if (lastDate.Month == 12)
                {
                    int year = lastDate.Year;
                    lastDate = new DateTime(year + 1, 3, 31);
                }
                else
                {
                    int year = lastDate.Year;
                    lastDate = new DateTime(year, 12, 31);
                }
            }

        }

        /// <summary>
        /// This method calculates the end dates for each half year.
        /// </summary>
        /// <param name="lastDate"></param>
        /// <param name="chartEndDate"></param>
        /// <param name="EndDates">A list of datetime that stores the enddates.</param>
        private static void GetEndDatesForEachHalfYear(DateTime lastDate, DateTime chartEndDate, ref List<DateTime> EndDates)
        {
            while (lastDate <= chartEndDate)
            {
                EndDates.Add(lastDate);
                lastDate = lastDate.AddMonths(6);
            }
        }

        /// <summary>
        /// This method calculates the end dates for each year.
        /// </summary>
        /// <param name="lastDate"></param>
        /// <param name="chartEndDate"></param>
        /// <param name="EndDates">A list of datetime that stores the enddates.</param>
        private static void GetEndDatesForEachYear(DateTime lastDate, DateTime chartEndDate, ref List<DateTime> EndDates)
        {

            while (lastDate <= chartEndDate)
            {
                EndDates.Add(lastDate);
                lastDate = lastDate.AddYears(1);
            }
        }

        /// <summary>
        /// This method calculates the end dates for each month
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="EndDates">A list of datetime that stores the enddates.</param>
        private static void GetEndDatesForEachMonth(ref int year, ref int month, ref List<DateTime> EndDates)
        {
            int numberOfDays = DateTime.DaysInMonth(year, month);
            DateTime lastDay = new DateTime(year, month, numberOfDays);
            EndDates.Add(lastDay);
            month++;
        }

        #endregion
    }
}