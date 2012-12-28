using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Aims.Data.Server
{
    [DataContract]
    public class CompanySecurityModel : SecurityModel
    {
        [DebuggerStepThrough]
        public CompanySecurityModel()
        {
        }

        [DebuggerStepThrough]
        public CompanySecurityModel(String id, String name, String shortName, String ticker, CountryModel country, String issuerId, String securityType)
        {
            this.Id = id;
            this.Name = name;
            this.ShortName = shortName;
            this.Ticker = ticker;
            this.Country = country;
            this.IssuerId = issuerId;
            this.SecurityType = securityType;
        }

        

        [DataMember]
        public CountryModel Country { get; set; }
    }
}
