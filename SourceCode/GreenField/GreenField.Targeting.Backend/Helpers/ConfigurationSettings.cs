using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace GreenField.Targeting.Backend.Helpers
{
    public class ConfigurationSettings
    {
        private const String AimsConnectionStringKey = "Aims";
        public static String AimsConnectionString
        {
            get
            {
                var connectionStringElementOpt = ConfigurationManager.ConnectionStrings[AimsConnectionStringKey];
                if (connectionStringElementOpt == null) throw new ApplicationException("There is no \"" + AimsConnectionStringKey + "\" connection string in the config file.");
                return connectionStringElementOpt.ConnectionString;
            }
        }

        public const String ShouldDropRepositoriesOnEachReloadKey = "ShouldDropRepositoriesOnEachReload";
        public static Boolean ShouldDropRepositoriesOnEachReload
        {
            get
            {
                var something = ConfigurationManager.AppSettings[ShouldDropRepositoriesOnEachReloadKey];
                if (String.IsNullOrWhiteSpace(something)) return false;
                var result = Convert.ToBoolean(something);
                return result;
            }
        }
    }
}