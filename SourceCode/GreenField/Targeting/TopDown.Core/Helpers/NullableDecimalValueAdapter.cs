using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core
{
	public class NullableDecimalValueAdapter : IValueAdapter<Decimal?>
	{
		public void Take(IValueTaker taker, Decimal? value)
		{
			taker.TakeNullableDecimal(value);
		}

		public Decimal? Give(IValueGiver giver)
		{
			return giver.GiveNullableDecimal();
		}
	}
}
