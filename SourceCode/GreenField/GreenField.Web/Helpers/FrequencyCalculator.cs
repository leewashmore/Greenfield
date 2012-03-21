using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DataContracts;

namespace GreenField.Web.Helpers
{
    public static class FrequencyCalculator
    {
        public static List<DateTime> RetrieveDatesAccordingToFrequency(List<DateTime> objEndDates, DateTime startDate, DateTime endDate, string FrequencyInterval)
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
                        for (int i = 1; i <= totalWeeks; i++)
                        {
                            DateTime lastDaysAllWeek = endDay.AddDays(7 * i);
                            EndDates.Add(lastDaysAllWeek);
                        }

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
                            int numberOfDays = DateTime.DaysInMonth(year, month);
                            DateTime lastDay = new DateTime(year, month, numberOfDays);
                            EndDates.Add(lastDay);
                            month++;
                        }

                        for (int i = 0; i < totalYear - 1; i++)
                        {
                            month = 1;
                            year++;

                            while (month <= 12)
                            {
                                int numberOfDays = DateTime.DaysInMonth(year, month);
                                DateTime lastDay = new DateTime(year, month, numberOfDays);
                                EndDates.Add(lastDay);
                                month++;
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
                                    int numberOfDays = DateTime.DaysInMonth(year, month);
                                    DateTime lastDay = new DateTime(year, month, numberOfDays);
                                    EndDates.Add(lastDay);
                                    month++;
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
                                    while (lastDate <= chartEndDate)
                                    {
                                        EndDates.Add(lastDate);
                                        lastDate = lastDate.AddMonths(3);
                                    }
                                    break;
                                }
                            case (2):
                                {
                                    lastDate = new DateTime(chartStartDate.Year, 6, 30);
                                    while (lastDate <= chartEndDate)
                                    {
                                        EndDates.Add(lastDate);
                                        lastDate = lastDate.AddMonths(3);
                                    }
                                    break;
                                }
                            case (3):
                                {
                                    lastDate = new DateTime(chartStartDate.Year, 9, 31);
                                    while (lastDate <= chartEndDate)
                                    {
                                        EndDates.Add(lastDate);
                                        lastDate = lastDate.AddMonths(3);
                                    }
                                    break;
                                }

                            case (4):
                                {
                                    lastDate = new DateTime(chartStartDate.Year, 12, 31);
                                    while (lastDate <= chartEndDate)
                                    {
                                        EndDates.Add(lastDate);
                                        lastDate = lastDate.AddMonths(3);
                                    }
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
                                    while (lastDate <= chartEndDate)
                                    {
                                        EndDates.Add(lastDate);
                                        lastDate = lastDate.AddMonths(6);
                                    }
                                    break;
                                }

                            case (2):
                                {
                                    lastDate = new DateTime(chartStartDate.Year, 12, 31);
                                    while (lastDate <= chartEndDate)
                                    {
                                        EndDates.Add(lastDate);
                                        lastDate = lastDate.AddMonths(6);
                                    }
                                    break;
                                }
                        }

                        break;
                    }

                case ("Yearly"):
                    {
                        int totalYearBetweenDates = chartEndDate.Year - chartStartDate.Year;
                        DateTime lastDate = new DateTime(chartStartDate.Year, 12, 31);
                        while (lastDate <= chartEndDate)
                        {
                            EndDates.Add(lastDate);
                            lastDate.AddYears(1);
                        }
                        break;
                    }

                default:
                    {
                        return objEndDates;
                        break;
                    }
            }

            //#region CalculateListData

            //foreach (DateTime item in EndDates)
            //{
            //    int i = 1;
            //    bool dateObjectFound = true;

            //    if (objPricingData.Any(r => r.FromDate == item))
            //    {
            //        resultFrequency.Add(objPricingData.Where(r => r.FromDate == item).First());
            //        dateObjectFound = false;
            //        continue;
            //    }
            //    else
            //    {
            //        dateObjectFound = true;
            //    }

            //    while (dateObjectFound)
            //    {
            //        bool objDataFoundDec = objPricingData.Any(r => r.FromDate == item.AddDays(-i));
            //        if (objDataFoundDec)
            //        {
            //            resultFrequency.Add(objPricingData.Where(r => r.FromDate == item.AddDays(-i)).First());
            //            dateObjectFound = false;
            //        }
            //        else
            //        {
            //            i++;
            //        }
            //    }
            //}
            //#endregion

            return EndDates;
        }

        #region HelperMethods

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

        private static int GetHalfYearly(int nMonth)
        {
            if (nMonth <= 7)
                return 1;
            else if (nMonth <= 12)
                return 2;
            return 0;
        }

        #endregion
    }
}