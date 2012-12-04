using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Expressions
{
	public interface IValueGiver
	{
		Decimal GiveDecimal();
		Decimal? GiveNullableDecimal();
		String GiveString();
	}
}
