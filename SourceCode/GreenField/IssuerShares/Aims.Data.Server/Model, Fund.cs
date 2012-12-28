using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace Aims.Data.Server
{
    [DataContract]
    public class FundModel : SecurityModel
    {
        [DebuggerStepThrough]
        public FundModel(String id, String name, String shortName, String ticker, String issuerId, String securityType)
        {
            this.Id = id;
            this.Name = name;
            this.ShortName = shortName;
            this.Ticker = ticker;
            this.IssuerId = issuerId;
            this.SecurityType = securityType;
        }

       
    }
}
