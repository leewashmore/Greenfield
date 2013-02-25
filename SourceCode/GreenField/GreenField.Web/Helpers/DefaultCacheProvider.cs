using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace GreenField.Web.Helpers
{
    public sealed class CacheKeyNames
    {
        /*
        public const string SECURITY = "SECURITY";
        public const string BENCHMARK = "BENCHMARK";
        public const string INDEX = "INDEX";
        public const string COMMODITY = "COMMODITY";
        public const string CURRENCY = "CURRENCY";
         */

        // TODO following have input parameters and need a cache matrix
        // RetrieveFilterSelectionData
        // RetrieveMarketSnapshotSelectionData

        public const string EntitySelectionDataCache = "EntitySelectionDataCache";
        public const string FilterSelectionDataCache = "FilterSelectionDataCache";
        public const string PortfolioSelectionDataCache = "PortfolioSelectionDataCache";

        public const string AvailableDatesInPortfoliosCache = "AvailableDatesInPortfoliosCache";
        public const string CountrySelectionDataCache = "CountrySelectionDataCache";
        public const string RegionSelectionDataCache = "RegionSelectionDataCache";

        public const string FXCommodityDataCache = "FXCommodityDataCache";
        public const string MarketSnapshotSelectionDataCache = "MarketSnapshotSelectionDataCache";
        public const string LastDayOfMonthsCache = "LastDayOfMonthsCache";
    }

    public class CacheExpiration
    {
        public string CacheKeyName { get; set; }
        public string ShortName { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
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
            if (cache[key + "Policy"] == null)
                return null;

            return cache[key];
        }

        public void Set(string key, object data, int cacheTime)
        {
            var policy = new CacheItemPolicy {AbsoluteExpiration = DateTime.Now.AddMinutes(cacheTime)};
            //Hint: can also use policy.RemovedCallback = new CacheEntryRemovedCallback (ItemRemoved);

            cache.Add(new CacheItem(key, data), policy);
            cache.Add(new CacheItem(key + "Policy", policy), null);
        }

        // Not used
        public bool IsSet(string key)
        {
            return (cache[key] != null);
        }

        public void Invalidate(string key)
        {
            MemoryCache.Default.Remove(key);
            MemoryCache.Default.Remove(key + "Policy");
        }

        public CacheExpiration GetExpiration(string key)
        {
            if (cache[key + "Policy"] != null)
            {
                var cacheExpiration = new CacheExpiration
                    {
                        CacheKeyName = key,
                        AbsoluteExpiration = ((CacheItemPolicy) cache[key + "Policy"]).AbsoluteExpiration
                    };

                switch (key)
                {
                    case CacheKeyNames.EntitySelectionDataCache:
                        cacheExpiration.ShortName = "Entity";
                        break;
                    case CacheKeyNames.FilterSelectionDataCache:
                        cacheExpiration.ShortName = "Filter";
                        break;
                    case CacheKeyNames.PortfolioSelectionDataCache:
                        cacheExpiration.ShortName = "Portfolio";
                        break;

                    case CacheKeyNames.AvailableDatesInPortfoliosCache:
                        cacheExpiration.ShortName = "Dates";
                        break;
                    case CacheKeyNames.CountrySelectionDataCache:
                        cacheExpiration.ShortName = "Country";
                        break;
                    case CacheKeyNames.RegionSelectionDataCache:
                        cacheExpiration.ShortName = "Region";
                        break;

                    case CacheKeyNames.FXCommodityDataCache:
                        cacheExpiration.ShortName = "FXCommodity";
                        break;
                    case CacheKeyNames.MarketSnapshotSelectionDataCache:
                        cacheExpiration.ShortName = "MarketSnapshot";
                        break;
                    case CacheKeyNames.LastDayOfMonthsCache:
                        cacheExpiration.ShortName = "DayOfMonths";
                        break;
                }

                return cacheExpiration;
            }

            return null;
        }

        public List<CacheExpiration> GetAllExpirations()
        {
            var dates = new List<CacheExpiration>
                {
                    GetExpiration(CacheKeyNames.EntitySelectionDataCache),
                    GetExpiration(CacheKeyNames.FilterSelectionDataCache),
                    GetExpiration(CacheKeyNames.PortfolioSelectionDataCache),
                    GetExpiration(CacheKeyNames.AvailableDatesInPortfoliosCache),
                    GetExpiration(CacheKeyNames.CountrySelectionDataCache),
                    GetExpiration(CacheKeyNames.RegionSelectionDataCache),
                    GetExpiration(CacheKeyNames.FXCommodityDataCache),
                    GetExpiration(CacheKeyNames.MarketSnapshotSelectionDataCache),
                    GetExpiration(CacheKeyNames.LastDayOfMonthsCache)
                };

            return dates;
        }

        public void InvalidateAll()
        {
            //MemoryCache.Default.Dispose(); possible threading issues
            //TODO needs to test
            foreach (var element in MemoryCache.Default)
                Invalidate(element.Key);
        }


        public void InvalidateAllExceptEntity()
        {
            //MemoryCache.Default.Dispose(); possible threading issues
            //TODO needs to test
            foreach (var element in MemoryCache.Default)
                if (element.Key != CacheKeyNames.EntitySelectionDataCache)
                    Invalidate(element.Key);
        }
    }
}