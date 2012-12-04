using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt
{
    public class GlobeModel
    {
        public GlobeModel(
            ICollection<IGlobeResident> residents,
            IExpression<Decimal?> baseExpression,
            IExpression<Decimal> benchmarkExpression,
            IExpression<Decimal> overlayExpression,
            IExpression<Decimal?> portfolioAdjustmentExpression,
            IExpression<Decimal?> portfolioScaledExpression,
            IExpression<Decimal?> trueExposureExpression,
            IExpression<Decimal?> trueActiveExpression
        )
        {
            this.Residents = residents;
            this.Base = baseExpression;
            this.Benchmark = benchmarkExpression;
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueExposure = trueExposureExpression;
            this.TrueActive = trueActiveExpression;
        }

        public ICollection<IGlobeResident> Residents { get; private set; }
        public IExpression<Decimal?> Base { get; private set; }
        public IExpression<Decimal> Benchmark { get; private set; }
        public IExpression<Decimal> Overlay { get; private set; }
        public IExpression<Decimal?> PortfolioAdjustment { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<Decimal?> TrueExposure { get; private set; }
        public IExpression<Decimal?> TrueActive { get; private set; }
    }
}
