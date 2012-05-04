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
       public  String COUNTRY { get; set; }

       public String COUNTRY_NAME { get; set; }

       public Decimal? PORTFOLIO_WEIGHT { get; set; }

       public Decimal? BENCHMARK_WEIGHT { get; set; }      

       public Decimal? PORTFOLIO_RETURN { get; set; }

       public Decimal? BENCHMARK_RETURN { get; set; }

       public Decimal? ASSET_ALLOCATION { get; set; }

       public Decimal? STOCK_SELECTION_TOTAL { get; set; }

    }
}
