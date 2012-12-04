using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;

namespace TopDown.Core
{
	public class StringValueAdapter : IValueAdapter<String>
	{
		public void Take(IValueTaker taker, String value)
		{
			taker.TakeString(value);
		}

		public String Give(IValueGiver giver)
		{
			return giver.GiveString();
		}
	}
}
