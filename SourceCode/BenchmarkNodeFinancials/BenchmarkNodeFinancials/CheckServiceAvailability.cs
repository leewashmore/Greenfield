using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.Configuration;

namespace BenchmarkNodeFinancials
{
    public static class CheckServiceAvailability
    {
            #region ServiceAvailability
            /// <summary>
            /// Method to Check Availability of WCF O Data Service
            /// </summary>
            /// <returns>returns True if Service is Up, else False</returns>
            public static bool ServiceAvailability()
            {
                bool isServiceUp = true;
                try
                {
                    MetadataExchangeClient client = new MetadataExchangeClient(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]), MetadataExchangeClientMode.HttpGet);
                    MetadataSet metaData = client.GetMetadata();
                    return isServiceUp;
                }
                catch (Exception ex)
                {
                    isServiceUp = false;
                   // ExceptionTrace.LogException(ex);
                    Console.WriteLine(ex);
                    return isServiceUp;
                }
            }
            #endregion       
    }
}
