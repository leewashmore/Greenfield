using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class CSTUserPreferenceInfo
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string ListName { get; set; }

        [DataMember]
        public string Accessibility { get; set; }

        [DataMember]
        public string ScreeningId { get; set; }

        [DataMember]
        public string DataDescription { get; set; }

        [DataMember]
        public string TableColumnName { get; set; }

        [DataMember]
        public int DataID { get; set; }

        [DataMember]
        public int EstimateID { get; set; }

        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public string YearType { get; set; }

        [DataMember]
        public int FromDate { get; set; }

        [DataMember]
        public int ToDate { get; set; }

        [DataMember]
        public int DataPointsOrder { get; set; }

        [DataMember]
        public String TableColumn { get; set; }
    }
}
