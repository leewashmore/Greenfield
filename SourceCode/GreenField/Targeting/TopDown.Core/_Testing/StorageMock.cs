using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core._Testing
{
	public class StorageMock<TValue> : IStorage<TValue>
	{
		public TValue this[String key]
		{
			get
			{
				return default(TValue);
			}
			set
			{
				
				// do nothing
			}
		}
	}
}
