using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarketingCalc.ServiceCaller.PortfolioValuationOperations;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace MarketingCalc.ServiceCaller
{
    public class MarketingCalcServiceCaller
    {

        public void CallService(string portfolio_id)
        {
            PortfolioValuationOperationsClient a = new PortfolioValuationOperationsClient();
            Console.WriteLine(portfolio_id);
            PortfolioValuation[] t = a.PortfolioLevelValuationForMarketing(portfolio_id);
            //Console.WriteLine(t);
            SerializeObjectToXML(t);
        }


        private static string SerializeObjectToXML(object item)
        {
            try
            {
                string xmlText;
                Type objectType = item.GetType();
                XmlSerializer xmlSerializer = new XmlSerializer(objectType);
                MemoryStream memoryStream = new MemoryStream();
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = Formatting.Indented })
                {
                    xmlSerializer.Serialize(xmlTextWriter, item);
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    xmlText = new UTF8Encoding().GetString(memoryStream.ToArray());
                    memoryStream.Dispose();
                   // Console.WriteLine(xmlText);
                    return xmlText;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                return null;
            }
        }

    }
}
