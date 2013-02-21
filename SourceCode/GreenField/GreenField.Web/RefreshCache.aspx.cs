using System;
using System.Collections.Generic;
using GreenField.Web.Helpers;
using GreenField.Web.Services;

namespace GreenField.Web
{
    public partial class RefreshCache : System.Web.UI.Page
    {
        public List<CacheExpiration> CacheExpirations;
        public double x10 = 10.0;
        
        protected void Page_Load(object sender, EventArgs e)
        { 
            //if (!IsPostBack)
            {
                Chart1.ChartAreas[0].AxisY.Title = "Expires in Hours";
                var dataSeries = Chart1.Series["Series1"];

                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.EntitySelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.EntitySelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.FilterSelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.FilterSelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.PortfolioSelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.PortfolioSelectionDataCache));

                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.AvailableDatesInPortfoliosCache),
                                        GetExpirationInHours(CacheKeyNames.AvailableDatesInPortfoliosCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.CountrySelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.CountrySelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.RegionSelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.RegionSelectionDataCache));

                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.FXCommodityDataCache),
                                        GetExpirationInHours(CacheKeyNames.FXCommodityDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.MarketSnapshotSelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.MarketSnapshotSelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.LastDayOfMonthsCache),
                                        GetExpirationInHours(CacheKeyNames.LastDayOfMonthsCache));
            }
            /*
            CacheExpirations = new DefaultCacheProvider().GetAllExpirations();

            foreach(CacheExpiration cacheExpiration in CacheExpirations)
            {
                String x, x2, x3;
                if (cacheExpiration != null)
                {
                    x = cacheExpiration.CacheKeyName;
                    x2 = cacheExpiration.ShortName;
                    x3 = cacheExpiration.AbsoluteExpiration.ToString();
                    if (cacheExpiration.AbsoluteExpiration != null)
                    {
                        TimeSpan minutesToExpire = cacheExpiration.AbsoluteExpiration.Value.Subtract(DateTime.Now);
                    }
                }
            }*/
        }

        private double GetExpirationInHours(string key)
        {
            CacheExpiration cacheExpiration = new DefaultCacheProvider().GetExpiration(key);
            
            if (cacheExpiration != null && cacheExpiration.AbsoluteExpiration != null)
                return Math.Round(cacheExpiration.AbsoluteExpiration.Value.Subtract(DateTime.Now).TotalHours, 2);

            return 0.0;
        }

        private string GetExpiration(string key)
        {
            CacheExpiration cacheExpiration = new DefaultCacheProvider().GetExpiration(key);

            if (cacheExpiration != null && cacheExpiration.AbsoluteExpiration != null)
                return cacheExpiration.AbsoluteExpiration.Value.Subtract(DateTime.Now).ToString();

            return "";
        }

        private string GetShortName(string key)
        {
            CacheExpiration cacheExpiration = new DefaultCacheProvider().GetExpiration(key);

            return cacheExpiration != null ? cacheExpiration.ShortName : "";
        }

        protected void RefreshEntities_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.EntitySelectionDataCache);
            new SecurityReferenceOperations().RetrieveEntitySelectionData();
            Server.Transfer("Bridge.aspx");
        }

        protected void RefreshAll_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().InvalidateAllExceptEntity();
            
            //new BenchmarkHoldingsOperations().RetrieveFilterSelectionData();
            new BenchmarkHoldingsOperations().RetrievePortfolioSelectionData();

            new BenchmarkHoldingsOperations().RetrieveAvailableDatesInPortfolios();
            new ModelFXOperations().RetrieveCountrySelectionData();
            new ModelFXOperations().RetrieveRegionSelectionData();

            new ModelFXOperations().RetrieveCommoditySelectionData();
            new PerformanceOperations().RetrieveMarketSnapshotSelectionData("JADHAVK");
            new PerformanceOperations().GetLastDayOfMonths();

            Response.Redirect("Bridge.aspx");
        }
    }
}