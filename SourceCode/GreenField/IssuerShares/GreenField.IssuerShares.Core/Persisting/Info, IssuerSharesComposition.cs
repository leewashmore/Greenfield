using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GreenField.IssuerShares.Core.Persisting
{
    /// <summary>
    /// Represents a record of the ISSUER_SHARES_COMPOSITION table.
    /// </summary>
    public class IssuerSharesCompositionInfo
    {
        [DebuggerStepThrough]
        public IssuerSharesCompositionInfo()
        {
        }

        [DebuggerStepThrough]
        public IssuerSharesCompositionInfo(String securityId, String issuerId, Char preferred)
            : this()
        {
            this.SecurityId = securityId;
            this.IssuerId = issuerId;
            this.Preferred = preferred;
        }

        public String SecurityId { get; set; }

        public String IssuerId { get; set; }

        public Char Preferred { get; set; }
    }
}
