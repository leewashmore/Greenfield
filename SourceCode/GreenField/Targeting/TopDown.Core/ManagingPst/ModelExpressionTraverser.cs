using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    public class ModelExpressionTraverser
    {
        public IEnumerable<IExpression> Traverse(RootModel root)
        {
            var result = new List<IExpression>();
            this.Traverse(root, result);
            return result;
        }

        protected void Traverse(RootModel root, List<IExpression> result)
        {
            result.Add(root.TargetTotal);
            root.Items.ForEach(item => result.Add(item.Target));
        }

    }
}
