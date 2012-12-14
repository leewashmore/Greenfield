using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
	public class CashModel
	{
        public CashModel(IExpression<Decimal?> @base, IExpression<Decimal?> scaled, IExpression<Decimal?> trueExposure, IExpression<Decimal?> trueActive)
		{
			this.Base = @base;
            this.PortfolioScaled = scaled;
            this.TrueExposure = trueExposure;
            this.TrueActive = trueActive;
		}

		public IExpression<Decimal?> Base { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<decimal?> TrueExposure { get; set; }
        public IExpression<decimal?> TrueActive { get; set; }

		// We don't need BaseActive
		// because it is always has the same value as Base
		// because cash doesn't have benchmark (because it must be always 0 as a residual of the sum of country benchmarks which is always 100%)
		// and BaseActive = Base - Benchmark
		// public Decimal BaseActive { get; set; }
        
    }
}
