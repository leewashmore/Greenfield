using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Greenfield.Gadgets.Models
{
    public class DCFCalculationParameters
    {
        public decimal CostOfEquity { get; set; }

        public decimal MarginalTaxRate { get; set; }

        public decimal CostOfDebt { get; set; }

        public decimal MarketCap { get; set; }

        public decimal GrossDebt { get; set; }

        public decimal Year9CashFlow { get; set; }

        public decimal TerminalGrowthRate { get; set; }

        public decimal Year10DiscountingFactor { get; set; }

        public decimal PresentValueOfForecast { get; set; }

        public decimal Cash { get; set; }

        public decimal FutureValueOfInvestments { get; set; }

        public decimal FutureValueOfMinorties { get; set; }

        public decimal NumberOfShares { get; set; }

        public decimal CurrentMarketPrice { get; set; }

        public decimal GrossDebtA { get; set; }
    }
}
