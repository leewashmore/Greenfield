using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpst
{
    public class ModelChangeDetector : ModelChangeDetectorBase
    {
        private ModelExpressionTraverser modelExpressionTraverser;
        
        [DebuggerStepThrough]
        public ModelChangeDetector(ModelExpressionTraverser modelExpressionTraverser)
        {
            this.modelExpressionTraverser = modelExpressionTraverser;
        }
        
        public Boolean HasChanged(RootModel root)
        {
            var expressions = this.modelExpressionTraverser.Traverse(root);
            var result = base.HasChanged(expressions);
            return result;
        }
    }
}
