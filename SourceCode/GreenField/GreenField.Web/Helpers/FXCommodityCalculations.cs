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
            decimal? GPToday = null;
            decimal? GPLastYearEnd = null;
            decimal? GP12MonthsAgo = null;
            decimal? GP36MonthsAgo = null;
            DateTime CurrentDate = DateTime.Now.Date;
            DateTime Date1DayBack = Convert.ToDateTime(CurrentDate.Month + "/" + (CurrentDate.Day - 1) + "/" + CurrentDate.Year);
            DateTime DateLastYearEnd = Convert.ToDateTime("12/31/" + (Date1DayBack.Year - 1));
            DateTime Date12MonthsAgo = Convert.ToDateTime(Date1DayBack.Month + "/" + Date1DayBack.Day + "/" + (Date1DayBack.Year - 1));//CurrentDate.AddYears(-1);
            DateTime Date36MonthsAgo = Convert.ToDateTime(Date1DayBack.Month + "/" + Date1DayBack.Day + "/" + (Date1DayBack.Year - 3)); //CurrentDate.AddYears(-3);
            
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
                //result.InstrumentId = item.InstrumentId;
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