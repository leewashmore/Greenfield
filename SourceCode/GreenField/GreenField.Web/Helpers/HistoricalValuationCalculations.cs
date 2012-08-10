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
            if (count > 0)
            {
                avg = sumPRevenue / count;

                for (int _index = 0; _index < pRevenueData.Count; _index++)
                {
                    pRevenueData[_index].Average = avg;
                }
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
            if (count > 1)
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

                for (int _index = 0; _index < pRevenueData.Count; _index++)
                {
                    if (pRevenueData[_index].Average != null && stdDev != null)
                    {
                        pRevenueData[_index].StdDevPlus = pRevenueData[_index].Average + stdDev;
                        pRevenueData[_index].StdDevMinus = pRevenueData[_index].Average - stdDev;
                    }
                }
            }
            return pRevenueData;
        }
    }
}