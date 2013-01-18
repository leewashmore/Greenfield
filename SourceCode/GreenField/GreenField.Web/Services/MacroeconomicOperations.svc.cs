using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Web.Security;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using iTextSharp.text;
using iTextSharp.text.pdf;
using GreenField.DAL;
using GreenField.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Helpers;
using GreenField.Web.Helpers.Service_Faults;

namespace GreenField.Web.Services
{
    /// <summary>
    /// Service for Security Reference
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class MacroeconomicOperations
    {
        #region Properties
        /// <summary>
        /// Stores Dimension Service entities
        /// </summary>
        private ExternalResearchEntities externalResearchEntity;
        public ExternalResearchEntities ExternalResearchEntity
        {
            get
            {
                if (externalResearchEntity == null)
                {
                    externalResearchEntity = new ExternalResearchEntities();
                }
                return externalResearchEntity;
            }
            set { externalResearchEntity = value; }
        }

        /// <summary>
        /// Stores service fault exception custom exception resource manager
        /// </summary>
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }
        #endregion
        
        #region Insert Macroeconomic Data
        /// <summary>
        /// Inserts Macroeconomic Data
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public Boolean InsertMacroeconomicData(String countryCode, String field, int year, decimal? value, DateTime updateDate, String updateSource)
        {
            try
            {
                //ICPresentationEntities entity = new ICPresentationEntities();
                //Int32? result = entity.InsertMacroeconomic_Data(countryCode, field, year, value, updateDate, updateSource);

                Macroeconomic_Data macroeconomic_Data = new Macroeconomic_Data();
                macroeconomic_Data.COUNTRY_CODE = countryCode;
                macroeconomic_Data.FIELD = field;
                macroeconomic_Data.YEAR1 = year;
                macroeconomic_Data.VALUE = value;
                macroeconomic_Data.UPDATE_DATE = updateDate;
                macroeconomic_Data.UPDATE_SOURCE = updateSource;
                ExternalResearchEntity.Macroeconomic_Data.AddObject(macroeconomic_Data);
                int? result = ExternalResearchEntity.SaveChanges();
                return result == 1;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        #endregion
    }
}