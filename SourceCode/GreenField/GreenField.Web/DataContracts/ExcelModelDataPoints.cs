using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    
    public class ExcelModelData
    {
        private ModelReferenceDataPoints _modelReferenceData;
        public ModelReferenceDataPoints ModelReferenceData
        {
            get { return _modelReferenceData; }
            set { _modelReferenceData = value; }
        }

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

        private List<string> _currencies;
        public List<string> Currencies
        {
            get { return _currencies; }
            set { _currencies = value; }
        }

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
        private string _issuerId;
        public string IssuerId
        {
            get { return _issuerId; }
            set { _issuerId = value; }
        }

        private string _issuerName;
        public string IssuerName
        {
            get { return _issuerName; }
            set { _issuerName = value; }
        }

        private string _COATypes;
        public string COATypes
        {
            get { return _COATypes; }
            set { _COATypes = value; }
        }

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

        private List<string> _units;
        public List<string> Units
        {
            get { return _units; }
            set { _units = value; }
        }

        private object _Q1Override;
        public object Q1Override
        {
            get { return _Q1Override; }
            set { _Q1Override = value; }
        }

        private object _Q2Override;
        public object Q2Override
        {
            get { return _Q2Override; }
            set { _Q2Override = value; }
        }

        private object _Q3Override;
        public object Q3Override
        {
            get { return _Q3Override; }
            set { _Q3Override = value; }
        }

        private object _Q4Override;
        public object Q4Override
        {
            get { return _Q4Override; }
            set { _Q4Override = value; }
        }

    }

}