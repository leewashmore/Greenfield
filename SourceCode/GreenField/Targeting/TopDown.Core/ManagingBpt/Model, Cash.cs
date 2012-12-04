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
        public CashModel(IExpression<Decimal?> @base, IExpression<Decimal?> @scaled)
		{
			this.Base = @base;
            this.Scaled = @scaled;
		}

		public IExpression<Decimal?> Base { get; private set; }
        public IExpression<Decimal?> Scaled { get; private set; }

		// We don't need BaseActive
		// because it is always has the same value as Base
		// because cash doesn't have benchmark (because it must be always 0 as a residual of the sum of country benchmarks which is always 100%)
		// and BaseActive = Base - Benchmark
		// public Decimal BaseActive { get; set; }
	}
}
