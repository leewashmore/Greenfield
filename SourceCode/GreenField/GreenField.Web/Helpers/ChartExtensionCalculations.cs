using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.DataContracts;

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
                        data.ToDate = pricingItem.FROMDATE;
                        data.Price = Convert.ToDecimal(pricingItem.DAILY_CLOSING_PRICE);

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

                List<DateTime> transactionDates = dimensionTransactionData.Select(a => a.TRADE_DATE).Distinct().ToList();

                foreach (DateTime tradeDate in transactionDates)
                {
                    sumBuyTransactions = dimensionTransactionData.Where(a => a.TRADE_DATE == tradeDate && a.TRANSACTION_CODE.ToUpper() == "BUY").Sum(a => Convert.ToDecimal(a.VALUE_PC));
                    sumSellTransactions = (-1) * dimensionTransactionData.Where(a => a.TRADE_DATE == tradeDate && a.TRANSACTION_CODE.ToUpper() == "SELL").Sum(a => Convert.ToDecimal(a.VALUE_PC));
                    sumTotalTransaction = sumBuyTransactions + sumSellTransactions;

                    if (securityExtensionData.Where(a => a.ToDate == tradeDate) != null )
                        securityExtensionData.Where(a => a.ToDate == tradeDate).First().AmountTraded = sumTotalTransaction;
                }

                return securityExtensionData;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
    }

}