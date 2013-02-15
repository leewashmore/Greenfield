using System;
using System.Runtime.Caching;

namespace GreenField.Web.Helpers
{
    public interface ICacheProvider
    {
        object Get(string key);
        void Set(string key, object data, int cacheTime);
        bool IsSet(string key);
        void Invalidate(string key);
    }

    public class DefaultCacheProvider : ICacheProvider
    {
        private ObjectCache cache
        {
            get { return MemoryCache.Default; }
        }

        public object Get(string key)
        {
            return cache[key];
        }

        public void Set(string key, object data, int cacheTime)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.AddMinutes(cacheTime);

            cache.Add(new CacheItem(key, data), policy);
            cache.Add(new CacheItem(key + "Policy", data), null);
        }

        public bool IsSet(string key)
        {
            return (cache[key] != null);
        }

        public void Invalidate(string key)
        {
            cache.Remove(key);
        }

        public void InvalidateAll()
        {
            //MemoryCache.Default.Dispose(); possible threading issues
            //TODO needs to test
            foreach (var element in MemoryCache.Default)
                MemoryCache.Default.Remove(element.Key);
        }
    }
}