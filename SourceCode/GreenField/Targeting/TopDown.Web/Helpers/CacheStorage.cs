using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TopDown.Core;
using System.Diagnostics;
using System.Web.Caching;

namespace TopDown.Web
{
	public class CacheStorage<TValue> : IStorage<TValue>
		where TValue : class
	{
		private Cache cache;
		public CacheStorage(Cache cache)
		{
			this.cache = cache;
		}

		public TValue this[String key]
		{
			[DebuggerStepThrough]
			get
			{
				var result = this.cache.Get(key) as TValue;
				return result;
			}
			[DebuggerStepThrough]
			set
			{
                if (value == null)
                {
                    this.cache.Remove(key);
                }
                else
                {
                    this.cache.Insert(key, value);
                }
			}
		}
	}
}