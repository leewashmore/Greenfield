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
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Specifies the Market Capitalization data points
    /// and associated portfolio and benchmark values
    /// </summary>
    public class MarketCapitalizationDataPoint
    {
        /// <summary>
        /// Data Point Name
        /// </summary>
        private String name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;                    
                }
            }
        }

        /// <summary>
        /// Portfolio Value
        /// </summary>
        private String portfolioValue;
        public string PortfolioValue
        {
            get
            {
                return portfolioValue;
            }
            set
            {
                if (portfolioValue != value)
                {
                    portfolioValue = value;                    
                }
            }
        }

        /// <summary>
        /// Benhcmark Value
        /// </summary>
        private String benchmarkValue;
        public string BenchmarkValue
        {
            get
            {
                return benchmarkValue;
            }
            set
            {
                if (benchmarkValue != value)
                {
                    benchmarkValue = value;
                }
            }
        }      
    }
}
