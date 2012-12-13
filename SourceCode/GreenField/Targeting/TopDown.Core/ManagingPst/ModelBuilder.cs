using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;
using TopDown.Core.ManagingSecurities;
using Aims.Core;

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

        public ItemModel CreateItem(
            ISecurity security,
            EditableExpression targetExpression
        )
        {
            var result = new ItemModel(security, targetExpression);
            return result;
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
            var name = ValueNames.Target;
            var result = this.commonParts.CreateInputExpression(name, this.defaultTarget);
            return result;
        }

        private class CashFormula : IFormula<Decimal?>
        {
            private NullableSumExpression targetTotalExpression;

            public CashFormula(NullableSumExpression targetTotalExpression)
            {
                this.targetTotalExpression = targetTotalExpression;
            }

            public Decimal? Calculate(CalculationTicket ticket)
            {
                Decimal? result;
                var total = this.targetTotalExpression.Value(ticket);
                if (total.HasValue)
                {
                    result = 1.0m - total.Value;
                }
                else
                {
                    result = null;   
                }
                return result;
            }
        }

        public IExpression<Decimal?> CreateCashExpression(NullableSumExpression targetTotalExpression)
        {
            var formula = new CashFormula(targetTotalExpression);
            var result = new Expression<Decimal?>("Cash", formula, this.commonParts.NullableDecimalValueAdapter, this.commonParts.ValidateNonNegativeOrNull);
            return result;
        }
    }
}
