using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GreenField.Gadgets.Helpers.PortfolioDetails
{
    public class ColumnHeaders
    {
        public string RevenueGrowthCurrentYear {
            get
            {
                return "Revenue Growth " + DateTime.Now.Year;
            }
        }

        public string RevenueGrowthNextYear
        {
            get
            {
                return "Revenue Growth " + (DateTime.Now.Year+1);
            }
        }

        public string NetIncomeCurrentYear
        {
            get
            {
                return "Net Income " + DateTime.Now.Year;
            }
        }

        public string NetIncomeNextYear
        {
            get
            {
                return "Net Income " + (DateTime.Now.Year+1);
            }
        }
        public string NetDebtEquityHeader
        {
            get
            {
                return "Net Debt Equity " + (DateTime.Now.Year)+" %";
            }
        }

        public string FreeCashFlowMarginHeader
        {
            get
            {
                return "Free Cash Flow Margin " + (DateTime.Now.Year) + " %";
            }
        }

        public string DividendYieldCurrentYear
        {
            get
            {
                return "Dividend Yield " + DateTime.Now.Year;
            }
        }

        public string DividendYieldNextYear
        {
            get
            {
                return "Dividend Yield " + (DateTime.Now.Year + 1);
            }
        }
    }
}
