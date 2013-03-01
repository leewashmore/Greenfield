using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using GreenField.Web.Helpers;
using GreenField.Web.Services;

namespace GreenField.Web
{
    public partial class RefreshCache : Page
    {
        public List<CacheExpiration> CacheExpirations;
        public double x10 = 10.0;

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            {
                Chart1.ChartAreas[0].AxisY.Title = "Expires in Hours";
                Chart1.ChartAreas[0].AxisX.Interval = 1;
                Series dataSeries = Chart1.Series["Series1"];

                // 1. Security
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.SecurityDataCache),
                                        GetExpirationInHours(CacheKeyNames.SecurityDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.EntitySelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.EntitySelectionDataCache));
                //dataSeries.Points[0].Color = System.Drawing.Color.Black;

                // 2. Benchmark
                //dataSeries.Points.AddXY(GetShortName(CacheKeyNames.FilterSelectionDataCache),
                //                        GetExpirationInHours(CacheKeyNames.FilterSelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.PortfolioSelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.PortfolioSelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.AvailableDatesInPortfoliosCache),
                                        GetExpirationInHours(CacheKeyNames.AvailableDatesInPortfoliosCache));

                // 3. ModelFX
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.CountrySelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.CountrySelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.RegionSelectionDataCache),
                                        GetExpirationInHours(CacheKeyNames.RegionSelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.FXCommodityDataCache),
                                        GetExpirationInHours(CacheKeyNames.FXCommodityDataCache));

                // 4. Performance
                //dataSeries.Points.AddXY(GetShortName(CacheKeyNames.MarketSnapshotSelectionDataCache),
                //                        GetExpirationInHours(CacheKeyNames.MarketSnapshotSelectionDataCache));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.LastDayOfMonthsCache),
                                        GetExpirationInHours(CacheKeyNames.LastDayOfMonthsCache));

                // 5. Targeting
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.PortfolioRepository),
                                        GetExpirationInHours(CacheKeyNames.PortfolioRepository));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.SecurityManager),
                                        GetExpirationInHours(CacheKeyNames.SecurityManager));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.CountryRepository),
                                        GetExpirationInHours(CacheKeyNames.CountryRepository));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.TaxonomyRepository),
                                        GetExpirationInHours(CacheKeyNames.TaxonomyRepository));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.BasketRepository),
                                        GetExpirationInHours(CacheKeyNames.BasketRepository));
                dataSeries.Points.AddXY(GetShortName(CacheKeyNames.TargetingTypeRepository),
                                        GetExpirationInHours(CacheKeyNames.TargetingTypeRepository));
            }
        }

        private double GetExpirationInHours(string key)
        {
            CacheExpiration cacheExpiration = new DefaultCacheProvider().GetExpiration(key);

            if (cacheExpiration != null && cacheExpiration.AbsoluteExpiration != null)
                return Math.Round(cacheExpiration.AbsoluteExpiration.Value.Subtract(DateTime.Now).TotalHours, 4);

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

        protected void RefreshAll_Click(object sender, EventArgs e)
        {
            //new DefaultCacheProvider().InvalidateAllExceptEntity();
            new DefaultCacheProvider().Invalidate(CacheKeyNames.SecurityDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.EntitySelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.PortfolioSelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.AvailableDatesInPortfoliosCache);

            new DefaultCacheProvider().Invalidate(CacheKeyNames.CountrySelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.RegionSelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.FXCommodityDataCache);

            new DefaultCacheProvider().Invalidate(CacheKeyNames.LastDayOfMonthsCache);


            // 1. Security
            new SecurityReferenceOperations().RetrieveEntitySelectionData();

            // 2. Benchmark
            //new BenchmarkHoldingsOperations().RetrieveFilterSelectionData();
            new BenchmarkHoldingsOperations().RetrievePortfolioSelectionData();
            new BenchmarkHoldingsOperations().RetrieveAvailableDatesInPortfolios();

            // 3. ModelFX
            new ModelFXOperations().RetrieveCountrySelectionData();
            new ModelFXOperations().RetrieveRegionSelectionData();
            new ModelFXOperations().RetrieveCommoditySelectionData();

            // 4. Performance
            //new PerformanceOperations().RetrieveMarketSnapshotSelectionData("JADHAVK");
            new PerformanceOperations().GetLastDayOfMonths();

            // 5. Targeting


            
            Response.Redirect("Bridge.aspx");
        }

        protected void RefreshSecuritiesCache_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.SecurityDataCache);
            new SecurityReferenceOperations().RetrieveSecuritiesData();
            Server.Transfer("Bridge.aspx");
        }

        protected void RefreshEntitiesCache_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.EntitySelectionDataCache);
            new SecurityReferenceOperations().RetrieveEntitySelectionData();
            Server.Transfer("Bridge.aspx");
        }
        
        protected void RefreshBenchmarkCache_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.PortfolioSelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.AvailableDatesInPortfoliosCache);

            new BenchmarkHoldingsOperations().RetrievePortfolioSelectionData();
            new BenchmarkHoldingsOperations().RetrieveAvailableDatesInPortfolios();

            Server.Transfer("Bridge.aspx");
        }

        protected void RefreshModelFXCache_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.CountrySelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.RegionSelectionDataCache);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.FXCommodityDataCache);

            new ModelFXOperations().RetrieveCountrySelectionData();
            new ModelFXOperations().RetrieveRegionSelectionData();
            new ModelFXOperations().RetrieveCommoditySelectionData();

            Server.Transfer("Bridge.aspx");
        }

        protected void RefreshPerformanceCache_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.LastDayOfMonthsCache);
            new PerformanceOperations().GetLastDayOfMonths();

            Server.Transfer("Bridge.aspx");
        }

        protected void InvalidateTargeting_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Invalidate(CacheKeyNames.PortfolioRepository);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.SecurityManager);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.CountryRepository);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.TaxonomyRepository);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.BasketRepository);
            new DefaultCacheProvider().Invalidate(CacheKeyNames.TargetingTypeRepository);

            Server.Transfer("Bridge.aspx");
        }

        protected void InvalidateAll_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().InvalidateAll();
            Server.Transfer("Bridge.aspx");
        }

        protected void RefreshGraph_Click(object sender, EventArgs e)
        {
            Server.Transfer("Bridge.aspx");
         }
    }
}