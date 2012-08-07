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
    public class GadgetWithPeriodColumns
    {
        public Int32? GridId { get; set; }

        public String GadgetName { get; set; }

        public String GadgetDesc { get; set; }
        
        public Decimal? Amount { get; set; }
      
        public Int32? PeriodYear { get; set; }

    }
}
