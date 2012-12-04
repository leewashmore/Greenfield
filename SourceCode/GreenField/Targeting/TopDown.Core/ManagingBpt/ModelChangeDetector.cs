using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpt
{
    public class ModelChangeDetector : ModelChangeDetectorBase
    {
        private ModelExpressionTraverser expressionTraverser;

        [DebuggerStepThrough]
        public ModelChangeDetector(ModelExpressionTraverser expressionTraverser)
        {
            this.expressionTraverser = expressionTraverser;
        }

        public Boolean HasChanged(RootModel root)
        {
            var expressions = this.expressionTraverser.Traverse(root);
            return base.HasChanged(expressions);
        }

    }
}
