using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;
using Aims.Expressions;

namespace TopDown.Core.ManagingCalculations
{
    public class TradingTargetRecord
    {
        [DebuggerStepThrough]
        public TradingTargetRecord(
            String portfolioId,
            ISecurity security,
            Decimal target
        )
        {
            this.Security = security;
            this.PortfolioId = portfolioId;
            this.Target = target;
        }

        public String PortfolioId { get; set; }
        public ISecurity Security { get; set; }
        public Decimal Target { get; set; }
        public Decimal? SumByCountry { get; set; }
        public Decimal? PercentByContry { get; set; }
    }
}
