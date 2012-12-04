using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core._Testing
{
    public static class Extender
    {
        public static TValue Next<TValue>(this IEnumerator<TValue> values)
        {
            if (!values.MoveNext()) throw new ApplicationException();
            return values.Current;
        }
    }
}
