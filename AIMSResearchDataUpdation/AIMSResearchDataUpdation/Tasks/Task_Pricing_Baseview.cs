using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMSResearchDataUpdation.ServiceCaller;
using AIMSResearchDataUpdation.ServiceCaller.DimensionServiceReference;
using System.Xml.Linq;
using System.Reflection;
using System.Globalization;

namespace AIMSResearchDataUpdation
{
    public class Task_Pricing_Baseview
    {
        #region Fields
        private DimensionService dimensionService = new DimensionService();
        private ModelService modelService = new ModelService();
        private DateTime queryStart = DateTime.Now; 
        #endregion

        #region Properties
        public TimeSpan Period { get; set; }
        public Int32? RecordsUpdated { get; set; }         
        public Double PercentageCompletion { get; set; }
        #endregion

        #region Constructor
        public Task_Pricing_Baseview(string instrumentId = null, string type = null, string ticker = null
            , string issueName = null, DateTime? beginDate = null, DateTime? endDate = null)
        {
            queryStart = DateTime.Now;            
            DisplayProgress();
            RecordsUpdated = 0;
            dimensionService.RetrievePricingBaseviewData(RetrievePricingBaseviewDataCallbackMethod, (result) => { PercentageCompletion = result; }
                , instrumentId, type, ticker, issueName, beginDate, endDate);
        } 
        #endregion

        #region Callback methods
        private void RetrievePricingBaseviewDataCallbackMethod(List<GF_PRICING_BASEVIEW> result)
        {
            if (result == null || result.Count.Equals(0))
                return;

            XDocument doc = XMLUtils.GetEntityXml<GF_PRICING_BASEVIEW>(result);
            String xmlScript = doc.ToString();
            modelService.UpdatePricingBaseviewData(UpdatePricingBaseviewDataCallbackMethod, xmlScript);
        }

        private void UpdatePricingBaseviewDataCallbackMethod(Int32? result)
        {
            RecordsUpdated = RecordsUpdated + result;
            DisplayProgress();
        }

        #endregion

        #region Helper Methods
        private void DisplayProgress()
        {
            Period = DateTime.Now - queryStart;
            Console.Write("\rSecurity data fetch - {0}% | {1} | Records {2}"
                , Math.Round(PercentageCompletion, 2).ToString("00.00", CultureInfo.InvariantCulture)
                , Period.ToString(), (RecordsUpdated == null ? "None" : RecordsUpdated.ToString()));
        }
        #endregion
    }
}
