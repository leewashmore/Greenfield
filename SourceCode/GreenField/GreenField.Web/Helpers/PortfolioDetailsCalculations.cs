using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations for Portfolio Details UI
    /// </summary>
    public static class PortfolioDetailsCalculations
    {
        public static List<PortfolioDetailsData> CalculatePortfolioDetails(List<PortfolioDetailsData> portfolioDetailsData)
        {
            if (portfolioDetailsData == null)
                throw new InvalidOperationException();
            if (portfolioDetailsData.Count == 0)
                return new List<PortfolioDetailsData>();

            decimal? sumDirtyValuePC = 0;
            decimal? sumModelWeight = 0;

            sumDirtyValuePC = portfolioDetailsData.Sum(a => a.DirtyValuePC);
            sumModelWeight = portfolioDetailsData.Sum(a => a.AshEmmModelWeight);

            //Removed for DEmo
            //if (sumModelWeight == 0 || sumDirtyValuePC == 0)
            //    return new List<PortfolioDetailsData>();

            foreach (PortfolioDetailsData item in portfolioDetailsData)
            {
                item.PortfolioWeight = item.DirtyValuePC / sumDirtyValuePC;
                item.RePortfolioWeight = item.PortfolioWeight;
                item.ReBenchmarkWeight = item.BenchmarkWeight;
                item.ReAshEmmModelWeight = item.AshEmmModelWeight;
                item.ActivePosition = item.AshEmmModelWeight - item.BenchmarkWeight;
                //item.AshEmmModelWeight = item.AshEmmModelWeight / sumModelWeight;
            }
            return portfolioDetailsData;
        }
    }
}