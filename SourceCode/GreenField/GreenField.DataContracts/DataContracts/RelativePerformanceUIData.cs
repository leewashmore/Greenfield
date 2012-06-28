using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract for Relative Performance UI
    /// </summary>
    [DataContract]
    public class RelativePerformanceUIData
    {
        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string EntityId { get; set; }

        [DataMember]
        public string EntityType { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public decimal MTDReturn { get; set; }

        [DataMember]
        public decimal QTDReturn { get; set; }

        [DataMember]
        public decimal YTDReturn { get; set; }

        [DataMember]
        public decimal OneYearReturn { get; set; }

        [DataMember]
        public decimal ThreeYearReturn { get; set; }

        [DataMember]
        public decimal FiveYearReturn { get; set; }

        [DataMember]
        public int SortId { get; set; }

    }

}