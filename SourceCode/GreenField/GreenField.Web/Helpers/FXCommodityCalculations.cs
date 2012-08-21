using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;


namespace GreenField.Web.Helpers
{
    public class FXCommodityCalculations
    {
        /// <summary>
        /// To perform calculations on commodity data
        /// </summary>
        /// <param name="commodityData"></param>
        /// <returns></returns>
        public static FXCommodityData CalculateCommodityData(List<FXCommodityData> commodityData)
        {
            decimal? GPToday = null;
            decimal? GPLastYearEnd = null;
            decimal? GP12MonthsAgo = null;
            decimal? GP36MonthsAgo = null;

            DateTime CurrentDate = System.DateTime.Now;
            DateTime Date1DayBack = GetPreviousDate(CurrentDate);
            DateTime DateLastYearEnd = CurrentDate.AddYears(-1).AddMonths(-(CurrentDate.Month) + 12).AddDays(-(CurrentDate.Day) + 31);
            DateLastYearEnd = CheckBusinessDay(DateLastYearEnd);
            DateTime Date12MonthsAgo = CurrentDate.AddYears(-1);
            Date12MonthsAgo = CheckBusinessDay(Date12MonthsAgo);
            DateTime Date36MonthsAgo = CurrentDate.AddYears(-3);
            Date36MonthsAgo = CheckBusinessDay(Date36MonthsAgo);

            FXCommodityData result = new FXCommodityData();

            foreach (FXCommodityData item in commodityData)
            {
                if (item.FromDate == Date1DayBack)
                    GPToday = item.DailyClosingPrice;

                if (item.FromDate == DateLastYearEnd)
                    GPLastYearEnd = item.DailyClosingPrice;

                if (item.FromDate == Date12MonthsAgo)
                    GP12MonthsAgo = item.DailyClosingPrice;

                if (item.FromDate == Date36MonthsAgo)
                    GP36MonthsAgo = item.DailyClosingPrice;

                result.CommodityId = item.CommodityId;
            }

            if (GPToday != null && GPLastYearEnd != null)
                result.YTD = (GPToday - GPLastYearEnd) / GPLastYearEnd * 100;

            if (GPToday != null && GP12MonthsAgo != null)
                result.Year1 = (GPToday - GP12MonthsAgo) / GP12MonthsAgo * 100;

            if (GPToday != null && GP36MonthsAgo != null)
                result.Year3 = (GPToday - GP36MonthsAgo) / GP36MonthsAgo * 100;

            return result;
        }
        /// <summary>
        /// To Check business days
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime CheckBusinessDay(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    if (date.Day == 1)
                    {
                        date = GetPreviousDate(date);
                    }
                    else if (date.Day > 1)
                    {
                        date = date.AddDays(-1);
                    }
                    break;
                case DayOfWeek.Sunday:
                    if (date.Day < 2)
                    {
                        date = GetPreviousDate(date);
                    }
                    else if (date.Day > 2)
                    {
                        date = date.AddDays(-2);
                    }
                    break;
            }
            return date;
        }
        /// <summary>
        /// To get previouds date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static DateTime GetPreviousDate(DateTime date)
        {
            if (date.Day == 1)
            {
                if (date.Month == 1)
                {                   
                    date = date.AddYears(-1);
                    date = date.AddMonths(-(date.Month) + 12);
                    date = date.AddDays(-(date.Day) + DateTime.DaysInMonth(date.Year, date.Month));                   

                }
                else if (date.Month > 1)
                {
                    date = date.AddMonths(-1);
                    date = date.AddDays(-(date.Day) + DateTime.DaysInMonth(date.Year, date.Month));
                    
                }
            }
            else if (date.Day > 1)
            {
                date = date.AddDays(-1);
            }
            date = CheckBusinessDay(date);
            return date;
        }
    }
}