using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public class No
    {
        public const String ExpressionName = null;

        private static readonly ICalculationTracer calculationTracer = new NoCalculationTracer();
        public static ICalculationTracer CalculationTracer { get { return calculationTracer; } }

        private class NoCalculationTracer : ICalculationTracer
        {
            public void WriteLine(String line)
            {

            }

            public void WriteValue(String name, Decimal? value)
            {

            }

            public void Indent()
            {

            }

            public void Unindent()
            {

            }


            public void WriteValue<TValue>(string name, TValue value, IValueAdapter<TValue> adapter)
            {
                
            }
        }

    }
}
