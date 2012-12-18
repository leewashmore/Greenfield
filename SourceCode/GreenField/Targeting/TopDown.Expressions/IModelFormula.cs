using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface IModelFormula<TModel, TValue>
    {
        TValue Calculate(TModel model, CalculationTicket ticket);
        TValue Calculate(TModel model, CalculationTicket ticket, ICalculationTracer tracer);
    }
}
