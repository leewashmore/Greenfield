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
            var result = this.Calculate(ticket, No.CalculationTracer);
            return result;
        }


        public Decimal? Calculate(CalculationTicket ticket, ICalculationTracer tracer)
        {
            tracer.WriteLine("Cash base");
            tracer.Indent();
            var baseValue = root.Base.Value(ticket, tracer, "Base");
            Decimal? result;
            if (baseValue.HasValue)
            {
                result = 1m - baseValue.Value;
                tracer.WriteValue("1 - Base", result);
            }
            else
            {
                result = null;
                tracer.WriteValue("Nothing", null);
            }
            tracer.Unindent();
            return result;
        }
    }
}
