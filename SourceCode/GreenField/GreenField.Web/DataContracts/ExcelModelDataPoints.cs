using System.Collections.Generic;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    /// <summary>
    /// ExcelModelData: download ExcelModel
    /// </summary>
    public class ExcelModelData
    {
        private ModelReferenceDataPoints modelReferenceData;
        public ModelReferenceDataPoints ModelReferenceData
        {
            get { return modelReferenceData; }
            set { modelReferenceData = value; }
        }

        /// <summary>
        /// ModelUpload DataPoints
        /// </summary>
        private List<DataPointsModelUploadData> modelUploadDataPoints;
        public List<DataPointsModelUploadData> ModelUploadDataPoints
        {
            get
            {
                return modelUploadDataPoints;
            }
            set
            {
                modelUploadDataPoints = value;
            }
        }

        /// <summary>
        /// List of Available Currencies
        /// </summary>
        private List<string> currencies;
        public List<string> Currencies
        {
            get { return currencies; }
            set { currencies = value; }
        }

        /// <summary>
        /// List of Commodities
        /// </summary>
        private List<string> commodities;
        public List<string> Commodities
        {
            get { return commodities; }
            set { commodities = value; }
        }

        /// <summary>
        /// Reuters Data
        /// </summary>
        private List<FinancialStatementDataModels> reutersData;
        public List<FinancialStatementDataModels> ReutersData
        {
            get { return reutersData; }
            set { reutersData = value; }
        }

        /// <summary>
        /// Currency for Reuters Sheet
        /// </summary>
        private string currencyReuters;   
        public string CurrencyReuters
        {
            get { return currencyReuters; }
            set { currencyReuters = value; }
        }

        /// <summary>
        /// Consensus Estimate Data
        /// </summary>
        private List<ModelConsensusEstimatesData> consensusEstimateData;
        public List<ModelConsensusEstimatesData> ConsensusEstimateData
        {
            get { return consensusEstimateData; }
            set { consensusEstimateData = value; }
        }
        

    }

    /// <summary>
    /// Model Class for ExcelModel- ModelReference
    /// </summary>
    public class ModelReferenceDataPoints
    {
        /// <summary>
        /// IssuerId
        /// </summary>
        private string issuerId;
        public string IssuerId
        {
            get
            {
                return issuerId;
            }
            set
            {
                issuerId = value;
            }
        }

        /// <summary>
        /// Issue name
        /// </summary>
        private string issuerName;
        public string IssuerName
        {
            get { return issuerName; }
            set { issuerName = value; }
        }

        /// <summary>
        /// COA type
        /// </summary>
        private string COATypesp;
        public string COATypes
        {
            get { return COATypesp; }
            set { COATypesp = value; }
        }

        /// <summary>
        /// List of Currencies
        /// </summary>
        private List<string> currencies;
        public List<string> Currencies
        {
            get
            {
                if (currencies == null)
                    currencies = new List<string>();
                return currencies;
            }
            set { currencies = value; }
        }

        /// <summary>
        /// Units: units/millions/thousands/billions
        /// </summary>
        private List<string> units;
        public List<string> Units
        {
            get { return units; }
            set { units = value; }
        }

        /// <summary>
        /// Q1-Override
        /// </summary>
        private object Q1Overridep;
        public object Q1Override
        {
            get { return Q1Overridep; }
            set { Q1Overridep = value; }
        }

        /// <summary>
        /// Q2-Override
        /// </summary>
        private object Q2Overridep;
        public object Q2Override
        {
            get { return Q2Overridep; }
            set { Q2Overridep = value; }
        }

        /// <summary>
        /// Q3-Override
        /// </summary>
        private object Q3Overridep;
        public object Q3Override
        {
            get { return Q3Overridep; }
            set { Q3Overridep = value; }
        }

        /// <summary>
        /// Q4-Override
        /// </summary>
        private object Q4Overridep;
        public object Q4Override
        {
            get { return Q4Overridep; }
            set { Q4Overridep = value; }
        }

    }

}