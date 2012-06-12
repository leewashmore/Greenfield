﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DimensionEntitiesService;
using GreenField.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Calculations for Holdings -ChartExtension Gadget
    /// </summary>
    public static class ChartExtensionCalculations
    {
        /// <summary>
        /// Pricing Calculations for Security
        /// </summary>
        /// <param name="dimensionSecurityPricingData">Collection of type GF_PRICING_BASEVIEW</param>
        /// <param name="totalReturnCheck">Total/Gross Return</param>
        /// <returns>Collection of type ChartExtensionData</returns>
        public static List<ChartExtensionData> CalculateSecurityPricing(List<GF_PRICING_BASEVIEW> dimensionSecurityPricingData, bool totalReturnCheck = false)
        {
            try
            {
                List<ChartExtensionData> result = new List<ChartExtensionData>();

                if (dimensionSecurityPricingData == null)
                    throw new InvalidOperationException();

                decimal objAdjustedDollarPrice = 0;
                decimal objPreviousDailySpotFx = 0;
                decimal objPriceReturn = 0;
                decimal objReturn = 0;

                if (dimensionSecurityPricingData.Count != 0)
                {
                    foreach (DimensionEntitiesService.GF_PRICING_BASEVIEW pricingItem in dimensionSecurityPricingData)
                    {
                        if (pricingItem.DAILY_SPOT_FX == 0)
                            continue;

                        ChartExtensionData data = new ChartExtensionData();
                        data.Ticker = pricingItem.TICKER;
                        data.Type = "SECURITY";
                        data.ToDate = pricingItem.FROMDATE;
                        data.Price = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);
                        data.SortId = 1;

                        if (pricingItem.FROMDATE == dimensionSecurityPricingData[0].FROMDATE)
                        {
                            data.AdjustedDollarPrice = (Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE) / Convert.ToDecimal(pricingItem.DAILY_SPOT_FX));
                        }
                        else
                        {
                            data.AdjustedDollarPrice = objAdjustedDollarPrice / ((1 + (objReturn / 100)) * (Convert.ToDecimal(pricingItem.DAILY_SPOT_FX) / objPreviousDailySpotFx));
                        }

                        objAdjustedDollarPrice = data.AdjustedDollarPrice;
                        objPreviousDailySpotFx = Convert.ToDecimal(pricingItem.DAILY_SPOT_FX);
                        objReturn = ((totalReturnCheck) ? (Convert.ToDecimal(pricingItem.DAILY_GROSS_RETURN)) : (Convert.ToDecimal(pricingItem.DAILY_PRICE_RETURN)));
                        result.Add(data);
                    }

                    if ((result).ToList().Count() > 0)
                    {
                        result.OrderBy(r => r.ToDate).FirstOrDefault().PriceReturn = 100;
                    }

                    foreach (ChartExtensionData objChartExtensionData in result.OrderBy(r => r.ToDate).ToList())
                    {
                        if (objChartExtensionData.ToDate == result.OrderBy(r => r.ToDate).First().ToDate)
                        {
                            objAdjustedDollarPrice = objChartExtensionData.AdjustedDollarPrice;
                            objPriceReturn = objChartExtensionData.PriceReturn;
                        }
                        else
                        {
                            objChartExtensionData.PriceReturn = (objChartExtensionData.AdjustedDollarPrice / objAdjustedDollarPrice) * objPriceReturn;
                            objPriceReturn = objChartExtensionData.PriceReturn;
                            objAdjustedDollarPrice = objChartExtensionData.AdjustedDollarPrice;
                        }
                    }

                    foreach (ChartExtensionData item in result)
                    {
                        item.PriceReturn = item.PriceReturn - 100;
                    }

                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Performs calculations for Daily Transactions
        /// </summary>
        /// <param name="dimensionTransactionData">collection of Data of type GF_TRANSACTION returned from Dimension</param>
        /// <param name="securityExtensionData">Collection of type ChartExtensionData, containing calculated data for the security</param>
        /// <returns>Collection of ChartExtensionData</returns>
        public static List<ChartExtensionData> CalculateTransactionValues(List<GF_TRANSACTIONS> dimensionTransactionData, List<ChartExtensionData> securityExtensionData)
        {
            try
            {
                //Arguement null Exception
                if (dimensionTransactionData == null || securityExtensionData == null)
                    throw new InvalidOperationException();

                decimal sumBuyTransactions;
                decimal sumSellTransactions;
                decimal sumTotalTransaction;
                ChartExtensionData data = new ChartExtensionData();
                string securityLongName = securityExtensionData.Select(a => a.Ticker).First();

                List<DateTime> transactionDates = dimensionTransactionData.Select(a => a.TRADE_DATE).Distinct().ToList();

                foreach (DateTime tradeDate in transactionDates)
                {
                    sumBuyTransactions = dimensionTransactionData.Where(a => a.TRADE_DATE == tradeDate && a.TRANSACTION_CODE.ToUpper() == "BUY").Sum(a => Convert.ToDecimal(a.VALUE_PC));
                    sumSellTransactions = (-1) * dimensionTransactionData.Where(a => a.TRADE_DATE == tradeDate && a.TRANSACTION_CODE.ToUpper() == "SELL").Sum(a => Convert.ToDecimal(a.VALUE_PC));
                    sumTotalTransaction = sumBuyTransactions + sumSellTransactions;

                    if (securityExtensionData.Where(a => a.ToDate == tradeDate) != null)
                        securityExtensionData.Where(a => a.ToDate == tradeDate).First().AmountTraded = sumTotalTransaction;
                    else
                    {
                        data = new ChartExtensionData();
                        data.Ticker = securityLongName;
                        data.ToDate = tradeDate;
                        data.Type = "Transaction Only";
                        data.AmountTraded = sumTotalTransaction;
                        data.SortId = 2;
                        securityExtensionData.Add(data);
                    }
                }

                return securityExtensionData;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Performs calculations for Sector/Country Returns
        /// </summary>
        /// <param name="dimensionReturnData">Collection of type GF_PERF_DAILY_ATRRIBUTION retrieved from Dimension,contains Sector & Country level Data</param>
        /// <param name="securityTransactionExtensionData">Collection of type ChartExtension Data, contains Pricing data & Transaction Data</param>
        /// <returns>Collection of ChartExtensionData</returns>
        public static List<ChartExtensionData> CalculateSectorCountryReturnValues(List<GF_PERF_DAILY_ATTRIBUTION> dimensionReturnData)
        {
            List<ChartExtensionData> result = new List<ChartExtensionData>();
            ChartExtensionData data = new ChartExtensionData();
            //Arguement null Exception
            if (dimensionReturnData == null)
                throw new InvalidOperationException();


            foreach (GF_PERF_DAILY_ATTRIBUTION item in dimensionReturnData)
            {
                data = new ChartExtensionData();
                if (item.NODE_NAME.ToUpper().Trim() == "COUNTRY")
                {
                    data.Ticker = item.AGG_LVL_1_LONG_NAME;
                    data.Type = "COUNTRY";
                    data.SortId = 3;
                }
                else if (item.NODE_NAME.ToUpper().Trim() == "GICS LEVEL 5")
                {
                    data.Ticker = item.AGG_LVL_1_LONG_NAME;
                    data.Type = "SECTOR";
                    data.SortId = 4;
                }
                data.ToDate = item.TO_DATE;
                data.OneD = Convert.ToDecimal(item.POR_RC_AVG_WGT_1D);
                data.WTD = Convert.ToDecimal(item.POR_RC_AVG_WGT_1W);
                data.MTD = Convert.ToDecimal(item.POR_RC_AVG_WGT_MTD);
                data.QTD = Convert.ToDecimal(item.POR_RC_AVG_WGT_QTD);
                data.YTD = Convert.ToDecimal(item.POR_RC_AVG_WGT_YTD);
                data.OneY = Convert.ToDecimal(item.POR_RC_AVG_WGT_1Y);
                result.Add(data);
            }

            return result;
        }
    }

}