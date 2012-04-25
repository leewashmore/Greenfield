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

            //if (sumModelWeight == 0 || sumDirtyValuePC == 0)
            //    return new List<PortfolioDetailsData>();

            foreach (PortfolioDetailsData item in portfolioDetailsData)
            {
                item.PortfolioWeight = item.DirtyValuePC / sumDirtyValuePC;
                //item.AshEmmModelWeight = item.AshEmmModelWeight / sumModelWeight;
            }
            return portfolioDetailsData;
        }
    }
}