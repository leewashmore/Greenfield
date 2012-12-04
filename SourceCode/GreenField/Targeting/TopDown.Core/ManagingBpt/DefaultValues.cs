using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt
{
	public class DefaultValues
	{
		[DebuggerStepThrough]
		public DefaultValues(
			Decimal defaultBenchmark,
			Decimal? defaultBase,
			Decimal? defaultBaseActive,
			Decimal defaultOverlay,
			Decimal? defaultPortfolioAdjustment,
			Decimal? defaultPortfolioScaled,
            Decimal? defaultTrueExposure,
            Decimal? defaultTrueActive
		)
		{
			this.DefaultBenchmark = defaultBenchmark;
			this.DefaultBase = defaultBase;
			this.DefaultBaseActive = defaultBaseActive;
			this.DefaultOverlay = defaultOverlay;
			this.DefaultPortfolioAdjustment = defaultPortfolioAdjustment;
            this.DefaultTrueExposure = defaultTrueExposure;
            this.DefaultTrueActive = defaultTrueActive;
		}

		public Decimal DefaultBenchmark { get; private set; }
		public Decimal? DefaultBase { get; private set; }
		public Decimal? DefaultBaseActive { get; private set; }
		public Decimal DefaultOverlay { get; private set; }
		public Decimal? DefaultPortfolioAdjustment { get; private set; }
		public Decimal? DefaultPortfolioScaled { get; private set; }
        public Decimal? DefaultTrueExposure { get; private set; }
        public Decimal? DefaultTrueActive { get; private set; }

		public static DefaultValues CreateDefaultValues()
		{
			var result = new DefaultValues(0.0m, null, null, 0.0m, null, null, null, null);
			return result;
		}


        
    }
}