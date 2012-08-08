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

namespace GreenField.Gadgets.Models
{
    public class PerformancePeriodData
    {
        public String DataPointName { get; set; }

        public Decimal? BenchmarkValue { get; set; }

        public Decimal? PortfolioValue { get; set; }
    }
}
