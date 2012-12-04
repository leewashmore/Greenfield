using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingPst
{
    public class ModelBuilder
    {
        private CommonParts commonParts;
		private Decimal? defaultTarget;

        [DebuggerStepThrough]
        public ModelBuilder(
            Decimal? defaultTarget,
            CommonParts commonParts
        )
        {
			this.defaultTarget = defaultTarget;
            this.commonParts = commonParts;
        }

        public NullableSumExpression CreateTargetTotalExpression(IEnumerable<ItemModel> items)
        {
            return new NullableSumExpression(
				ValueNames.TargetTotal,
                items.Select(x => x.Target),
                this.defaultTarget,
                commonParts.ValidateNonNegativeOrNull
            );
        }

        public EditableExpression CreateTargetExpression()
        {
			return this.commonParts.CreateInputExpression(ValueNames.Target, this.defaultTarget);
        }
    }
}
