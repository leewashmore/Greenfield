using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;
using System.Diagnostics;

namespace TopDown.Core.Testing
{
    public class CalculationTracer : ICalculationTracer
    {
        public void WriteLine(String line)
        {
            Trace.WriteLine(line);
        }

        public void WriteValue(String name, Decimal? value)
        {
            Trace.WriteLine(name + ": " + value);
        }

        public void WriteValue<TValue>(String name, TValue value, IValueAdapter<TValue> adapter)
        {
            Trace.WriteLine(name + ": " + value);
        }

        public void Indent()
        {
            Trace.Indent();
        }

        public void Unindent()
        {
            Trace.Unindent();
        }
    }
}
