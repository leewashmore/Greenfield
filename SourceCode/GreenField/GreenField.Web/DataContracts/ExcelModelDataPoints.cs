using System.Collections.Generic;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    /// <summary>
    /// ExcelModelData: download ExcelModel
    /// </summary>
    public class ExcelModelData
    {
        private ModelReferenceDataPoints _modelReferenceData;
        public ModelReferenceDataPoints ModelReferenceData
        {
            get { return _modelReferenceData; }
            set { _modelReferenceData = value; }
        }

        /// <summary>
        /// ModelUpload DataPoints
        /// </summary>
        private List<DataPointsModelUploadData> _modelUploadDataPoints;
        public List<DataPointsModelUploadData> ModelUploadDataPoints
        {
            get
            {
                return _modelUploadDataPoints;
            }
            set
            {
                _modelUploadDataPoints = value;
            }
        }

        /// <summary>
        /// List of Available Currencies
        /// </summary>
        private List<string> _currencies;
        public List<string> Currencies
        {
            get { return _currencies; }
            set { _currencies = value; }
        }

        /// <summary>
        /// List of Commodities
        /// </summary>
        private List<string> _commodities;
        public List<string> Commodities
        {
            get { return _commodities; }
            set { _commodities = value; }
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
        private string _issuerId;
        public string IssuerId
        {
            get
            {
                return _issuerId;
            }
            set
            {
                _issuerId = value;
            }
        }

        /// <summary>
        /// Issue name
        /// </summary>
        private string _issuerName;
        public string IssuerName
        {
            get { return _issuerName; }
            set { _issuerName = value; }
        }

        /// <summary>
        /// COA type
        /// </summary>
        private string _COATypes;
        public string COATypes
        {
            get { return _COATypes; }
            set { _COATypes = value; }
        }

        /// <summary>
        /// List of Currencies
        /// </summary>
        private List<string> _currencies;
        public List<string> Currencies
        {
            get
            {
                if (_currencies == null)
                    _currencies = new List<string>();
                return _currencies;
            }
            set { _currencies = value; }
        }

        /// <summary>
        /// Units: units/millions/thousands/billions
        /// </summary>
        private List<string> _units;
        public List<string> Units
        {
            get { return _units; }
            set { _units = value; }
        }

        /// <summary>
        /// Q1-Override
        /// </summary>
        private object _Q1Override;
        public object Q1Override
        {
            get { return _Q1Override; }
            set { _Q1Override = value; }
        }

        /// <summary>
        /// Q2-Override
        /// </summary>
        private object _Q2Override;
        public object Q2Override
        {
            get { return _Q2Override; }
            set { _Q2Override = value; }
        }

        /// <summary>
        /// Q3-Override
        /// </summary>
        private object _Q3Override;
        public object Q3Override
        {
            get { return _Q3Override; }
            set { _Q3Override = value; }
        }

        /// <summary>
        /// Q4-Override
        /// </summary>
        private object _Q4Override;
        public object Q4Override
        {
            get { return _Q4Override; }
            set { _Q4Override = value; }
        }

    }

}