﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Core.Persisting
{
    public class SecurityInfo
    {
        [DebuggerStepThrough]
        public SecurityInfo()
        {
        }

        [DebuggerStepThrough]
        public SecurityInfo(String id, String ticker, String shortName, String name, String isoCountryCode, String lookThruFund, String issuerId, String issuerName)
            : this()
        {
            this.Id = id;
            this.Ticker = ticker;
            this.ShortName = shortName;
            this.Name = name;
            this.IsoCountryCode = isoCountryCode;
            this.LookThruFund = lookThruFund;
            this.IssuerId = issuerId;
            this.IssuerName = issuerName;
        }

        public String Id { get; set; }
        public String Ticker { get; set; }
        public String ShortName { get; set; }
        public String Name { get; set; }
        public String IsoCountryCode { get; set; }
        public String LookThruFund { get; set; }
        public String IssuerId { get; set; }
        public String IssuerName { get; set; }
    }
}
