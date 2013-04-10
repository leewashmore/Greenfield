using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Resources;
using System.Text;
using AIMS.Composites.DAL;
using AIMS.Composites.Service.DimensionWebService;
using System.Configuration;

namespace AIMS.Composites.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CompositesOperations" in both code and config file together.
    public class CompositesOperations : ICompositesOperations
    {
        #region PropertyDeclaration

        private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationSettings.AppSettings["DimensionWebService"]));

                return dimensionEntity;
            }
        }

        #endregion

        #region FaultResourceManager
        /*

        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }
         */

        #endregion

        #region CompositesServices

        public List<GetComposites_Result> GetComposites()
        {
            try
            {
                return new AIMS_MainEntities().GetComposites().ToList();
            }
            catch (Exception ex)
            {
                //ExceptionTrace.LogException(ex);
                //string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                //throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
             
                return null;
            }
        }

        public List<GetCompositePortfolios_Result> GetCompositePortfolios(string compositeId)
        {
            try
            {
               return new AIMS_MainEntities().GetCompositePortfolios(compositeId).ToList(); 
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void PopulateCompositeLTHoldings()
        {
            try
            {
                List<GetComposites_Result> composites = GetComposites();
                foreach(GetComposites_Result composite in composites)
                {
                    //Step 1   Retrieve the list of portfolios in the composite that are active (using the new COMPOSITE_MATRIX table).
                    List<GetCompositePortfolios_Result> portfolios = GetCompositePortfolios(composite.COMPOSITE_ID);
                    
                    // question loop and insert of do in one shot
                    portfolios
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion


        /*
        For each composite,
            Step 1   Retrieve the list of portfolios in the composite that are active (using the new COMPOSITE_MATRIX table).
            Step 2   For portfolios returned in Step 1, retrieve all records from GF_PORTFOLIO_LTHOLDINGS.
            Step 3   Delete records when appropriate based on Look_Thru setting in COMPOSITE_MATRIX view.  When Look_Thru <> 'Y', delete records returned from view where PORTFOLIO_ID <> A_PFCHOLDINGS_PORLT
            Step 4   Aggregate remaining records together by the ASEC_SEC_SHORT_NAME, and PORTFOLIO_DATE.
         */

    }
}
