﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;

namespace TopDown.Core.ManagingBpst
{
    /// <summary>
    /// Set of compositions 
    /// </summary>
    public class RootModel
    {
        [DebuggerStepThrough]
        public RootModel(
            TargetingTypeGroupBasketSecurityBaseValueChangesetInfo latestBaseChangeset,
            BasketPortfolioSecurityTargetChangesetInfo latestPortfolioTargetChangeset,
            CoreModel core,
            DateTime benchmarkDate
        )
        {
            this.LatestBaseChangeset = latestBaseChangeset;
            this.LatestPortfolioTargetChangeset = latestPortfolioTargetChangeset;
            this.Core = core;
            this.BenchmarkDate = benchmarkDate;
        }

        public TargetingTypeGroupBasketSecurityBaseValueChangesetInfo LatestBaseChangeset { get; private set; }
        public BasketPortfolioSecurityTargetChangesetInfo LatestPortfolioTargetChangeset { get; private set; }
        public CoreModel Core { get; private set; }
        public DateTime BenchmarkDate { get; private set; }
    }
}
