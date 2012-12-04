using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core
{
	public class DecimalValueAdapter : IValueAdapter<Decimal>
	{
		public void Take(IValueTaker taker, Decimal value)
		{
			taker.TakeDecimal(value);
		}

		public Decimal Give(IValueGiver giver)
		{
			return giver.GiveDecimal();
		}
	}
}
