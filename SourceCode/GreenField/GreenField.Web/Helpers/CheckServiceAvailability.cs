using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.ServiceModel.Description;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using GreenField.DataContracts;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Class for checking availability of O-Data Services
    /// </summary>
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
                ExceptionTrace.LogException(ex);
                return isServiceUp;
            }
        }

        public static void SerializeAndTrace(object result)
        {
            XmlSerializer XmlS = new XmlSerializer(result.GetType());
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = new XmlTextWriter(sw);
            XmlS.Serialize(tw, result);
            Trace.Write(sw.ToString());
        }

        #endregion
    }
}