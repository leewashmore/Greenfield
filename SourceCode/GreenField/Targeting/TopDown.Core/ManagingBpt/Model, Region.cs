using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;
using Aims.Core;

namespace TopDown.Core.ManagingBpt
{
    public class RegionModel : Repository<IRegionModelResident>, IRegionModelResident, IGlobeResident, IModel
    {
        [DebuggerStepThrough]
        public RegionModel(
            String name,
            IExpression<Decimal> benchmarkExpression,
            IExpression<Decimal?> baseExpression,
            Func<RegionModel, IExpression<Decimal?>> baseActiveFormulaCreator,
            IExpression<Decimal> overlayExpression,
            IExpression<Decimal?> portfolioAdjustmentExpression,
            IExpression<Decimal?> portfolioScaledExpression,
            IExpression<Decimal?> trueExposureExpression,
            IExpression<Decimal?> trueActiveExpression,
            IEnumerable<IRegionModelResident> residents
        )
        {
            this.Name = name;
            this.Benchmark =benchmarkExpression ;
            this.Base = baseExpression;
            this.BaseActive = baseActiveFormulaCreator(this);
            this.Overlay = overlayExpression;
            this.PortfolioAdjustment = portfolioAdjustmentExpression;
            this.PortfolioScaled = portfolioScaledExpression;
            this.TrueExposure = trueExposureExpression;
            this.TrueActive = trueActiveExpression;
            this.Residents = residents;
        }

        public String Name { get; private set; }
        public IExpression<Decimal> Benchmark { get; private set; }
        public IExpression<Decimal?> Base { get; private set; }
        public IExpression<Decimal?> BaseActive { get; private set; }
        public IExpression<Decimal> Overlay { get; private set; }
        public IExpression<Decimal?> PortfolioAdjustment { get; private set; }
        public IExpression<Decimal?> PortfolioScaled { get; private set; }
        public IExpression<Decimal?> TrueExposure { get; private set; }
        public IExpression<Decimal?> TrueActive { get; private set; }
        public IEnumerable<IRegionModelResident> Residents { get; private set; }

        [DebuggerStepThrough]
        public void Accept(IRegionModelResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(IGlobeResidentResolver resolver)
        {
            resolver.Resolve(this);
        }

        [DebuggerStepThrough]
        public void Accept(IModelResolver resolver)
        {
            resolver.Resolve(this);
        }
    }
}
