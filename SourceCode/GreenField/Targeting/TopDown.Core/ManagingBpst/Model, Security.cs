using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingSecurities;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingBpst
{
    public class SecurityModel
    {
		[DebuggerStepThrough]
        public SecurityModel(
			ISecurity security,
			EditableExpression baseExpression,
			UnchangableExpression<Decimal> benchmark,
			IEnumerable<PortfolioTargetModel> portfolioTargets
		)
        {
            this.Security = security;
			this.Base = baseExpression;
            this.Benchmark = benchmark;
			this.PortfolioTargets = portfolioTargets.ToList();
        }

        public ISecurity Security { get; private set; }
        public EditableExpression Base { get; private set; }
        public UnchangableExpression<Decimal> Benchmark { get; private set; }
		public IEnumerable<PortfolioTargetModel> PortfolioTargets { get; private set; }
    }
}
