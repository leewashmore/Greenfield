using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.DataContracts
{
    public class ExcelModelUpload
    {
        /// <summary>
        /// Collection of Type ExcelModelUploadSheet
        /// </summary>
        private List<ExcelModelUploadSheet> _uploadSheetData;
        public List<ExcelModelUploadSheet> UploadSheetData
        {
            get
            {
                return _uploadSheetData; 
            }
            set { _uploadSheetData = value; }
        }
    }

    /// <summary>
    /// Class used for ExcelModelUpload Sheet
    /// </summary>
    public class ExcelModelUploadSheet
    {
        /// <summary>
        /// COA type
        /// </summary>
        private string _COAType;
        public string COAType
        {
            get { return _COAType; }
            set { _COAType = value; }
        }

        /// <summary>
        /// Data Description
        /// </summary>
        private string  _dataDescription;
        public string  DataDescription
        {
            get { return _dataDescription; }
            set { _dataDescription = value; }
        }

        /// <summary>
        /// Year One Data
        /// </summary>
        private object _yearOne;
        public object YearOne
        {
            get { return _yearOne; }
            set { _yearOne = value; }
        }

        /// <summary>
        /// Year Two Data
        /// </summary>
        private object _yearTwo;
        public object YearTwo
        {
            get { return _yearTwo; }
            set { _yearTwo = value; }
        }

        /// <summary>
        /// Year Three Data
        /// </summary>
        private object _yearThree;
        public object YearThree
        {
            get { return _yearThree; }
            set { _yearThree = value; }
        }

        /// <summary>
        /// Year Four Data
        /// </summary>
        private object _yearFour;
        public object YearFour
        {
            get { return _yearFour; }
            set { _yearFour = value; }
        }

        /// <summary>
        /// Year Five Data
        /// </summary>
        private object _yearFive;
        public object YearFive
        {
            get { return _yearFive; }
            set { _yearFive = value; }
        }

        /// <summary>
        /// Year Six Data
        /// </summary>
        private object _yearSix;
        public object YearSix
        {
            get { return _yearSix; }
            set { _yearSix = value; }
        }
    }

    public class ExcelModelDataUpload
    {
        /// <summary>
        /// COA
        /// </summary>
        private string _COA;
        public string COA
        {
            get { return _COA; }
            set { _COA = value; }
        }

        /// <summary>
        /// Description
        /// </summary>
        private string _descritption;
        public string Description
        {
            get { return _descritption; }
            set { _descritption = value; }
        }

        /// <summary>
        /// PeriodYear Value
        /// </summary>
        private string year;
        public string Year
        {
            get { return year; }
            set { year = value; }
        }

        /// <summary>
        /// Amount Value
        /// </summary>
        private object _amount;
        public object Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }
        
    }
}