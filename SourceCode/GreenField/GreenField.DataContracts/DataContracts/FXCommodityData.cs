using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class FXCommodityData
    {
        [DataMember]
        public string CommodityId { get; set; }

        [DataMember]
        public decimal? CurrentYearEnd { get; set; }
 
        [DataMember]
        public decimal? NextYearEnd { get; set; }
        
        [DataMember]
        public decimal? LongTerm { get; set; }

        [DataMember]
        public DateTime? FromDate { get; set; }

        [DataMember]
        public decimal? DailyClosingPrice { get; set; }


        //[DataMember]
        //public decimal? GPToday { get; set; }

        //[DataMember]
        //public decimal? GPLastYearEnd { get; set; }

        //[DataMember]
        //public decimal? GP12MonthsAgo { get; set; }

        //[DataMember]
        //public decimal? GP36MonthsAgo { get; set; }
        
        [DataMember]
        public decimal? YTD { get; set; }

        [DataMember]
        public decimal? Year1 { get; set;}

        [DataMember]
        public decimal? Year3 { get; set; }

        [DataMember]
        public string InstrumentId { get; set; }


    }
}