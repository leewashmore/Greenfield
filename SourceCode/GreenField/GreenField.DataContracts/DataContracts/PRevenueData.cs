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
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    [DataContract]
    public class PRevenueData
    {
        [DataMember]
        public string PeriodLabel { get; set;}

        [DataMember]
        public decimal? PRevenueVal { get; set; }

        [DataMember]
        public decimal? Average { get; set; }

        [DataMember]
        public decimal? StdDevPlus { get; set; }

        [DataMember]
        public decimal? StdDevMinus { get; set; }
    }
}
