using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingSecurities
{
    /// <summary>
    /// Security without a country.
    /// </summary>
    public class Fund : ISecurity
    {
        [DebuggerStepThrough]
        public Fund(String id, String name, String shortName, String ticker)
        {
            this.Id = id;
            this.Name = name;
            this.ShortName = shortName;
            this.Ticker = ticker;
        }

        public String Id { get; set; }
        public String Name { get; set; }
        public String ShortName { get; set; }
        public String Ticker { get; set; }

        [DebuggerStepThrough]
        public void Accept(ISecurityResolver resolver)
        {
            resolver.Resolve(this);
        }

        public override string ToString()
        {
            var result = String.Format(
                "Fund, ID: {0}, Ticker: {1}, Short name: {2}, Name: {3}",
                this.Id,
                this.Ticker,
                this.Name,
                this.ShortName
            );
            return result;
        }
    }
}
