using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingPst
{
    public class PstUpdateChange : IPstChange
    {
        [DebuggerStepThrough]
        public PstUpdateChange(String portfolioId, String securityId, Decimal targetBefore, Decimal targetAfter, String comment)
        {
            this.PortfolioId = portfolioId;
            this.SecurityId = securityId;
            this.TargetBefore = targetBefore;
            this.TargetAfter = targetAfter;
            this.Comment = comment;
        }

        public String PortfolioId { get; private set; }
        public String SecurityId { get; private set; }
        public Decimal TargetBefore { get; private set; }
        public Decimal TargetAfter { get; private set; }
        public String Comment { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IPstChangeResolver resolver)
        {
            resolver.Resolve(this);
        }

        
    }
}
