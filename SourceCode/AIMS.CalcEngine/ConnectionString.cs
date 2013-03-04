using System;
using System.Configuration;

namespace AIMS.CalcEngine
{
    public class ConnectionString
    {
        private const String AimsMainConnectionStringKey = "AimsMain";

        public static String AimsMainConnectionString
        {
            get
            {
                var connectionStringElementOpt =
                    ConfigurationManager.ConnectionStrings[AimsMainConnectionStringKey];

                if (connectionStringElementOpt == null)
                    throw new ApplicationException("There is no \"" + AimsMainConnectionStringKey +
                                                   "\" connection string in the config file.");

                return connectionStringElementOpt.ConnectionString;
            }
        }
    }
}