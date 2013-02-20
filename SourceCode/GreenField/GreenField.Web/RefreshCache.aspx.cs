using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GreenField.Web.Helpers;

namespace GreenField.Web
{
    public partial class RefreshCache : System.Web.UI.Page
    {
        public List<CacheExpiration> CacheExpirations;
        public double x10 = 10.0;
        
        protected void Page_Load(object sender, EventArgs e)
        {
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
            }
        }

        public string GetDataPoint(string key)
        {
            return "10.0";
            /*
            CacheExpiration cacheExpiration = new DefaultCacheProvider().GetExpiration(key);
            
            if (cacheExpiration != null && cacheExpiration.AbsoluteExpiration != null)
                return cacheExpiration.AbsoluteExpiration.Value.Subtract(DateTime.Now).TotalHours;

            return 0.0;
             */
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            new DefaultCacheProvider().InvalidateAll();
        }

        protected void Chart1_Load(object sender, EventArgs e)
        {

        }

    }
}