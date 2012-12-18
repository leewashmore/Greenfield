using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface IFormula<TValue>
    {
        TValue Calculate(CalculationTicket ticket);
        TValue Calculate(CalculationTicket ticket, ICalculationTracer tracer);
    }
}
