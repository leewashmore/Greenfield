using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TopDown.Web.Helpers
{
    public class Settings
    {
		private Settings()
		{
		}
        
		public String ConnectionString { get; set; }
        
        public static Settings CreateFromConfiguration()
        {
			var aimsConnectionElement = ConfigurationManager.ConnectionStrings["Aims"];
			if (aimsConnectionElement == null) throw new ApplicationException("There is no entry named \"Aims\" in the connectionStrings section of the configuration file.");
            var settings = new Settings
            {
                ConnectionString = aimsConnectionElement.ConnectionString
            };
            return settings;
        }
    }
}