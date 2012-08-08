using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// DataContract class for ChartExtension Gadget, Holdings 4.4.1
    /// </summary>
    [DataContract]
    public class ChartExtensionData
    {
        [DataMember]
        public DateTime ToDate { get; set; }

        [DataMember]
        public string Ticker { get; set; }
                
        [DataMember]
        public string Type { get; set; }
                              
        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public decimal AdjustedDollarPrice { get; set; }

        [DataMember]
        public decimal PriceReturn { get; set; }

        [DataMember]
        public decimal? AmountTraded { get; set; }

        [DataMember]
        public decimal OneD { get; set; }

        [DataMember]
        public decimal WTD { get; set; }

        [DataMember]
        public decimal MTD { get; set; }

        [DataMember]
        public decimal QTD { get; set; }

        [DataMember]
        public decimal YTD { get; set; }

        [DataMember]
        public decimal OneY { get; set; }

        [DataMember]
        public int SortId { get; set; }
               
    }
}