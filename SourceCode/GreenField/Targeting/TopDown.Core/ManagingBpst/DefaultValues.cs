using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpst
{
    public class DefaultValues
    {
        [DebuggerStepThrough]
        public DefaultValues(Decimal? defaultBase, Decimal defaultBenchmark, Decimal? defaultPortfolioTarget)
        {
            this.DefaultBase = defaultBase;
            this.DefaultBenchmark = defaultBenchmark;
            this.DefaultPortfolioTarget = defaultPortfolioTarget;
        }

        public Decimal? DefaultBase { get; private set; }
        public Decimal DefaultBenchmark { get; private set; }
        public Decimal? DefaultPortfolioTarget { get; private set; }

        public static DefaultValues CreateDefaultValues()
        {
            var result = new DefaultValues(null, 0.0m, null);
            return result;
        }
    }
}
