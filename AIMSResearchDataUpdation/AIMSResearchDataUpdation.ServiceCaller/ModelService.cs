using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIMSResearchDataUpdation.DAL;

namespace AIMSResearchDataUpdation.ServiceCaller
{
    public class ModelService : IModelService
    {
        private AIMSEntities dataEntity = new AIMSEntities();

        public void UpdatePricingBaseviewData(Action<Int32?> callback, string xmlScript)
        {
            Int32? result = dataEntity.usp_UpdateGFPricingBaseview(xmlScript).FirstOrDefault();
            if (result == null)
            {
                Console.Write("\rBreak");
            }
            callback(result);
        }

        public void UpdateSecurityBaseviewData(Action<int?> callback, string xmlScript)
        {
            Int32? result = dataEntity.usp_UpdateGFSecurityBaseview(xmlScript).FirstOrDefault();
            callback(result);
        }
    }
}
