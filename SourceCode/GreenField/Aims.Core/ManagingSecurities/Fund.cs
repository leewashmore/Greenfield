using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Aims.Core
{
    /// <summary>
    /// Security without a country.
    /// </summary>
    public class Fund : ISecurity
    {
        [DebuggerStepThrough]
        public Fund(String id, String name, String shortName, String ticker, String issuerId, String securityType, String currency, String isin, String isoCountryCode)
        {
            this.Id = id;
            this.Name = name;
            this.ShortName = shortName;
            this.Ticker = ticker;
            this.IssuerId = issuerId;
            this.SecurityType = securityType;
            this.Currency = currency;
            this.Isin = isin;
            this.IsoCountryCode = isoCountryCode;
        }

        public String Id { get; set; }
        public String Name { get; set; }
        public String ShortName { get; set; }
        public String Ticker { get; set; }
        public String IssuerId { get; set; }
        public String SecurityType { get; set; }
        public String Currency { get; set; }
        public String Isin { get; set; }
        public String IsoCountryCode { get; set; }

        [DebuggerStepThrough]
        public void Accept(ISecurityResolver resolver)
        {
            resolver.Resolve(this);
        }

        public override string ToString()
        {
            var result = String.Format(
                "Fund, ID: {0}, Ticker: {1}, Short name: {2}, Name: {3}, Issuer ID: {4}, Security type: {5}",
                this.Id,
                this.Ticker,
                this.Name,
                this.ShortName,
                this.IssuerId,
                this.SecurityType
            );
            return result;
        }





        
    }
}
