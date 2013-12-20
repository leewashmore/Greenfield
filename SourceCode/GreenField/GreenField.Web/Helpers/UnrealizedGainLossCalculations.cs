using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.DAL;

/// <summary>
/// Class contains methods that calculate the Unrealized Gain Loss of a security. 
/// </summary>
public static class UnrealizedGainLossCalculations
{
    /// <summary>
    /// Method that calculates the adjusted price for a selected security
    /// </summary>
    /// <param name="resultSetArrangedByDescRecord">The input list sorted in descending order of fromdate</param>
    /// <param name="noOfRows">No. of records in the arrangedByDescRecord list</param>
    /// <returns>adjustedPriceResult</returns>
    public static List<UnrealizedGainLossData> CalculateAdjustedPrice(List<GreenField.DAL.GF_PRICING_BASEVIEW> resultSetArrangedByDescRecord)
    {
        if (resultSetArrangedByDescRecord == null)
        {
            throw new ArgumentNullException("UnrealizedGainLossCalulcations:CalculateAdjustedPrice");
        }
        if (resultSetArrangedByDescRecord.Count < 90)
        {
            throw new ArgumentNullException("Insufficient Data");
        }
        List<UnrealizedGainLossData> adjustedPriceResult = new List<UnrealizedGainLossData>();

        decimal? previousAdjustedPrice;
        decimal? previousPriceReturn;

        UnrealizedGainLossData entry = new UnrealizedGainLossData();

        //calculations for the first record
        entry.AdjustedPrice = resultSetArrangedByDescRecord[0].DAILY_CLOSING_PRICE;
        entry.DailyClosingPrice = resultSetArrangedByDescRecord[0].DAILY_CLOSING_PRICE;
        entry.FromDate = (DateTime)resultSetArrangedByDescRecord[0].FROMDATE;
        entry.Volume = resultSetArrangedByDescRecord[0].VOLUME;
        entry.Ticker = resultSetArrangedByDescRecord[0].TICKER;
        entry.IssueName = resultSetArrangedByDescRecord[0].ISSUE_NAME;
        entry.Type = resultSetArrangedByDescRecord[0].TYPE;
        previousAdjustedPrice = entry.AdjustedPrice;
        previousPriceReturn = resultSetArrangedByDescRecord[0].DAILY_PRICE_RETURN;

        //return empty set if previousAdjustedPrice or previousPriceReturn null results
        if (previousAdjustedPrice == null || previousPriceReturn == null)
        {
            return adjustedPriceResult;
        }

        adjustedPriceResult.Add(entry);

        //calculations for the rest of the records
        for (int i = 1; i < resultSetArrangedByDescRecord.Count; i++)
        {
            entry = new UnrealizedGainLossData();
            decimal? adjustedPriceDenominator = 1 + (previousPriceReturn / 100);
            if (adjustedPriceDenominator.Equals(0))
            {
                throw new InvalidOperationException("Service returned invalid data. Operation could not be completed");
            }
            entry.AdjustedPrice = previousAdjustedPrice / adjustedPriceDenominator;
            previousPriceReturn = resultSetArrangedByDescRecord[i].DAILY_PRICE_RETURN;
            entry.DailyClosingPrice = resultSetArrangedByDescRecord[i].DAILY_CLOSING_PRICE;
            entry.FromDate = (DateTime)resultSetArrangedByDescRecord[i].FROMDATE;
            entry.Volume = resultSetArrangedByDescRecord[i].VOLUME;
            entry.Ticker = resultSetArrangedByDescRecord[i].TICKER;
            entry.IssueName = resultSetArrangedByDescRecord[i].ISSUE_NAME;
            entry.Type = resultSetArrangedByDescRecord[i].TYPE;
            adjustedPriceResult.Add(entry);
            previousAdjustedPrice = entry.AdjustedPrice;
        }
        return adjustedPriceResult;
    }

    /// <summary>
    /// Method that calculates the Moving Average of a selected security
    /// </summary>
    /// <param name="adjustedPriceResult">Contains the calculated Adjusted price of the security</param>
    /// <param name="noOfRows">No. of records</param>
    /// <returns>resultAscOrder</returns>
    public static List<UnrealizedGainLossData> CalculateMovingAverage(List<UnrealizedGainLossData> adjustedPriceResult)
    {
        if (adjustedPriceResult == null)
        {
            throw new ArgumentNullException();
        }
        if (adjustedPriceResult.Count < 90)
        {
            throw new ArgumentNullException("Insufficient Data");
        }
        List<UnrealizedGainLossData> resultAscOrder
            = adjustedPriceResult
            .OrderBy(res => res.FromDate)
            .ToList();

        decimal? totalPrice = resultAscOrder[0].AdjustedPrice;

        //calculations for the first record
        resultAscOrder[0].MovingAverage = resultAscOrder[0].AdjustedPrice;

        //calculations for the rest of the records
        for (int i = 0; i < adjustedPriceResult.Count - 1; i++)
        {
            totalPrice = totalPrice + resultAscOrder[i + 1].AdjustedPrice;
            resultAscOrder[i + 1].MovingAverage = totalPrice / (i + 2);
        }
        return resultAscOrder;
    }

    /// <summary>
    /// Method that calculates the Ninety Day Weight Average for a selected security
    /// </summary>
    /// <param name="movingAverageResult">Contains the Calculated Moving Average of the security</param>
    /// <param name="noOfRows">No. of records</param>
    /// <returns>movingAverageResult</returns>
    public static List<UnrealizedGainLossData> CalculateNinetyDayWtAvg(List<UnrealizedGainLossData> movingAverageResult)
    {
        if (movingAverageResult == null)
        {
            throw new ArgumentNullException();
        }
        if (movingAverageResult.Count < 90)
        {
            return new List<UnrealizedGainLossData>();
        }
        decimal? nintyDayVolumeSummation = 0;
        if (movingAverageResult.Count < 90)
        {
            throw new InvalidOperationException("Service returned incomplete data. Operation could not be completed");
        }
        for (int i = 0; i < 90; i++)
        {
            movingAverageResult[i].NinetyDayWtAvg = 0;
            nintyDayVolumeSummation = nintyDayVolumeSummation + movingAverageResult[i].Volume;
        }
        if (nintyDayVolumeSummation == 0)
        {
            for (int j = 90; j < movingAverageResult.Count; j++)
            {
                if (movingAverageResult[j].Volume != 0)
                {
                    nintyDayVolumeSummation = movingAverageResult[j].Volume;
                    movingAverageResult.RemoveRange(0, j - 90);
                    break;
                }                    
            }                
         }            
           movingAverageResult[89].NinetyDayWtAvg = nintyDayVolumeSummation;
           for (int i = 90; i < movingAverageResult.Count; i++)
           {
               nintyDayVolumeSummation = nintyDayVolumeSummation + movingAverageResult[i].Volume - movingAverageResult[i - 90].Volume;
               movingAverageResult[i].NinetyDayWtAvg = nintyDayVolumeSummation;
           }  
        return movingAverageResult;
    }

    /// <summary>
    /// Method that calculates the Cost for a selected security
    /// </summary>
    /// <param name="ninetyDayWTResult">Contains the Calculated Ninety Day Average Weight of the security</param>
    /// <param name="noOfRows">No. of records</param>
    /// <returns>ninetyDayWtResult</returns>
    public static List<UnrealizedGainLossData> CalculateCost(List<UnrealizedGainLossData> ninetyDayWTResult)
    {
        if (ninetyDayWTResult == null)
        {
            throw new ArgumentNullException();
        }
        if (ninetyDayWTResult.Count.Equals(0))
        {
            return new List<UnrealizedGainLossData>();
        }
        if (ninetyDayWTResult.Count < 90)
        {
            throw new InvalidOperationException("Service returned incomplete data. Operation could not be completed");
        }
        decimal? lastRecordNinetyDayWtAvg = ninetyDayWTResult[89].NinetyDayWtAvg;
        if (lastRecordNinetyDayWtAvg == null || lastRecordNinetyDayWtAvg == 0)
        {
            throw new InvalidOperationException("Service returned invalid data. Operation could not be completed");
        }
        for (int i = 0; i < 90; i++)
        {
            ninetyDayWTResult[i].Cost = (ninetyDayWTResult[i].AdjustedPrice * ninetyDayWTResult[i].Volume) / ninetyDayWTResult[89].NinetyDayWtAvg;
        }
        for (int i = 90; i < ninetyDayWTResult.Count; i++)
        {
            decimal? recordNinetyDayWtAvg = ninetyDayWTResult[i].NinetyDayWtAvg;
            if (recordNinetyDayWtAvg == null || recordNinetyDayWtAvg == 0)
            {
                throw new InvalidOperationException("Service returned invalid data. Operation could not be completed");
            }            
            ninetyDayWTResult[i].Cost = (ninetyDayWTResult[i].AdjustedPrice * ninetyDayWTResult[i].Volume) / ninetyDayWTResult[i].NinetyDayWtAvg;
        }
        return ninetyDayWTResult;
    }

    /// <summary>
    /// Method that calculates the Weight Avg Cost for a selected security
    /// </summary>
    /// <param name="costResult">Contains the Calculated Cost of the security</param>
    /// <param name="noOfRows">No. of records</param>
    /// <returns>costResult</returns>
    public static List<UnrealizedGainLossData> CalculateWtAvgCost(List<UnrealizedGainLossData> costResult)
    {
        if (costResult == null)
        {
            throw new ArgumentNullException();
        }
        if (costResult.Count.Equals(0))
        {
            return new List<UnrealizedGainLossData>();
        }
        if (costResult.Count < 90)
        {
            throw new InvalidOperationException("Service returned incomplete data. Operation could not be completed");
        }
        decimal? sumCost = 0;

        for (int i = 0; i < 90; i++)
        {
            costResult[i].WtAvgCost = 0;
            sumCost = sumCost + costResult[i].Cost;
        }
        if (sumCost == null || sumCost == 0)
        {
            for (int j = 90; j < costResult.Count; j++)
            {
                if (costResult[j].Cost != 0)
                {
                    sumCost = costResult[j].Cost;
                    costResult.RemoveRange(0, j - 90);
                    break;
                }
            }
        }

        costResult[89].WtAvgCost = sumCost;
        for (int i = 90; i < costResult.Count; i++)
        {
            sumCost = sumCost + costResult[i].Cost - costResult[i - 90].Cost;
            costResult[i].WtAvgCost = sumCost;
        }
        return costResult;
    }

    /// <summary>
    /// Method that calculates the Unrealized Gain Loss for a selected security
    /// </summary>
    /// <param name="wtAvgCostResult">Contains the Unrealized Gain Loss of the security</param>
    /// <param name="noOfRows">No. of records</param>
    /// <returns>wtAvgCostResult</returns>
    public static List<UnrealizedGainLossData> CalculateUnrealizedGainLoss(List<UnrealizedGainLossData> wtAvgCostResult)
    {
        if (wtAvgCostResult == null)
        {
            throw new ArgumentNullException();
        }
        if (wtAvgCostResult.Count.Equals(0))
        {
            return new List<UnrealizedGainLossData>();
        }
        if (wtAvgCostResult.Count < 90)
        {
            throw new InvalidOperationException("Service returned incomplete data. Operation could not be completed");
        }
        for (int i = 89; i < wtAvgCostResult.Count; i++)
        {
            decimal? recordWtAvgCost = wtAvgCostResult[i].WtAvgCost;
            if (recordWtAvgCost == null || recordWtAvgCost == 0)
            {
                throw new InvalidOperationException("Service returned invalid data. Operation could not be completed");
            }
            wtAvgCostResult[i].UnrealizedGainLoss = (wtAvgCostResult[i].AdjustedPrice / wtAvgCostResult[i].WtAvgCost) - 1;
        }
        return wtAvgCostResult;
    }

    /// <summary>
    /// Method to Retrieve Data Filtered according to Frequency.
    /// If for a particular day , data is not present then the data for 1-day before is considered.
    /// </summary>
    /// <param name="objUnrealizedGainLossData"></param>
    /// <param name="endDates"></param>
    /// <returns></returns>
    public static List<UnrealizedGainLossData> RetrieveUnrealizedGainLossData(List<UnrealizedGainLossData> objUnrealizedGainLossData, List<DateTime> endDates)
    {
        if (objUnrealizedGainLossData == null || endDates == null)
        {
            throw new ArgumentNullException();
        }
        if (objUnrealizedGainLossData.Count.Equals(0) || endDates.Count.Equals(0))
        {
            return new List<UnrealizedGainLossData>();
        }
        List<UnrealizedGainLossData> resultFrequency = new List<UnrealizedGainLossData>();
        List<DateTime> EndDates = endDates;
        foreach (DateTime item in EndDates)
        {
            int i = 1;
            bool dateObjectFound = true;

            if (objUnrealizedGainLossData.Any(r => r.FromDate.Date == item.Date))
            {
                resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate.Date == item.Date).First());
                dateObjectFound = false;
                continue;
            }
            else
            {
                dateObjectFound = true;
            }
            while (dateObjectFound)
            {
                bool objDataFoundDec = objUnrealizedGainLossData.Any(r => r.FromDate.Date == item.AddDays(-i).Date);
                if (objDataFoundDec)
                {
                    resultFrequency.Add(objUnrealizedGainLossData.Where(r => r.FromDate.Date == item.AddDays(-i).Date).First());
                    dateObjectFound = false;
                }
                else
                {
                    i++;
                    if (i > 30)
                    {
                        dateObjectFound = false;
                        continue;
                    }
                }
            }
        }
        return resultFrequency.Distinct().ToList();
    }
}