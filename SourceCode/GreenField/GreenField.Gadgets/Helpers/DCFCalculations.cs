using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Greenfield.Gadgets.Models;

namespace Greenfield.Gadgets.Helpers
{
    public static class DCFCalculations
    {
        internal static decimal CalculateCostOfEquity(decimal marketRiskPrem, decimal beta, decimal riskFreeRate
            , decimal stockSpecificDiscount)
        {
            decimal costofEquity;

            costofEquity = riskFreeRate + (beta * marketRiskPrem) + stockSpecificDiscount;

            return costofEquity;
        }

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

        internal static decimal CalculateWACC(decimal weightOfEquity, decimal costOfEquity, decimal costOfDebt,
            decimal marginalTaxrate)
        {
            decimal wacc;

            wacc = (weightOfEquity * costOfEquity) + ((1 - weightOfEquity) * (costOfDebt * (1 - marginalTaxrate)));

            return wacc;
        }

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

        internal static decimal CalculateNominalTerminalValue(decimal wacc, decimal terminalGrowthRate, decimal cashFlow)
        {
            decimal nominalTerminalValue = 0;

            if ((wacc - terminalGrowthRate) != 0)
            {
                nominalTerminalValue = (cashFlow * (1 + terminalGrowthRate)) / (wacc - terminalGrowthRate);
            }

            return nominalTerminalValue;
        }

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

        internal static decimal CalculateTotalEnterpriseValue(decimal presentValueOfForecast, decimal presentTerminalVal)
        {
            decimal totalEnterpriseVal = presentValueOfForecast + presentTerminalVal;

            return totalEnterpriseVal;
        }

        internal static decimal CalculateEquityValue(decimal totalEnterpriseVal, decimal cash, decimal fvInvestments,
            decimal grossDebt, decimal fvMinorities)
        {
            decimal equityValue = totalEnterpriseVal + cash + fvInvestments - grossDebt - fvMinorities;

            return equityValue;
        }

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

        internal static DCFValue CalculateDCFValue(DCFCalculationParameters inputParameter)
        {
            DCFValue calculatedValue = new DCFValue();

            if (inputParameter == null)
            {
                throw new Exception("DCFCalculationParameters is null");
            }

            decimal weightOfEquity = CalculateWeightofEquity(inputParameter.MarketCap, inputParameter.GrossDebt);

            decimal wacc = CalculateWACC(weightOfEquity, inputParameter.CostOfEquity,
                inputParameter.CostOfDebt, inputParameter.MarginalTaxRate);

            decimal nominalTerminalValue = CalculateNominalTerminalValue(wacc, inputParameter.TerminalGrowthRate,
                inputParameter.Year10CashFlow);

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