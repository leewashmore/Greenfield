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
    public class PeriodAttributeData

    { 
       public  String Country { get; set; }

       public String CountryName { get; set; }

       public Decimal? PortfolioWeight { get; set; }

       public Decimal? BenchmarkWeight { get; set; }      

       public Decimal? PortfolioReturn { get; set; }

       public Decimal? BenchmarkReturn { get; set; }

       public Decimal? AssetAllocation { get; set; }

       public Decimal? StockSelectionTotal { get; set; }

    }
}
