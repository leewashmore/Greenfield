using System;
using Greenfield.Gadgets.Models;

namespace Greenfield.Gadgets.Helpers
{
    /// <summary>
    /// calculating DCF values
    /// </summary>
    public static class DCFCalculations
    {
        /// <summary>
        /// Calculating CostOfEquity
        /// </summary>
        /// <param name="marketRiskPrem">Market Risk Premium</param>
        /// <param name="beta">Beta</param>
        /// <param name="riskFreeRate">Risk Free Rate</param>
        /// <param name="stockSpecificDiscount">Stock Specific Discount</param>
        /// <returns>Cost of Equity</returns>
        internal static decimal CalculateCostOfEquity(decimal marketRiskPrem, decimal beta, decimal riskFreeRate
            , decimal stockSpecificDiscount)
        {
            decimal costofEquity;

            costofEquity = riskFreeRate + (beta * marketRiskPrem) + stockSpecificDiscount;

            return costofEquity;
        }

        /// <summary>
        /// Calculate Weight of Equity
        /// </summary>
        /// <param name="marketCap">Market Cap</param>
        /// <param name="grossDebt">Gross Debt</param>
        /// <returns>Weight of Equity</returns>
        internal static decimal CalculateWeightofEquity(decimal marketCap, decimal grossDebt)
        {
            decimal weightOfEquity;

            decimal sum = marketCap + grossDebt;

            if (sum != 0)
            {
                weightOfEquity = marketCap / (marketCap + grossDebt);
            }
            else
            {
                return 0;
            }

            return weightOfEquity;
        }

        /// <summary>
        /// Calculate WACC
        /// </summary>
        /// <param name="weightOfEquity">Weight of Equity</param>
        /// <param name="costOfEquity">Cost of Equity</param>
        /// <param name="costOfDebt">Cost of Debt</param>
        /// <param name="marginalTaxrate">marginal tax Rate</param>
        /// <returns></returns>
        internal static decimal CalculateWACC(decimal weightOfEquity, decimal costOfEquity, decimal costOfDebt,
            decimal marginalTaxrate)
        {
            decimal wacc;

            wacc = (weightOfEquity * costOfEquity) + ((1 - weightOfEquity) * (costOfDebt * (1 - marginalTaxrate)));

            return wacc;
        }

        /// <summary>
        /// Calculate Terminal Growth rate
        /// </summary>
        /// <param name="sustainableROIC">Sustainable ROIC</param>
        /// <param name="sustainableDivPayoutRatio">SDPR</param>
        /// <param name="longTermNominalGDPGrowth">long Term Nominal GDP Growth</param>
        /// <returns>Terminal Growth Rate</returns>
        internal static decimal CalculateTerminalGrowthRate(decimal sustainableROIC, decimal sustainableDivPayoutRatio,
            decimal longTermNominalGDPGrowth)
        {
            decimal terminalGrowthRate;

            decimal value = sustainableROIC * (1 - sustainableDivPayoutRatio);

            if (value < longTermNominalGDPGrowth)
            {
                terminalGrowthRate = value;
            }
            else
            {
                terminalGrowthRate = longTermNominalGDPGrowth;
            }

            return terminalGrowthRate;
        }

        /// <summary>
        /// Calculate Nominal terminal value
        /// </summary>
        /// <param name="wacc">WACC</param>
        /// <param name="terminalGrowthRate">TGR</param>
        /// <param name="cashFlow">CashFlow</param>
        /// <returns>Nominal Terminal Value</returns>
        internal static decimal CalculateNominalTerminalValue(decimal wacc, decimal terminalGrowthRate, decimal cashFlow)
        {
            decimal nominalTerminalValue = 0;

            if ((wacc - terminalGrowthRate) != 0)
            {
                nominalTerminalValue = (cashFlow * (1 + terminalGrowthRate)) / (wacc - terminalGrowthRate);
            }

            return nominalTerminalValue;
        }

        /// <summary>
        /// calculate Present Terminal Value
        /// </summary>
        /// <param name="nominalTerminalVal">Nominal Terminal value</param>
        /// <param name="discountingFactor">Disounting Factor</param>
        /// <returns>Present Terminal Value</returns>
        internal static decimal CalculatePresentTerminalValue(decimal nominalTerminalVal, decimal discountingFactor)
        {
            decimal presentTerminalVal = 0;

            if (discountingFactor == 0)
            {
                return 0;
            }

            if (discountingFactor != 0)
            {
                presentTerminalVal = nominalTerminalVal / discountingFactor;
            }

            return presentTerminalVal;
        }

        /// <summary>
        /// calculate Total Enterprise Value
        /// </summary>
        /// <param name="presentValueOfForecast">Present Value Explicit Forecast</param>
        /// <param name="presentTerminalVal">Present Terminal Value</param>
        /// <returns>Total Enterprise Value</returns>
        internal static decimal CalculateTotalEnterpriseValue(decimal presentValueOfForecast, decimal presentTerminalVal)
        {
            decimal totalEnterpriseVal = presentValueOfForecast + presentTerminalVal;

            return totalEnterpriseVal;
        }

        /// <summary>
        /// Calculate Equity Value
        /// </summary>
        /// <param name="totalEnterpriseVal">Total Enterprise Value</param>
        /// <param name="cash">Cash</param>
        /// <param name="fvInvestments">FVInvestment</param>
        /// <param name="grossDebt">GrossDebt</param>
        /// <param name="fvMinorities">FVMinorities</param>
        /// <returns>Equity Value</returns>
        internal static decimal CalculateEquityValue(decimal totalEnterpriseVal, decimal cash, decimal fvInvestments,
            decimal grossDebt, decimal fvMinorities)
        {
            decimal equityValue = totalEnterpriseVal + cash + fvInvestments - grossDebt - fvMinorities;

            return equityValue;
        }

        /// <summary>
        /// Calculate DCF Value Per Share
        /// </summary>
        /// <param name="equityvalue">Equity value</param>
        /// <param name="numOfShares">Number of Shares</param>
        /// <returns>DCF Value Per Share</returns>
        internal static decimal CalculateDCFValuePerShare(decimal equityvalue, decimal numOfShares)
        {
            decimal dcfValuePerShare = 0;

            if (numOfShares != 0)
            {
                dcfValuePerShare = equityvalue / numOfShares;
            }
            else
            {
                return 0;
            }

            return dcfValuePerShare;
        }

        /// <summary>
        /// calculate UPside Value
        /// </summary>
        /// <param name="dcfValuePerShare">DCF Value Per Share</param>
        /// <param name="currentMarketPrice">Current Market Price</param>
        /// <returns>Upside Value</returns>
        internal static decimal CalculateUpsideValue(decimal dcfValuePerShare, decimal currentMarketPrice)
        {
            decimal upside = 0;

            if (currentMarketPrice != 0)
            {
                upside = (dcfValuePerShare / currentMarketPrice) - 1;
            }
            else
            {
                return 0;
            }

            return upside;
        }

        /// <summary>
        /// Calculates TGR & DCF value Per share
        /// </summary>
        /// <param name="inputParameter">object of type DCFCalculationParameters</param>
        /// <returns>object of type DCFValueL: TGR & DCF value per share</returns>
        internal static DCFValue CalculateDCFValue(DCFCalculationParameters inputParameter)
        {
            DCFValue calculatedValue = new DCFValue();

            if (inputParameter == null)
            {
                throw new Exception("DCFCalculationParameters is null");
            }

            decimal weightOfEquity = CalculateWeightofEquity(inputParameter.MarketCap, inputParameter.GrossDebtA);

            decimal wacc = CalculateWACC(weightOfEquity, inputParameter.CostOfEquity,
                inputParameter.CostOfDebt, inputParameter.MarginalTaxRate);

            decimal nominalTerminalValue = CalculateNominalTerminalValue(wacc, inputParameter.TerminalGrowthRate,
                inputParameter.Year9CashFlow);

            decimal presentTerminalValue = CalculatePresentTerminalValue(nominalTerminalValue,
                inputParameter.Year10DiscountingFactor);

            decimal totalEnterpriseVal = CalculateTotalEnterpriseValue(inputParameter.PresentValueOfForecast,
                presentTerminalValue);

            decimal equityValue = CalculateEquityValue(totalEnterpriseVal, inputParameter.Cash,
                inputParameter.FutureValueOfInvestments, inputParameter.GrossDebt, inputParameter.FutureValueOfMinorties);

            decimal dcfValuePerShare = CalculateDCFValuePerShare(equityValue, inputParameter.NumberOfShares);

            decimal upsideValue = CalculateUpsideValue(dcfValuePerShare, inputParameter.CurrentMarketPrice);

            calculatedValue.DCFValuePerShare = dcfValuePerShare;
            calculatedValue.UpsideValue = upsideValue;

            return calculatedValue;
        }
    }
}