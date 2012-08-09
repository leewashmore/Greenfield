using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMSResearchDataUpdation.ServiceCaller;
using AIMSResearchDataUpdation.ServiceCaller.DimensionServiceReference;
using System.Xml.Linq;
using System.Globalization;

namespace AIMSResearchDataUpdation
{
    class Task_Security_Baseview
    {
        #region Fields
        private DimensionService dimensionService = new DimensionService();
        private ModelService modelService = new ModelService();
        private DateTime queryStart = DateTime.Now;
        #endregion

        #region Constructor
        public Task_Security_Baseview(string securityId = null, string instrumentId = null, string issuerId = null, string ticker = null, string issueName = null)
        {
            queryStart = DateTime.Now;
            DisplayProgress();
            RecordsUpdated = 0;
            dimensionService.RetrieveSecurityBaseviewData(RetrieveSecurityBaseviewDataCallbackMethod, (result) => { PercentageCompletion = result; }
                , securityId, instrumentId, issuerId, ticker, issueName);
        } 
        #endregion

        #region Properties
        public TimeSpan Period { get; set; }
        public Int32? RecordsUpdated { get; set; }
        public Double PercentageCompletion { get; set; }
        #endregion 

        #region Callback Methods
        private void RetrieveSecurityBaseviewDataCallbackMethod(List<GF_SECURITY_BASEVIEW> result)
        {
            if (result == null || result.Count.Equals(0))
                return;

            XDocument doc = XMLUtils.GetEntityXml<GF_SECURITY_BASEVIEW>(result);
            String xmlScript = doc.ToString();

            modelService.UpdateSecurityBaseviewData(UpdateSecurityBaseviewDataCallbackMethod, xmlScript);
        }

        private void UpdateSecurityBaseviewDataCallbackMethod(Int32? result)
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
