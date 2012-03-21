using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.Web.DataContracts;
using GreenField.DAL;

namespace GreenField.Web.Helpers
{
    public static class UnrealizedGainLossCalculations
    { 
        /// <summary>
        /// Method that calculates the adjusted price for a selected security
        /// </summary>
        /// <param name="arrangedByDescRecord"></param>
        /// <param name="noOfRows"></param>
        /// <returns>adjustedPriceResult</returns>
        public static List<UnrealizedGainLossData> CalculateAdjustedPrice(List<DimensionEntitiesService.GF_PRICING_BASEVIEW> arrangedByDescRecord, int noOfRows)
        {
            List<UnrealizedGainLossData> adjustedPriceResult = new List<UnrealizedGainLossData>();
            double previousAdjustedPrice;
            double previousPriceReturn;
            UnrealizedGainLossData entry = new UnrealizedGainLossData();
            //Calculations for the first record
            entry.AdjustedPrice = Convert.ToDouble(arrangedByDescRecord[0].DAILY_CLOSING_PRICE);
            previousAdjustedPrice = entry.AdjustedPrice;
            previousPriceReturn = Convert.ToDouble(arrangedByDescRecord[0].DAILY_PRICE_RETURN);
            entry.DailyClosingPrice = Convert.ToDecimal(arrangedByDescRecord[0].DAILY_CLOSING_PRICE);
            entry.FromDate = (DateTime)arrangedByDescRecord[0].FROMDATE;
            entry.Volume = Convert.ToDecimal(arrangedByDescRecord[0].VOLUME);
            adjustedPriceResult.Add(entry);

            //Calculations for the rest of the records
            for (int i = 1; i < noOfRows; i++)
            {
                entry = new UnrealizedGainLossData();
                entry.AdjustedPrice = previousAdjustedPrice / (1 + (previousPriceReturn / 100));
                previousAdjustedPrice = entry.AdjustedPrice;
                previousPriceReturn = Convert.ToDouble(arrangedByDescRecord[i].DAILY_PRICE_RETURN);
                entry.DailyClosingPrice = Convert.ToDecimal(arrangedByDescRecord[i].DAILY_CLOSING_PRICE);
                entry.FromDate = (DateTime)arrangedByDescRecord[i].FROMDATE;
                entry.Volume = Convert.ToDecimal(arrangedByDescRecord[i].VOLUME);
                adjustedPriceResult.Add(entry);
            }
            return adjustedPriceResult;
        }

        /// <summary>
        /// Method that calculates the Moving Average of a selected security
        /// </summary>
        /// <param name="adjustedPriceResult"></param>
        /// <param name="noOfRows"></param>
        /// <returns>resultAscOrder</returns>
        public static List<UnrealizedGainLossData> CalculateMovingAverage(List<UnrealizedGainLossData> adjustedPriceResult, int noOfRows)
        {

            int rowCount = 1;
            List<UnrealizedGainLossData> resultAscOrder = adjustedPriceResult.OrderBy(res => res.FromDate).ToList();
            double totalPrice = resultAscOrder[0].AdjustedPrice;

            //Calculations for the first record
            resultAscOrder[0].MovingAverage = resultAscOrder[0].AdjustedPrice;

            //Calculations for the rest of the records
            for (int i = 0; i < noOfRows - 1; i++)
            {
                totalPrice = totalPrice + resultAscOrder[i + 1].AdjustedPrice;
                rowCount = rowCount + 1;
                resultAscOrder[i + 1].MovingAverage = totalPrice / rowCount;
            }
            return resultAscOrder;
        }

        /// <summary>
        /// Method that calculates the Ninety Day Weight Average for a selected security
        /// </summary>
        /// <param name="movingAverageResult"></param>
        /// <param name="noOfRows"></param>
        /// <returns>movingAverageResult</returns>
        public static List<UnrealizedGainLossData> CalculateNinetyDayWtAvg(List<UnrealizedGainLossData> movingAverageResult, int noOfRows)
        {
            decimal currentSum = 0;
            for (int i = 0; i < 90; i++)
            {
                currentSum = currentSum + movingAverageResult[i].Volume;
            }

            movingAverageResult[89].NinetyDayWtAvg = Convert.ToDouble(currentSum);

            for (int i = 90; i < noOfRows; i++)
            {
                currentSum = currentSum + movingAverageResult[i].Volume - movingAverageResult[i - 90].Volume;
                movingAverageResult[i].NinetyDayWtAvg = Convert.ToDouble(currentSum);
            }

            return movingAverageResult;
        }

        /// <summary>
        /// Method that calculates the Cost for a selected security
        /// </summary>
        /// <param name="ninetyDayWtResult"></param>
        /// <param name="noOfRows"></param>
        /// <returns>ninetyDayWtResult</returns>
        public static List<UnrealizedGainLossData> CalculateCost(List<UnrealizedGainLossData> ninetyDayWtResult, int noOfRows)
        {
            for (int i = 0; i < 90; i++)
            {
                ninetyDayWtResult[i].Cost = (Convert.ToDecimal(ninetyDayWtResult[i].AdjustedPrice) * (ninetyDayWtResult[i].Volume)) / Convert.ToDecimal(ninetyDayWtResult[89].NinetyDayWtAvg);
            }
            for (int i = 90; i < noOfRows; i++)
            {
                ninetyDayWtResult[i].Cost = (Convert.ToDecimal(ninetyDayWtResult[i].AdjustedPrice) * (ninetyDayWtResult[i].Volume)) / Convert.ToDecimal(ninetyDayWtResult[i].NinetyDayWtAvg);
            }

            return ninetyDayWtResult;
        }

        /// <summary>
        /// Method that calculates the Weight Avg Cost for a selected security
        /// </summary>
        /// <param name="costResult"></param>
        /// <param name="noOfRows"></param>
        /// <returns>costResult</returns>
        public static List<UnrealizedGainLossData> CalculateWtAvgCost(List<UnrealizedGainLossData> costResult, int noOfRows)
        {
            decimal sumCost = 0;
            for (int i = 0; i < 90; i++)
            {
                sumCost = sumCost + costResult[i].Cost;
            }
            costResult[89].WtAvgCost = Convert.ToDouble(sumCost);
            for (int i = 90; i < noOfRows; i++)
            {
                sumCost = sumCost + costResult[i].Cost - costResult[i - 90].Cost;
                costResult[i].WtAvgCost = Convert.ToDouble(sumCost);
            }
            return costResult;
        }

        /// <summary>
        /// Method that calculates the Unrealized Gain Loss for a selected security
        /// </summary>
        /// <param name="wtAvgCostResult"></param>
        /// <param name="noOfRows"></param>
        /// <returns>wtAvgCostResult</returns>
        public static List<UnrealizedGainLossData> CalculateUnrealizedGainLoss(List<UnrealizedGainLossData> wtAvgCostResult, int noOfRows)
        {
            for (int i = 89; i < noOfRows; i++)
            {
                wtAvgCostResult[i].UnrealizedGainLoss = (wtAvgCostResult[i].AdjustedPrice / wtAvgCostResult[i].WtAvgCost) - 1;
            }
            return wtAvgCostResult;
        }
    }
}