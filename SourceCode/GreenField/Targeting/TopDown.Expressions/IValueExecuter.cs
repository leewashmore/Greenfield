using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
    public interface IValueAdapter<TValue>
    {
        void Take(IValueTaker taker, TValue value);
		TValue Give(IValueGiver giver);
    }
}
