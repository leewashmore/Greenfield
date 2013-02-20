using System;
using System.Runtime.Caching;
using System.Collections.Generic;

namespace GreenField.Web.Helpers
{
    public sealed class CacheKeyNames
    {
        public const string FilterSelectionDataCache = "FilterSelectionDataCache";
        public const string PortfolioSelectionDataCache = "PortfolioSelectionDataCache";
        public const string AvailableDatesInPortfoliosCache = "AvailableDatesInPortfoliosCache";
        public const string CountrySelectionDataCache = "CountrySelectionDataCache";
        public const string RegionSelectionDataCache = "RegionSelectionDataCache";
        public const string FXCommodityDataCache = "FXCommodityDataCache";
        public const string MarketSnapshotSelectionDataCache = "MarketSnapshotSelectionDataCache";
        public const string LastDayOfMonthsCache = "LastDayOfMonthsCache";
        public const string EntitySelectionDataCache = "EntitySelectionDataCache";
    }
    
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
            //can also use: policy.RemovedCallback = new CacheEntryRemovedCallback (ItemRemoved);

            cache.Add(new CacheItem(key, data), policy);
            cache.Add(new CacheItem(key + "Policy", data), null);
        }

        /*
        public DateTime GetExpiration(string key)
        {
            return  ((CacheItemPolicy)cache[key + "Policy"]).AbsoluteExpiration;
        }*/

        public List<DateTime> GetExpiration()
        {
            return null;

            /*
            List<DateTime> dates = new List<DateTime>;

            dates.add(GetExpiration(FilterSelectionDataCache));
            dates.add(GetExpiration(PortfolioSelectionDataCache));
            dates.add(GetExpiration(AvailableDatesInPortfoliosCache));
            dates.add(GetExpiration(CountrySelectionDataCache));
            dates.add(GetExpiration(RegionSelectionDataCache));
            dates.add(GetExpiration(FXCommodityDataCache));
            dates.add(GetExpiration(MarketSnapshotSelectionDataCache));
            dates.add(GetExpiration(LastDayOfMonthsCache));
            dates.add(GetExpiration(EntitySelectionDataCache));

            return dates;
             */
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