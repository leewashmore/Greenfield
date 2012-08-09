using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMSResearchDataUpdation.ServiceCaller.DimensionServiceReference;

namespace AIMSResearchDataUpdation.ServiceCaller
{
    public interface IDimensionService
    {
        void RetrievePricingBaseviewData(Action<List<GF_PRICING_BASEVIEW>> callback, Action<Double> progress = null, String instrumentId = null,
            String type = null, String ticker = null, string issueName = null, DateTime? beginDate = null, DateTime? endDate = null);

        void RetrieveCountryCurrencyMappingData(Action<List<GF_CTY_CUR>> callback);

        void RetrieveSecurityBaseviewData(Action<List<GF_SECURITY_BASEVIEW>> callback, Action<Double> progress = null, string securityId = null, string instrumentId = null,
            string issuerId = null, string ticker = null, string issueName = null);


    }
}
