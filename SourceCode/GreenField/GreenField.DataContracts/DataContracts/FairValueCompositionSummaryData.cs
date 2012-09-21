using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GreenField.DataContracts
{
    [DataContract]
    public class FairValueCompositionSummaryData
    {
        [DataMember]
        public string SOURCE{get; set;}

        [DataMember]
        public string MEASURE{get;set;}

        [DataMember]
        public decimal? BUY{get; set;}

        [DataMember]
        public decimal? SELL { get; set; }

        [DataMember]
        public decimal? UPSIDE { get; set; }

        [DataMember]
        public DateTime? DATE { get; set; }

        [DataMember]
        public Int32? DATA_ID { get; set; }

        [DataMember]
        public List<MeasuresList> MEASURES_LIST { get; set; }

    }

    public class MeasuresList
    {
        private string _measures;
        public string Measures
        {
            get { return _measures; }
            set { _measures = value; }
        }

        private int _dataId;
        public int DataId
        {
            get { return _dataId; }
            set { _dataId = value; }
        }
    }
}
