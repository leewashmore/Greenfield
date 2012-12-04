using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Expressions;

namespace TopDown.Core.ManagingBpt.Computing
{
    public class BaseForCashFormula : IFormula<Decimal?>
    {
        private GlobeModel root;

        [DebuggerStepThrough]
        public BaseForCashFormula(GlobeModel root)
        {
            this.root = root;
        }

        public Decimal? Calculate(CalculationTicket ticket)
        {
            var baseValue = root.Base.Value(ticket);
            if (baseValue.HasValue)
            {
                return 1m - baseValue.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
