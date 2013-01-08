using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace GreenField.IssuerShares.Server
{
    [DataContract]
    public class ShareRecordModel
    {
        [DataMember]
        public Int32? SecurityId { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public Decimal SharesOutstanding { get; set; }
    }

    [DataContract]
    public class IssuerShareRecordModel : ShareRecordModel
    {
        [DataMember]
        public String IssuerId { get; set; }
    }

    [DataContract]
    public class IssuerSecurityShareRecordModel
    {
        [DataMember]
        public Int32? SecurityId { get; set; }
        [DataMember]
        public String SecurityTicker { get; set; }
        [DataMember]
        public String IssuerId { get; set; }
        [DataMember]
        public IEnumerable<IssuerShareRecordModel> Shares { get; set; }
    }
}
