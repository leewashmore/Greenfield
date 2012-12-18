using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Aims.Core;

namespace TopDown.Core.Testing
{
	public class InMemoryStorage<TValue> : IStorage<TValue>
		where TValue : class
	{
		private Dictionary<String, TValue> map;
		[DebuggerStepThrough]
		public InMemoryStorage()
		{
			this.map = new Dictionary<String, TValue>();
		}
		public TValue this[String key]
		{
			get
			{
				TValue found;
				if (this.map.TryGetValue(key, out found)) return found;
				return null;
			}
			set
			{
				TValue found;
				if (this.map.TryGetValue(key, out found))
				{
					if (Object.ReferenceEquals(found, null))
					{
						this.map[key] = value;
					}
					else
					{
						throw new ApplicationException("There is an entry with the same key \"" + key + "\" already.");
					}
				}
				else
				{
					this.map.Add(key, value);
				}
			}
		}
	}
}
