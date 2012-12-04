using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpst
{
    public class ModelExpressionTraverser
    {
        public IEnumerable<IExpression> Traverse(RootModel root)
        {
            var result = new List<IExpression>();
            this.TraverseRoot(root, result);
            return result;
        }

        protected void TraverseRoot(RootModel root, List<IExpression> result)
        {
            result.Add(root.Core.BaseTotal);
            
            root.Core.Portfolios.ForEach(portfolio =>
            {
                result.Add(portfolio.PortfolioTargetTotal);
            });
            
            root.Core.Securities.ForEach(security =>
            {
                result.Add(security.Base);
                result.Add(security.Benchmark);
                result.AddRange(security.PortfolioTargets.Select(x => x.Target));
            });
        }
    }
}
