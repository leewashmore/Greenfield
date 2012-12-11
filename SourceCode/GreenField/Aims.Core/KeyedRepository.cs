using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aims.Core
{
	public class KeyedRepository<TKey, TValue> : Repository<TValue>
	{
		private Dictionary<TKey, TValue> map;
		
		public KeyedRepository()
		{
			this.map = new Dictionary<TKey, TValue>();
		}

		protected void RegisterValue(TValue value, TKey key)
		{
			base.RegisterValue(value);
			this.map.Add(key, value);
		}

		protected TValue FindValue(TKey id)
		{
			TValue found;
			if (this.map.TryGetValue(id, out found))
			{
				return found;
			}
			else
			{
				return default(TValue);
			}
		}
	}
}
