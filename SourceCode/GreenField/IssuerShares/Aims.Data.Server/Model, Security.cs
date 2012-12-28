using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Aims.Data.Server
{
    [DataContract]
    [KnownType(typeof(CompanySecurityModel))]
    [KnownType(typeof(FundModel))]
    public class SecurityModel
    {
        [DebuggerStepThrough]
        public SecurityModel()
        {
        }

        [DataMember]
        public String Id { get; set; }

        [DataMember]
        public String Name { get; set; }

        [DataMember]
        public String ShortName { get; set; }

        [DataMember]
        public String Ticker { get; set; }

        [DataMember]
        public String IssuerId { get; set; }

        [DataMember]
        public String SecurityType { get; set; }

    }
}
