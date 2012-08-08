using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.DAL;

namespace GreenField.Web.Helpers
{
    public class HistoricalValuationCalculations
    {

        //public static List<PRevenueData> CalculatePRevenue(List<GetPRevenueData_Result> pRevenueData)
        //{
        //    List<PRevenueData> result = new List<PRevenueData>();
        //    List<PRevenueData> quarterlyCalculatedData = new List<PRevenueData>();
        //    bool isQ1Exists = false;
        //    bool isQ2Exists = false;
        //    bool isQ3Exists = false;
        //    bool isQ4Exists = false;

        //    List<Int32> distinctYears = pRevenueData.OrderBy(p =>Convert.ToDecimal(p.Period_Year)).Select(p =>Convert.ToInt32( p.Period_Year)).Distinct().ToList();

        //    foreach (Int32 item1 in distinctYears)
        //    {
        //        List<PRevenueData> list = new List<PRevenueData>();
        //        foreach (GetPRevenueData_Result item2 in pRevenueData)
        //        {
        //            PRevenueData record = new PRevenueData();
        //            if (item1 == item2.Period_Year)
        //            {
        //                record.PeriodLabel = item2.PeriodLabel;
        //                record.Amount = item2.Amount;
        //                record.SharesOutstanding = item2.Shares_Outstanding;
        //                record.USDPrice = item2.USDPrice;
        //                record.PRevenueVal = 0.0M;
        //                result.Add(record);
        //                switch (item2.Period_Type)
        //                {
        //                    case "Q1":
        //                        isQ1Exists = true;
        //                        break;
        //                    case "Q2":
        //                        isQ2Exists = true;
        //                        break;
        //                    case "Q3":
        //                        isQ3Exists = true;
        //                        break;
        //                    case "Q4":
        //                        isQ4Exists = true;
        //                        break;
        //                    default:
        //                        break;
        //                }
        //            }
        //        }
        //        if (isQ1Exists && isQ2Exists && isQ3Exists && isQ4Exists)
        //            quarterlyCalculatedData = HistoricalValuationCalculations.CalculateQuarterlyPRevnue(list);
        //        result.AddRange(quarterlyCalculatedData);
        //        //foreach (PRevenueData item3 in quarterlyCalculatedData)
        //        //{
        //        //    PRevenueData data = new PRevenueData();
        //        //    data.PeriodLabel = item3.PeriodLabel;
        //        //    //data.Amount = item3.Amount;
        //        //    //data.SharesOutstanding = item3.SharesOutstanding;
        //        //    //data.USDPrice = item3.USDPrice;
        //        //    data.PRevenueVal = item3.PRevenueVal;
        //        //    result.Add(data);
        //        //}
        //        list = null;
        //        isQ1Exists = false;
        //        isQ2Exists = false;
        //        isQ3Exists = false;
        //        isQ4Exists = false;

                
        //    }

        //    return result;

        //}
        public static List<PRevenueData> CalculateQuarterlyPRevnue(List<PRevenueData> list)
        {
            List<PRevenueData> result = new List<PRevenueData>();
            decimal sum = list.Sum(p => Convert.ToDecimal(p.Amount));
            foreach (PRevenueData record in list)
            {
                record.PRevenueVal = (record.USDPrice * record.SharesOutstanding) / sum;
                break;
            }
            return result;

        }
        public static List<PRevenueData> CalculateAvg(List<PRevenueData> pRevenueData)
        {
            decimal? avg = null;
            decimal sumPRevenue = 0.0M;
            int count = 0;

            foreach (PRevenueData item in pRevenueData)
            {
                if (item.PRevenueVal != null)
                {
                    sumPRevenue = sumPRevenue + Convert.ToDecimal(item.PRevenueVal);
                    count = count + 1;
                }
            }
            if(sumPRevenue > 0 && count > 0)
                avg = sumPRevenue / count;

            for (int _index = 0; _index < pRevenueData.Count; _index++)
            {
                pRevenueData[_index].Average = avg;
            }
            return pRevenueData;
        }

        public static List<PRevenueData> CalculateStdDev(List<PRevenueData> pRevenueData)
        {
            decimal? stdDev = null;
            decimal sumPRevenue = 0.0M;
            decimal meanPRevenue = 0.0M;
            decimal sumPRevenueSqr = 0.0M;
            int count = 0;

            foreach (PRevenueData item in pRevenueData)
            {
                if (item.PRevenueVal != null)
                {
                    sumPRevenue = sumPRevenue + Convert.ToDecimal(item.PRevenueVal);
                    count = count + 1;
                }
            }
            if (sumPRevenue > 0 && count > 0)
            {
                meanPRevenue = sumPRevenue / count;

                foreach (PRevenueData item in pRevenueData)
                {
                    if (item.PRevenueVal != null)
                    {
                        sumPRevenueSqr = sumPRevenueSqr + (Convert.ToDecimal(item.PRevenueVal) - meanPRevenue) 
                                                            * (Convert.ToDecimal(item.PRevenueVal) - meanPRevenue);
                    }
                }
                stdDev = Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(sumPRevenueSqr / (count - 1))));
            }
            for (int _index = 0; _index < pRevenueData.Count; _index++)
            {
                if (pRevenueData[_index].Average != null && stdDev != null)
                {
                    pRevenueData[_index].StdDevPlus = pRevenueData[_index].Average + stdDev;
                    pRevenueData[_index].StdDevMinus = pRevenueData[_index].Average - stdDev;
                }
            }
            return pRevenueData;
        }
    }
}