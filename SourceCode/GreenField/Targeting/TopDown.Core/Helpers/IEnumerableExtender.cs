using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core
{
	public static class IEnumerableExtender
	{
        [DebuggerStepThrough]
		public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> handler)
		{
			foreach (var value in values)
			{
				handler(value);
			}
		}
	}
}
