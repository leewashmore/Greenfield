using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface IValueTaker
    {
        void TakeDecimal(Decimal value);
        void TakeNullableDecimal(Decimal? value);
        void TakeString(String value);
    }
}
