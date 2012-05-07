using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.Configuration;

namespace GreenField.Web.Helpers
{
    public static class CheckServiceAvailability
    {
        #region ServiceAvailability

        public static bool ServiceAvailability()
        {
            bool isServiceUp = true;
            try
            {
                MetadataExchangeClient client = new MetadataExchangeClient(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]), MetadataExchangeClientMode.HttpGet);
                MetadataSet metaData = client.GetMetadata();
                return isServiceUp;
            }
            catch (Exception)
            {
                isServiceUp = false;
                return isServiceUp;
            }
        }

        #endregion
    }
}