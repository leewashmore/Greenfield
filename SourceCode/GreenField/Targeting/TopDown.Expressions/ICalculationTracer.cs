using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface ICalculationTracer
    {
        void WriteLine(String line);
        void WriteValue(String name, Decimal? value);
        void WriteValue<TValue>(String name, TValue value, IValueAdapter<TValue> adapter);
        void Indent();
        void Unindent();
    }
}
