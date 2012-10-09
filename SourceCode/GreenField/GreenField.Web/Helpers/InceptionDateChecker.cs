using System;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Contains Method to Verify that Sinception Date is
    /// greater than selected period
    /// </summary>
    public static class InceptionDateChecker
    {

        /// <summary>
        /// Get First Day of Current Month
        /// </summary>
        /// <returns></returns>
        private static DateTime GetFirstDayOfCurrentMonth(DateTime selectedDate)
        {
            DateTime firstDay;

            DateTime currentDate = selectedDate;
            firstDay = currentDate.AddDays(-(currentDate.Day - 1));

            return firstDay;
        }

        /// <summary>
        /// Get First Day of  Current Quarter
        /// </summary>
        /// <returns></returns>
        private static DateTime GetFirstDayOfCurrentQuarter(DateTime selectedDate)
        {
            DateTime firstDay;
            DateTime currentDate = selectedDate;
            int quarterNumber = (currentDate.Month - 1) / 3 + 1;
            firstDay = new DateTime(currentDate.Year, (quarterNumber - 1) * 3 + 1, 1);
            return firstDay;
        }

        /// <summary>
        /// Get Date First Day of Current Year
        /// </summary>
        /// <returns></returns>
        private static DateTime GetFirstDayOfCurrentYear(DateTime selectedDate)
        {
            DateTime firstDay;

            DateTime currentDate = selectedDate;
            firstDay = new DateTime(currentDate.Year, 1, 1);

            return firstDay;
        }

        /// <summary>
        /// Get Same Day of Last Year
        /// </summary>
        /// <returns></returns>
        private static DateTime GetSameDayOfLastYear(DateTime selectedDate)
        {
            DateTime firstDay;

            DateTime currentDate = selectedDate;
            firstDay = currentDate.AddYears(-1).AddDays(1);

            return firstDay;
        }

        /// <summary>
        /// Get Same Day 3 years ago
        /// </summary>
        /// <returns></returns>
        private static DateTime GetSameDayOfPreviousThirdYear(DateTime selectedDate)
        {
            DateTime firstDay;

            DateTime currentDate = selectedDate;
            firstDay = currentDate.AddYears(-3).AddDays(1);

            return firstDay;
        }

        /// <summary>
        /// Get Same Day 5 years ago
        /// </summary>
        /// <returns></returns>
        private static DateTime GetSameDayOfPreviousFifthYear(DateTime selectedDate)
        {
            DateTime firstDay;

            DateTime currentDate = selectedDate;
            firstDay = currentDate.AddYears(-5).AddDays(1);

            return firstDay;
        }

        /// <summary>
        /// Get Same Day 10 years ago
        /// </summary>
        /// <returns></returns>
        private static DateTime GetSameDayOfPreviousTenthYear(DateTime selectedDate)
        {
            DateTime firstDay;

            DateTime currentDate = selectedDate;
            firstDay = currentDate.AddYears(-10).AddDays(1);

            return firstDay;
        }

        /// <summary>
        /// Get PerfDataBeg Date
        /// </summary>
        /// <param name="inceptionDate"></param>
        /// <returns></returns>
        private static DateTime GetPerfDatabegDate(DateTime inceptionDate)
        {
            DateTime perfDataBegDate;

            DateTime dateToCompareAgainst = new DateTime(2012, 1, 1);
            if (inceptionDate > dateToCompareAgainst)
            {
                perfDataBegDate = inceptionDate;
            }
            else
            {
                perfDataBegDate = dateToCompareAgainst;
            }

            return perfDataBegDate;
        }

        /// <summary>
        /// Check if portflio inception date is less than period start date
        /// </summary>
        /// <param name="startDate">Start Date of the Period</param>
        /// <param name="perfDataBegDate">Performance Data Beg Date</param>
        /// <returns></returns>
        private static bool CheckIfPerDataBegDateIsGreaterThanStartDate(DateTime startDate, DateTime perfDataBegDate)
        {
            bool isGreater = true;

            if (startDate < perfDataBegDate)
            {
                isGreater = false;
            }

            return isGreater;
        }

        /// <summary>
        /// Chekc if Inception is greater than period start date
        /// </summary>
        /// <param name="periodType">period type</param>
        /// <param name="inceptionDate">inception date</param>
        /// <param name="selectedDate">selected date</param>
        /// <returns></returns>
        public static bool ValidateInceptionDate(string periodType, DateTime inceptionDate, DateTime selectedDate)
        {
            bool isValid = false;
            DateTime startDate;
            DateTime perfDataBegDate;

            switch (periodType.ToUpper())
            {
                case "MTD":
                    startDate = GetFirstDayOfCurrentMonth(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "QTD":
                    startDate = GetFirstDayOfCurrentQuarter(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "YTD":
                    startDate = GetFirstDayOfCurrentYear(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "1Y":
                    startDate = GetSameDayOfLastYear(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "3Y":
                    startDate = GetSameDayOfPreviousThirdYear(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "5Y":
                    startDate = GetSameDayOfPreviousFifthYear(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "10Y":
                    startDate = GetSameDayOfPreviousTenthYear(selectedDate);
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                case "SI":
                    startDate = inceptionDate;
                    perfDataBegDate = GetPerfDatabegDate(inceptionDate);
                    isValid = CheckIfPerDataBegDateIsGreaterThanStartDate(startDate, perfDataBegDate);
                    break;
                default:
                    break;
            }

            return isValid;
        }
    }
}