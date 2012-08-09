using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMSResearchDataUpdation.DAL;


namespace AIMSResearchDataUpdation.ServiceCaller
{
    public interface IModelService
    {
        void UpdatePricingBaseviewData(Action<Int32?> callback, String xmlScript);

        void UpdateSecurityBaseviewData(Action<Int32?> callback, String xmlScript);
    }
}
