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
        private string _COAType;
        public string COAType
        {
            get { return _COAType; }
            set { _COAType = value; }
        }

        private string  _dataDescription;
        public string  DataDescription
        {
            get { return _dataDescription; }
            set { _dataDescription = value; }
        }

        private object _yearOne;
        public object YearOne
        {
            get { return _yearOne; }
            set { _yearOne = value; }
        }

        private object _yearTwo;
        public object YearTwo
        {
            get { return _yearTwo; }
            set { _yearTwo = value; }
        }

        private object _yearThree;
        public object YearThree
        {
            get { return _yearThree; }
            set { _yearThree = value; }
        }

        private object _yearFour;
        public object YearFour
        {
            get { return _yearFour; }
            set { _yearFour = value; }
        }

        private object _yearFive;
        public object YearFive
        {
            get { return _yearFive; }
            set { _yearFive = value; }
        }

        private object _yearSix;
        public object YearSix
        {
            get { return _yearSix; }
            set { _yearSix = value; }
        }
    }
}