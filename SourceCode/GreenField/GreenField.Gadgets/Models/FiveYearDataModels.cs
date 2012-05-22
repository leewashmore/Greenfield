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
      
        public String COUNTRY_NAME { get; set; }
        
        public String CATEGORY_NAME { get; set; }
       
        public String DISPLAY_TYPE { get; set; }
        
        public String DESCRIPTION { get; set; }
       
        public int SORT_ORDER { get; set; }
      
        public Decimal? YEAR_ONE { get; set; }
     
        public Decimal? YEAR_TWO { get; set; }

        public Decimal? YEAR_THREE { get; set; }
      
        public Decimal? YEAR_FOUR { get; set; }
       
        public Decimal? YEAR_FIVE { get; set; }
    }
}
