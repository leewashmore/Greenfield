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
    public class FiveYearDataModels
    {
      
        public String CountryName { get; set; }
        
        public String CategoryName { get; set; }
       
        public String DisplayType { get; set; }
        
        public String Description { get; set; }
       
        public int SortOrder { get; set; }
      
        public Decimal? YearOne { get; set; }
     
        public Decimal? YearTwo { get; set; }

        public Decimal? YearThree { get; set; }
      
        public Decimal? YearFour { get; set; }
       
        public Decimal? YearFive { get; set; }

        public Decimal? YearSix { get; set; }

        public Decimal? FiveYearAvg { get; set; }
    }
}
