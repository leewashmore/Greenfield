using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;


namespace GreenField.Web.Helpers
{
    public class FXCommodityCalculations
    {
        public static FXCommodityData CalculateCommodityData(List<FXCommodityData> commodityData)
        {
            FXCommodityData result = new FXCommodityData();

            decimal? GPToday = null;
            decimal? GPLastYearEnd = null;
            decimal? GP12MonthsAgo = null;
            decimal? GP36MonthsAgo = null;
            DateTime DateCurrentYear = DateTime.Now.Date;
            DateTime DateLastYearEnd = Convert.ToDateTime("12/31/" + (DateCurrentYear.Year - 1));
            DateTime Date12MonthsAgo = DateCurrentYear.AddYears(-1);
            DateTime Date36MonthsAgo = DateCurrentYear.AddYears(-3);

            foreach (FXCommodityData item in commodityData)
            {
                if (item.FromDate == DateCurrentYear)
                    GPToday = item.DailyClosingPrice;

                if (item.FromDate == DateLastYearEnd)
                    GPLastYearEnd = item.DailyClosingPrice;

                if (item.FromDate == Date12MonthsAgo)
                    GP12MonthsAgo = item.DailyClosingPrice;

                if (item.FromDate == Date36MonthsAgo)
                    GP36MonthsAgo = item.DailyClosingPrice;

            }
            if (GPToday != null && GPLastYearEnd != null)
                result.YTD = (GPToday - GPLastYearEnd) / GPLastYearEnd * 100;

            if (GPToday != null && GP12MonthsAgo != null)
                result.Year1 = (GPToday - GP12MonthsAgo) / GP12MonthsAgo * 100;

            if (GPToday != null && GP36MonthsAgo != null)
                result.Year3 = (GPToday - GP36MonthsAgo) / GP36MonthsAgo * 100;

            return result;
        }
    }
}