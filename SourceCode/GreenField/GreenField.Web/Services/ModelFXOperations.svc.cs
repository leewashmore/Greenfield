using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.DataContracts;
using GreenField.Web.Helpers;
using System.Data;
using System.Data.SqlClient;

namespace GreenField.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "ModelFXOperations" in code, svc and config file together.
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ModelFXOperations 
    {
        private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

                return dimensionEntity;
            }
        }

        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        /// <summary>
        /// Retrives data for Macro database key annual report
        /// </summary>
        /// <param name="CountryName"></param>
        /// <param name="regionName"></param>
        /// <returns>report data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<MacroDatabaseKeyAnnualReportData> RetrieveMacroDatabaseKeyAnnualReportData(string procName,string countryNameVal, string regionName)
        {
            try
            {
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception();

                List<MacroDatabaseKeyAnnualReportData> result = new List<MacroDatabaseKeyAnnualReportData>();

                DimensionEntitiesService.Entities entity = DimensionEntity;

                DataTable dtMacroData = Methods.GetDataTable(Procs.RETRIEVE_EM_SUMMARY_DATA_REPORT_PER_COUNTRY, Params.COUNTRYNAME, countryNameVal);

                for (int index =0; index < dtMacroData.Rows.Count; index++)
                {
                    MacroDatabaseKeyAnnualReportData data = new MacroDatabaseKeyAnnualReportData();
                    DataRow dtRow = dtMacroData.Rows[index];
                    data.CountryName = Convert.ToString(dtRow[Columns.COUNTRY_NAME]);
                    data.CategoryName = Convert.ToString(dtRow[Columns.CATEGORY_NAME]);
                    data.DisplayType = Convert.ToString(dtRow[Columns.DISPLAY_TYPE]);
                    data.Description = Convert.ToString(dtRow[Columns.DESCRIPTION]);
                    result.Add(data);
                }

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
    }
}
