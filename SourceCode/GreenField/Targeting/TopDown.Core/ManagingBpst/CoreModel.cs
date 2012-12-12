using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBaskets;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst
{
    public class CoreModel
    {
        [DebuggerStepThrough]
        public CoreModel(
            TargetingTypeGroup targetingTypeGroup,
            IBasket basket,
            IEnumerable<PortfolioModel> portfolios,
            IEnumerable<SecurityModel> securities,
            IExpression<Decimal?> baseTotalExpression,
            IExpression<Decimal> benchmarkTotalExpression
        )
        {
            this.TargetingTypeGroup = targetingTypeGroup;
            this.Basket = basket;
            this.Portfolios = portfolios.ToList();
            this.Securities = securities.ToList();
            this.BaseTotal = baseTotalExpression;
            this.BenchmarkTotal = benchmarkTotalExpression;
        }

        public TargetingTypeGroup TargetingTypeGroup { get; private set; }
        public IBasket Basket { get; private set; }
        public IEnumerable<PortfolioModel> Portfolios { get; private set; }
        public IEnumerable<SecurityModel> Securities { get; private set; }
        public IExpression<Decimal?> BaseTotal { get; private set; }
        public IExpression<Decimal> BenchmarkTotal { get; private set; }
    }
}
