using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.Drawing;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Reading an Excel File
    /// </summary>
    public class ReadOpenXMLModel
    {
        #region PropertyDeclaration

        #region Common

        /// <summary>
        /// Dimension Service Entity
        /// </summary>
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

        /// <summary>
        /// Entity of ExternalResearchDataModel
        /// </summary>
        private ExternalResearchEntities _externalResearchEntity;
        public ExternalResearchEntities ExternalResearchEntity
        {
            get
            {
                if (_externalResearchEntity == null)
                {
                    _externalResearchEntity = new ExternalResearchEntities();
                }
                return _externalResearchEntity;
            }
            set { _externalResearchEntity = value; }
        }

        /// <summary>
        /// Username
        /// </summary>
        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// Role of the User
        /// </summary>
        private string _userRole;
        public string UserRole
        {
            get { return _userRole; }
            set { _userRole = value; }
        }

        /// <summary>
        /// Root-Source of Data
        /// </summary>
        private string _rootSource = "";
        public string RootSource
        {
            get { return _rootSource; }
            set { _rootSource = value; }
        }

        /// <summary>
        /// Time-Stamp to be put in Each Data.
        /// </summary>
        private DateTime _timeStamp = DateTime.Now;
        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
            }
        }

        /// <summary>
        /// List for REF values
        /// </summary>
        private List<string> _REF;
        public List<string> REF
        {
            get
            {
                if (_REF == null)
                {
                    _REF = new List<string>();
                }
                return _REF;
            }
            set { _REF = value; }
        }

        private int _numberOfYears;
        public int NumberOfYears
        {
            get { return _numberOfYears; }
            set { _numberOfYears = value; }
        }

        private string _uniqueRefNumber;
        public string UniqueRefNumber
        {
            get { return _uniqueRefNumber; }
            set { _uniqueRefNumber = value; }
        }

        private Dictionary<string, string> _recordYearRefNo;
        public Dictionary<string, string> RecordYearRefNo
        {
            get
            {
                if (_recordYearRefNo == null)
                {
                    _recordYearRefNo = new Dictionary<string, string>();
                }
                return _recordYearRefNo;
            }
            set { _recordYearRefNo = value; }
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        private ExcelModelUploadSheet _uploadData;
        public ExcelModelUploadSheet UploadData
        {
            get { return _uploadData; }
            set { _uploadData = value; }
        }

        #region ModelUpload

        private IEnumerable<Sheet> _sheetModelUpload;
        public IEnumerable<Sheet> SheetModelUpload
        {
            get { return _sheetModelUpload; }
            set { _sheetModelUpload = value; }
        }

        private WorksheetPart _workSheetPartModelUpload;
        public WorksheetPart WorkSheetPartModelUpload
        {
            get { return _workSheetPartModelUpload; }
            set { _workSheetPartModelUpload = value; }
        }

        private SheetData _sheetDataModelUpload;
        public SheetData SheetDataModelUpload
        {
            get { return _sheetDataModelUpload; }
            set { _sheetDataModelUpload = value; }
        }

        private List<DataPointsModelUploadData> _COACodes;
        public List<DataPointsModelUploadData> COACodes
        {
            get
            {
                if (_COACodes == null)
                {
                    _COACodes = new List<DataPointsModelUploadData>();
                }
                return _COACodes;
            }
            set
            {
                _COACodes = value;
            }
        }

        private List<ExcelModelDataUpload> _modelUploadData;
        public List<ExcelModelDataUpload> ModelUploadData
        {
            get { return _modelUploadData; }
            set { _modelUploadData = value; }
        }


        #region FirstSixRowsData

        private Dictionary<string, int?> _yearList;
        public Dictionary<string, int?> YearList
        {
            get
            {
                if (_yearList == null)
                {
                    _yearList = new Dictionary<string, int?>();
                }
                return _yearList;
            }
            set { _yearList = value; }
        }

        private Dictionary<string, DateTime?> _periodEndDate;
        public Dictionary<string, DateTime?> PeriodEndDate
        {
            get { return _periodEndDate; }
            set { _periodEndDate = value; }
        }

        private Dictionary<string, int?> _periodLength;
        public Dictionary<string, int?> PeriodLength
        {
            get { return _periodLength; }
            set { _periodLength = value; }
        }

        private Dictionary<string, string> _actualOverride;
        public Dictionary<string, string> ActualOverride
        {
            get { return _actualOverride; }
            set { _actualOverride = value; }
        }

        private Dictionary<string, string> _commodityMeasure;
        public Dictionary<string, string> CommodityMeasure
        {
            get { return _commodityMeasure; }
            set { _commodityMeasure = value; }
        }

        private Dictionary<string, string> _commodityForecastUsed;
        public Dictionary<string, string> CommodityForecastUsed
        {
            get { return _commodityForecastUsed; }
            set { _commodityForecastUsed = value; }
        }

        #endregion

        #region YearDataValidate

        private bool _year1 = true;
        public bool Year1
        {
            get { return _year1 = true; }
            set { _year1 = value; }
        }

        private bool _year2 = true;
        public bool Year2
        {
            get { return _year2 = true; }
            set { _year2 = value; }
        }

        private bool _year3 = true;
        public bool Year3
        {
            get { return _year3 = true; }
            set { _year3 = value; }
        }

        private bool _year4 = true;
        public bool Year4
        {
            get { return _year4 = true; }
            set { _year4 = value; }
        }

        private bool _year5 = true;
        public bool Year5
        {
            get { return _year5 = true; }
            set { _year5 = value; }
        }

        private bool _year6 = true;
        public bool Year6
        {
            get { return _year6 = true; }
            set { _year6 = value; }
        }

        /// <summary>
        /// Years for Which Data is to be loaded
        /// </summary>
        private Dictionary<string, int?> _yearsToLoad;
        public Dictionary<string, int?> YearsToLoad
        {
            get
            {
                if (_yearsToLoad == null)
                {
                    _yearsToLoad = new Dictionary<string, int?>();
                }
                return _yearsToLoad;
            }
            set { _yearsToLoad = value; }
        }

        #endregion

        #endregion

        #region ModelReference

        private IEnumerable<Sheet> _sheetModelReference;
        public IEnumerable<Sheet> SheetModelReference
        {
            get { return _sheetModelReference; }
            set { _sheetModelReference = value; }
        }

        private WorksheetPart _workSheetPartModelReference;
        public WorksheetPart WorkSheetPartModelReference
        {
            get { return _workSheetPartModelReference; }
            set { _workSheetPartModelReference = value; }
        }

        private SheetData _sheetDataModelReference;
        public SheetData SheetDataModelReference
        {
            get { return _sheetDataModelReference; }
            set { _sheetDataModelReference = value; }
        }

        #region DataValues

        private ModelReferenceDataPoints _modelReferenceData;
        public ModelReferenceDataPoints ModelReferenceData
        {
            get { return _modelReferenceData; }
            set { _modelReferenceData = value; }
        }

        #endregion


        #endregion

        #endregion

        #region PublicMethods

        public void ReadExcelData()
        {
            try
            {
                using (SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open("D:\\TestQA.xls", true))
                {
                    WorkbookPart workbookPart = myWorkbook.WorkbookPart;
                    bool checkWorkSheetExists = CheckSheetsExist(workbookPart);

                    if (!checkWorkSheetExists)
                    {
                        //To Do
                        //Write Code to Exit when Sheets are not Present
                    }

                    string modelUploadSheetId = SheetModelUpload.First().Id.Value;
                    string modelReferenceSheetId = SheetModelReference.First().Id.Value;
                    WorkSheetPartModelUpload = (WorksheetPart)myWorkbook.WorkbookPart.GetPartById(modelUploadSheetId);
                    WorkSheetPartModelReference = (WorksheetPart)myWorkbook.WorkbookPart.GetPartById(modelReferenceSheetId);

                    ReadSheetData(WorkSheetPartModelUpload, WorkSheetPartModelReference);
                    NumberOfYears = FindNumberOfYearsToLoad(SheetDataModelUpload, workbookPart);

                    ReadModelReferenceSheetData(workbookPart);
                    ReadModelUploadSheetData(workbookPart);
                    ValidateSheetData();
                    DBDeleteOperations();
                    DBInsertOperations();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Create SheetData for both the Excel Sheets
        /// </summary>
        /// <param name="workSheetPartModelUpload"></param>
        /// <param name="workSheetPartModelReference"></param>
        private void ReadSheetData(WorksheetPart workSheetPartModelUpload, WorksheetPart workSheetPartModelReference)
        {
            try
            {
                SheetDataModelUpload = workSheetPartModelUpload.Worksheet.GetFirstChild<SheetData>();
                SheetDataModelReference = workSheetPartModelReference.Worksheet.GetFirstChild<SheetData>();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Read Data from the ModelReferenceSheet
        /// </summary>
        private void ReadModelReferenceSheetData(WorkbookPart workbookPart)
        {
            try
            {
                ReadModelReferenceData(SheetDataModelReference, workbookPart);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Read data from the ModelUploadSheet
        /// </summary>
        private void ReadModelUploadSheetData(WorkbookPart workbookPart)
        {
            try
            {
                ReadModelUploadData(SheetDataModelUpload, workbookPart);
                ReadModelUploadDataFirstRow(SheetDataModelUpload, workbookPart);
                ReadModelUploadDataSecondRow(SheetDataModelUpload, workbookPart);
                ReadModelUploadDataThirdRow(SheetDataModelUpload, workbookPart);
                ReadModelUploadDataFourthRow(SheetDataModelUpload, workbookPart);
                ReadModelUploadDataFifthRow(SheetDataModelUpload, workbookPart);
                ReadModelUploadDataSixthRow(SheetDataModelUpload, workbookPart);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Validate the Data in both the Sheets
        /// </summary>
        private void ValidateSheetData()
        {
            try
            {
                bool modelReferenceDataValid = ValidateModelReferenceData(ModelReferenceData);
                if (!modelReferenceDataValid)
                {

                }
                ValidateModelUploadData();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Database Operations
        /// </summary>
        private void DBDeleteOperations()
        {
            try
            {
                if (ModelReferenceData != null)
                {
                    DeleteInternalStatementRecords(ModelReferenceData.IssuerId, RootSource);
                    DeleteInternalDataRecords(ModelReferenceData.IssuerId, REF);
                    DeleteInternalCommodityAssumptionsRecords(ModelReferenceData.IssuerId, REF);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DBInsertOperations()
        {
            try
            {
                InsertIntoInternalStatementData();
                InsertIntoInternalData();
                InsertIntoInternalCommodityAssumptions();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Insert Data in Internal_Statement
        /// </summary>
        private void InsertIntoInternalStatementData()
        {
            TimeStamp = DateTime.Now;

            int year = 0;
            DateTime periodEndDate;
            string rootSource = UserRole;
            DateTime currentDate = DateTime.Now;
            string overRide = "";
            string amountType = "";
            string currency = ModelReferenceData.Currencies.First();
            int periodLength;

            if (ModelReferenceData != null)
            {
                foreach (var item in YearsToLoad)
                {
                    UniqueRefNumber = Guid.NewGuid().ToString();
                    periodLength = Convert.ToInt32(PeriodLength.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault());
                    periodEndDate = Convert.ToDateTime(PeriodEndDate.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault());
                    overRide = ActualOverride.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault();
                    if (overRide.ToUpper().Trim() == "YES" && currentDate > periodEndDate)
                    {
                        amountType = "ACTUAL";
                    }
                    else
                    {
                        amountType = "ESTIMATE";
                    }
                    year = Convert.ToInt32(item.Value);
                    InsertInternalStatementServiceMethod(ModelReferenceData.IssuerId, UniqueRefNumber, year, RootSource, TimeStamp, periodLength, periodEndDate, ModelReferenceData.Currencies.FirstOrDefault(), amountType);
                    RecordYearRefNo.Add(item.Key, UniqueRefNumber);
                }
            }
        }

        /// <summary>
        /// Insert Data into Internal_Data
        /// </summary>
        private void InsertIntoInternalData()
        {
            try
            {
                string issuerId = ModelReferenceData.IssuerId;
                string periodType = "A";
                decimal? amount;
                string convertFlag;
                foreach (var item in YearsToLoad)
                {
                    UniqueRefNumber = RecordYearRefNo.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault();
                    foreach (ExcelModelDataUpload data in ModelUploadData.Where(a => a.Year == item.Key).ToList())
                    {
                        decimal value;
                        if (Decimal.TryParse(data.Amount as string, out value))
                        {
                            amount = value;
                        }
                        else
                        {
                            amount = null;
                        }

                        if (amount != null)
                        {
                            convertFlag = Convert.ToString(COACodes.Where(a => a.COA == data.COA).Select(a => a.CONVERT_FLAG).FirstOrDefault());
                            if (convertFlag != null)
                            {
                                if (convertFlag.ToUpper().Trim() == "Y")
                                {
                                    amount = ConvertAmount(ModelReferenceData.Units.First(), Convert.ToDecimal(amount));
                                }
                            }
                            InsertInternalDataRecords(issuerId, UniqueRefNumber, periodType, data.COA, amount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        private void CheckInternalIssuer()
        {
            string issuerId = ModelReferenceData.IssuerId;
            string coa = ModelReferenceData.COATypes;
            string rootSource = UserRole;

            INTERNAL_ISSUER issuerData = FetchInternalIssuerData(issuerId);

            if (issuerData != null)
            {
                if (UserRole == "PRIMARY")
                { 
                
                    }
            }

        }

        /// <summary>
        /// Insert Data into Internal_Commodity_Assumptions
        /// </summary>
        private void InsertIntoInternalCommodityAssumptions()
        {
            string issuerId = ModelReferenceData.IssuerId;
            string commodityId = "";

            foreach (var item in YearsToLoad)
            {
                UniqueRefNumber = RecordYearRefNo.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault();
                commodityId = CommodityMeasure.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault();

                decimal value;
                object forecastValue = CommodityForecastUsed.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault();
                if (commodityId != null && forecastValue != null)
                {
                    if (Decimal.TryParse(forecastValue as string, out value))
                    {
                        InsertInternalCommodityAssumptionsData(issuerId, UniqueRefNumber, commodityId, value);
                    }
                }
            }
        }

        #endregion

        #region Common Methods

        public bool CheckSheetsExist(WorkbookPart workbookPart)
        {
            IEnumerable<Sheet> sheetModelUpload = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Model Upload");
            SheetModelUpload = sheetModelUpload;
            if (sheetModelUpload.Count() == 0)
            {
                return false;
            }
            IEnumerable<Sheet> sheetModelReference = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Model Reference");
            SheetModelReference = sheetModelReference;
            if (sheetModelReference.Count() == 0)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region ModelReference

        #region ReadExcelSheet

        /// <summary>
        /// Validate Integrity of ModelReferenceSheet
        /// </summary>
        /// <param name="sheetData">SheetData for Current WorkSheet</param>
        /// <returns>True/False implying whether Sheets is Met or Not</returns>
        private void ReadModelReferenceData(SheetData sheetData, WorkbookPart workbookPart)
        {
            int i = 0;
            string[] rowData = new string[18];

            foreach (Row r in sheetData.Elements<Row>())
            {
                foreach (Cell c in r.Elements<Cell>())
                {
                    if (c != null)
                    {
                        rowData[i] = c.InnerText;
                        if (c.DataType != null)
                        {
                            if (c.DataType == CellValues.SharedString)
                            {
                                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                if (stringTable != null)
                                {
                                    rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i])).InnerText;
                                }
                            }
                        }
                    }
                    else
                    {
                        rowData[i] = "";
                    }
                    ++i;
                }
            }

            ModelReferenceData = new ModelReferenceDataPoints();
            ModelReferenceData.IssuerId = rowData[1];
            ModelReferenceData.IssuerName = rowData[3];
            ModelReferenceData.COATypes = rowData[5];
            ModelReferenceData.Currencies = new List<string>() { rowData[7] };
            ModelReferenceData.Units = new List<string>() { rowData[9] };
            ModelReferenceData.Q1Override = rowData[11] as string;
            ModelReferenceData.Q2Override = rowData[13] as string;
            ModelReferenceData.Q3Override = rowData[15] as string;
            ModelReferenceData.Q4Override = rowData[17] as string;
        }

        #endregion

        #region ValiditityCheckers

        /// <summary>
        /// Validate Data in ModelReferenceSheet
        /// </summary>
        /// <param name="ModelReferenceData"></param>
        /// <returns></returns>
        private bool ValidateModelReferenceData(ModelReferenceDataPoints ModelReferenceData)
        {
            if (ModelReferenceData.COATypes == null || ModelReferenceData.COATypes.Trim() == "" ||
           ModelReferenceData.Currencies == null || ModelReferenceData.Currencies.FirstOrDefault().Trim() == "")
            {
                return false;
            }

            bool issuerIdValidity = CheckIssuerIdExist(ModelReferenceData.IssuerId);
            if (!issuerIdValidity)
            {
                return false;
            }

            bool COAValidity = CheckCOATypeValidity(ModelReferenceData.IssuerId, ModelReferenceData.COATypes);
            if (!COAValidity)
            {
                return false;
            }

            bool checkOverride = CheckOverrideValues();
            if (!checkOverride)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check If Issuer Exists
        /// </summary>
        /// <param name="issuerId">Issuer Id</param>
        /// <returns>Tru/False</returns>
        private bool CheckIssuerIdExist(string issuerId)
        {
            try
            {
                GF_SECURITY_BASEVIEW data = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ISSUER_ID == issuerId).FirstOrDefault();
                if (data == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Check if the COA type Entered is Valid or Not
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="COA"></param>
        /// <returns></returns>
        private bool CheckCOATypeValidity(string issuerId, string COA)
        {
            string dbCOA;
            INTERNAL_ISSUER dbResult = new INTERNAL_ISSUER();
            dbResult = ExternalResearchEntity.RetrieveCOAType(issuerId).FirstOrDefault();

            if (dbResult == null)
            {
                return true;
            }
            dbCOA = dbResult.COA_TYPE;

            if (dbCOA == null)
            {
                return true;
            }
            if (dbCOA == COA)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check the Override Values
        /// </summary>
        /// <returns></returns>
        private bool CheckOverrideValues()
        {
            decimal value;
            if (Decimal.TryParse(ModelReferenceData.Q1Override as string, out  value))
            {
                ModelReferenceData.Q1Override = value;
            }
            else
            {
                return false;
            }

            if (Decimal.TryParse(ModelReferenceData.Q2Override as string, out  value))
            {
                ModelReferenceData.Q2Override = value;
            }
            else
            {
                return false;
            }

            if (Decimal.TryParse(ModelReferenceData.Q3Override as string, out  value))
            {
                ModelReferenceData.Q3Override = value;
            }
            else
            {
                return false;
            }

            if (Decimal.TryParse(ModelReferenceData.Q4Override as string, out  value))
            {
                ModelReferenceData.Q4Override = value;
            }
            else
            {
                return false;
            }
            decimal sumOverride = (decimal)ModelReferenceData.Q1Override + (decimal)ModelReferenceData.Q2Override +
                (decimal)ModelReferenceData.Q3Override + (decimal)ModelReferenceData.Q4Override;
            if (sumOverride == 100M)
            {
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #region ModelUploadSheet

        #region ReadExcelSheet

        /// <summary>
        /// Read Data for ExcelModel- ModelUpload Sheet
        /// </summary>
        /// <param name="sheetData">SheetData of the Sheet</param>
        private void ReadModelUploadData(SheetData sheetData, WorkbookPart workbookPart)
        {
            int i = 0;
            int j = 0;
            string[] rowData = new string[NumberOfYears + 2];
            string coaType = "";
            string description = "";

            List<ExcelModelDataUpload> excelModelData = new List<ExcelModelDataUpload>();
            foreach (Row r in sheetData.Elements<Row>())
            {
                rowData = new string[NumberOfYears + 2];
                if (j > 5)
                {
                    i = 0;
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null)
                        {
                            rowData[i] = c.InnerText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                    if (stringTable != null)
                                    {
                                        rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i])).InnerText;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rowData[i] = "";
                        }
                        i++;
                    }

                    coaType = rowData[0];
                    description = rowData[1];
                    ExcelModelDataUpload modelData = new ExcelModelDataUpload();
                    int count = rowData.Count();
                    int k = 1;
                    foreach (string item in rowData)
                    {
                        if (k > 2)
                        {
                            modelData = new ExcelModelDataUpload();
                            modelData.COA = coaType;
                            modelData.Description = description;
                            modelData.Year = "Y" + (k - 2).ToString();
                            modelData.Amount = item;
                            excelModelData.Add(modelData);
                        }
                        k++;
                    }
                }
                j++;
            }
            ModelUploadData = excelModelData;
        }

        /// <summary>
        /// Find Number of years to Load Data for
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        /// <returns></returns>
        private int FindNumberOfYearsToLoad(SheetData sheetData, WorkbookPart workbookPart)
        {
            int count = 0;
            int j = 1;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 1)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (j > 2)
                        {
                            count++;
                        }
                        j++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Retrieve Years
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataFirstRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            string[] rowData = new string[8];
            int i = 0;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 1)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null)
                        {
                            rowData[i] = c.InnerText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                    if (stringTable != null)
                                    {
                                        rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i])).InnerText;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rowData[i] = "";
                        }
                        ++i;
                    }
                }
            }
            YearList = SetPeriodYears(rowData);
            if (YearList.Count == 0)
            {
                //To-Do
                //Add Exception Details
            }
        }

        /// <summary>
        /// Retrieve Period EndDate
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataSecondRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            object[] rowData = new object[8];
            double data;
            int i = 0;
            int j = 0;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 2)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null && j > 1)
                        {
                            if (Double.TryParse(c.InnerText, out data))
                            {
                                rowData[i] = DateTime.FromOADate(data);
                            }
                            else
                            {
                                rowData[i] = new DateTime(1900, 1, 31);
                            }
                        }
                        else
                        {
                            rowData[i] = new DateTime(1900, 1, 31);
                        }
                        ++i;
                        ++j;
                    }
                }
            }
            PeriodEndDate = SetPeriodEndDate(rowData);
            if (YearList.Count == 0)
            {
                //To-Do
                //Add Exception Details
            }
        }

        /// <summary>
        /// Retrieve Period Length
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataThirdRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            object[] rowData = new object[8];
            int i = 0;
            int j = 0;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 3)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null && j > 1)
                        {
                            rowData[i] = c.InnerText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                    if (stringTable != null)
                                    {
                                        rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i] as string)).InnerText;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rowData[i] = null;
                        }
                        ++i;
                        ++j;
                    }
                }
            }
            PeriodLength = SetPeriodLength(rowData);
            if (YearList.Count == 0)
            {
                //To-Do
                //Add Exception Details
            }
        }

        /// <summary>
        /// Retrieve Actual Override
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataFourthRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            object[] rowData = new object[8];
            int i = 0;
            int j = 0;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 4)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null && j > 1)
                        {
                            rowData[i] = c.InnerText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                    if (stringTable != null)
                                    {
                                        rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i] as string)).InnerText;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rowData[i] = null;
                        }
                        ++i;
                        ++j;
                    }
                }
            }
            ActualOverride = SetOverride(rowData);
            if (YearList.Count == 0)
            {
                //To-Do
                //Add Exception Details
            }
        }

        /// <summary>
        /// Retrieve Commodity Measure
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataFifthRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            object[] rowData = new object[8];
            int i = 0;
            int j = 0;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 5)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null && j > 1)
                        {
                            rowData[i] = c.InnerText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                    if (stringTable != null)
                                    {
                                        rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i] as string)).InnerText;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rowData[i] = null;
                        }
                        ++i;
                        ++j;
                    }
                }
            }
            CommodityMeasure = SetMeasure(rowData);
            if (YearList.Count == 0)
            {
                //To-Do
                //Add Exception Details
            }
        }

        /// <summary>
        /// Retrieve Commodity Forecast Used
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataSixthRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            object[] rowData = new object[8];
            int i = 0;
            int j = 0;
            foreach (Row r in sheetData.Elements<Row>())
            {
                if (r.RowIndex == 6)
                {
                    foreach (Cell c in r.Elements<Cell>())
                    {
                        if (c != null && j > 1)
                        {
                            rowData[i] = c.InnerText;
                            if (c.DataType != null)
                            {
                                if (c.DataType == CellValues.SharedString)
                                {
                                    var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                    if (stringTable != null)
                                    {
                                        rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i] as string)).InnerText;
                                    }
                                }
                            }
                        }
                        else
                        {
                            rowData[i] = null;
                        }
                        ++i;
                        ++j;
                    }
                }
            }
            CommodityForecastUsed = SetCommodityForecast(rowData);
            if (YearList.Count == 0)
            {
                //To-Do
                //Add Exception Details
            }
        }

        /// <summary>
        /// Fill up the Years
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, int?> SetPeriodYears(string[] data)
        {
            Dictionary<string, int?> result = new Dictionary<string, int?>();
            int i = 1;
            int value = 0;
            int j = 1;
            foreach (string item in data)
            {
                if (j > 2)
                {
                    if (item != null)
                    {
                        if (Int32.TryParse(item, out value))
                        {
                            if (value > 1950 || value < 2200)
                            {
                                result.Add("Y" + i, value);
                            }
                            else
                            {
                                result.Add("Y" + i, null);
                            }
                        }
                        else
                        {
                            result.Add("Y" + i, null);
                        }
                    }
                    else
                    {
                        result.Add("Y" + i, null);
                    }
                    i++;
                }
                j++;
            }
            YearsToLoad = result;
            return result;
        }

        /// <summary>
        /// Set the Values of PeriodEndDate
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, DateTime?> SetPeriodEndDate(object[] data)
        {
            Dictionary<string, DateTime?> result = new Dictionary<string, DateTime?>();
            DateTime invalid = new DateTime(1900, 1, 31);
            int i = 1;
            int j = 1;
            foreach (DateTime item in data)
            {
                if (j > 2)
                {
                    result.Add("Y" + i, item);
                    i++;
                }
                j++;
            }
            return result;
        }

        /// <summary>
        /// Set Period Length
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, int?> SetPeriodLength(object[] data)
        {
            Dictionary<string, int?> result = new Dictionary<string, int?>();
            int i = 1;
            int j = 1;
            int value = 0;
            foreach (string item in data)
            {
                if (j > 2)
                {
                    if (item != null)
                    {
                        if (Int32.TryParse(item, out value))
                        {
                            result.Add("Y" + i, value);
                        }
                        else
                        {
                            result.Add("Y" + i, null);
                        }
                    }
                    else
                    {
                        result.Add("Y" + i, null);
                    }
                    i++;
                }
                j++;
            }
            return result;
        }

        /// <summary>
        /// Set ActualOverride
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetOverride(object[] data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            int i = 1;
            int j = 1;
            foreach (string item in data)
            {
                if (j > 2)
                {
                    result.Add("Y" + i, item);
                    i++;
                }
                j++;
            }

            return result;
        }

        /// <summary>
        /// Set ActualOverride
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMeasure(object[] data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            int i = 1;
            int j = 1;
            foreach (string item in data)
            {
                if (j > 2)
                {
                    result.Add("Y" + i, item);
                    i++;
                }
                j++;
            }

            return result;
        }

        /// <summary>
        /// Set ActualOverride
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetCommodityForecast(object[] data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            int i = 1;
            int j = 1;
            foreach (string item in data)
            {
                if (j > 2)
                {
                    result.Add("Y" + i, item);
                    i++;
                }
                j++;
            }
            return result;
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Convert Amount according to Units
        /// </summary>
        /// <param name="units"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private decimal ConvertAmount(string units, decimal amount)
        {
            switch (units)
            {
                case "MILLIONS":
                    return amount;
                case "THOUSANDS":
                    return amount / 1000M;
                case "UNITS":
                    return amount / 1000000M;
                case "BILLIONS":
                    return amount * 1000M;
                default:
                    return amount;
            }
        }

        #endregion

        #region ValidityCheckers

        /// <summary>
        /// Verify COA codes from sheet with those returned from DB
        /// </summary>
        /// <param name="modelUploadData">Model Upload Data read from Sheet</param>
        /// <param name="COAList">COA codes returned from DB</param>
        /// <returns>Result of the match</returns>
        private bool CheckCOACodes(List<ExcelModelDataUpload> modelUploadData, List<DataPointsModelUploadData> COAList)
        {
            bool result = true;
            foreach (DataPointsModelUploadData item in COAList)
            {
                if (modelUploadData.Any(a => a.COA != item.COA))
                {
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Validate Data for Period Years
        /// </summary>
        /// <param name="periodYearData"></param>
        private void ValidatePeriodYears(Dictionary<string, int?> periodYearData)
        {
            if (periodYearData != null)
            {
                List<string> badYears = periodYearData.Where(a => a.Value == null).Select(a => a.Key).ToList();
                foreach (string item in badYears)
                {
                    RemoveBadYearData(item);
                }
            }
        }

        /// <summary>
        /// ValidateEndDates
        /// </summary>
        /// <param name="periodEndDates"></param>
        private void ValidateEndDates(Dictionary<string, DateTime?> periodEndDates)
        {
            if (periodEndDates != null)
            {
                foreach (var item in periodEndDates)
                {
                    if (item.Value != null)
                    {
                        int? yearValue = YearList.Where(a => a.Key == item.Key).Select(a => a.Value).FirstOrDefault();
                        if (yearValue != null)
                        {
                            if (item.Value.Value.Year != yearValue)
                            {
                                RemoveBadYearData(item.Key);
                            }
                        }
                    }
                    else
                    {
                        RemoveBadYearData(item.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Validate the PeriodLength
        /// </summary>
        /// <param name="periodLength"></param>
        private void ValidatePeriodLength(Dictionary<string, int?> periodLength)
        {
            if (periodLength != null)
            {
                foreach (var item in periodLength)
                {
                    if (item.Value != null)
                    {
                        int? length = item.Value;
                        if (!(length >= 1 && length <= 23))
                        {
                            RemoveBadYearData(item.Key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate the values of Reported Override
        /// </summary>
        /// <param name="actualOverride"></param>
        private void ValidateActualOverride(Dictionary<string, string> actualOverride)
        {
            if (actualOverride != null)
            {
                foreach (var item in actualOverride)
                {
                    if (item.Value == null)
                    {
                        RemoveBadYearData(item.Key);
                    }
                    if (item.Value.ToUpper() != "YES" && item.Value.ToUpper() != "NO")
                    {
                        RemoveBadYearData(item.Key);
                    }
                }
            }
        }

        /// <summary>
        /// Remove Data for Year which is Corrupt
        /// </summary>
        /// <param name="yearName"></param>
        private void RemoveBadYearData(string yearName)
        {
            if (yearName != null)
            {
                bool exists = YearsToLoad.Any(a => a.Key == yearName);
                if (exists)
                {
                    YearsToLoad.Remove(yearName);
                }
            }
        }

        /// <summary>
        /// Validate the Values of Amount in the Excel
        /// </summary>
        private void ValidateAmountValues()
        {
            try
            {
                decimal value;
                ModelUploadData = ModelUploadData.Where(a => Decimal.TryParse(a.Amount as string, out value)).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Validate ModelUpload Sheet Data
        /// </summary>
        private void ValidateModelUploadData()
        {
            try
            {
                if (ModelReferenceData != null || ModelReferenceData.IssuerId != null)
                {
                    RetrieveCOAList(ModelReferenceData.IssuerId);
                }
                if (UserName != null)
                {
                    FetchUserRole(UserName);
                }
                bool validateCOACodes = CheckCOACodes(ModelUploadData, COACodes);
                if (!validateCOACodes)
                {
                    //To-Do
                    //Add Exception Details
                }
                ValidatePeriodYears(YearList);
                ValidateEndDates(PeriodEndDate);
                ValidatePeriodLength(PeriodLength);
                ValidateActualOverride(ActualOverride);
                ValidateAmountValues();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        #endregion

        #endregion

        #region DataBaseInteractivity

        #region DeleteData-Service Methods

        /// <summary>
        /// Delete data from Internal_Statement
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="rootSource"></param>
        private void DeleteInternalStatementRecords(string issuerId, string rootSource)
        {
            try
            {
                REF = ExternalResearchEntity.ModelDeleteInteralStatement(issuerId, rootSource).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Delete data from Internal_Data
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="refList"></param>
        private void DeleteInternalDataRecords(string issuerId, List<string> refList)
        {
            try
            {
                if (refList != null)
                {
                    foreach (string item in refList)
                    {
                        ExternalResearchEntity.ModelDeleteInternalData(issuerId, item);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Delete data from Internal_Commodity_Assumptions
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="refList"></param>
        private void DeleteInternalCommodityAssumptionsRecords(string issuerId, List<string> refList)
        {
            try
            {
                if (refList != null || issuerId != null)
                {
                    foreach (string item in refList)
                    {
                        ExternalResearchEntity.ModelDeleteInternalCommodityAssumptions(issuerId, item);

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Delete Records from Internal_Issuer
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="COA_Type"></param>
        /// <param name="lastPrimaryModelLoad"></param>
        /// <param name="lastIndustryModelLoad"></param>
        private void DeleteInternalIssuerRecords(string issuerId, string COA_Type, DateTime lastPrimaryModelLoad, DateTime lastIndustryModelLoad)
        {
            try
            {
                if (issuerId != null && COA_Type != null)
                {
                    ExternalResearchEntity.ModelDeleteInternalIssuer(issuerId);
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Delete data from Internal_Issuer_Quarterly_Assumptions
        /// </summary>
        /// <param name="issuerId">Issuer_Id</param>
        /// <param name="dataSource">Data_Source</param>
        private void DeleteInternalIssuerQuarterelyDistribution(string issuerId, string dataSource)
        {
            try
            {
                if (issuerId != null && dataSource != null)
                {
                    ExternalResearchEntity.ModelDeleteInternalIssuerQuarterlyDistribution(issuerId, dataSource);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region InsertData-Service Methods

        /// <summary>
        /// Insert Record in Internal_Statement Service Methods
        /// </summary>
        /// <param name="issuerId">Issuer Id</param>
        /// <param name="refNo">REF No</param>
        /// <param name="periodYear">Period Year</param>
        /// <param name="rootSource">Root Source</param>
        /// <param name="rootSourceDate">Root Source Date</param>
        /// <param name="periodLength">Period Length</param>
        /// <param name="periodEndDate">Period End Date</param>
        /// <param name="currency">Currency</param>
        /// <param name="amountType">Amount Type</param>
        private void InsertInternalStatementServiceMethod
            (string issuerId, string refNo, int periodYear, string rootSource, DateTime rootSourceDate,
                int periodLength, DateTime periodEndDate, string currency, string amountType)
        {
            try
            {
                if (issuerId != null)
                {
                    ExternalResearchEntity.ModelInsertInternalStatement(issuerId, refNo, periodYear, rootSource, rootSourceDate,
                        periodLength, periodEndDate, currency, amountType);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Insert data in Internal_Data
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="refNo"></param>
        /// <param name="periodType"></param>
        /// <param name="COA"></param>
        /// <param name="amount"></param>
        private void InsertInternalDataRecords(string issuerId, string refNo, string periodType, string COA, decimal? amount)
        {
            try
            {
                if (issuerId != null || refNo != null)
                {
                    ExternalResearchEntity.ModelInsertInternalData(issuerId, refNo, periodType, COA, amount);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Insert data into Internal_Commodity_Assumptions
        /// </summary>
        private void InsertInternalCommodityAssumptionsData(string issuerId, string ref_no, string commodityId, decimal amount)
        {
            try
            {
                if (issuerId != null && ref_no != null)
                {
                    ExternalResearchEntity.ModelInsertInternalCommodityAssumptions(issuerId, ref_no, commodityId, amount);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Insert Data in Internal_Issuer
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="COA_Type"></param>
        /// <param name="lastPrimaryModelLoad"></param>
        /// <param name="lastIndustryModelLoad"></param>
        private void InsertInternalIssuerData(string issuerId, string COA_Type, DateTime lastPrimaryModelLoad, DateTime lastIndustryModelLoad)
        {
            try
            {
                if (issuerId != null)
                {
                    ExternalResearchEntity.ModelInsertInternalIssuer(issuerId, COA_Type, lastPrimaryModelLoad, lastIndustryModelLoad);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// Insert data in Internal_Issuer_Quarterely_Distribution
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="dataSource"></param>
        /// <param name="periodType"></param>
        /// <param name="percentage"></param>
        /// <param name="lastUpdated"></param>
        private void InsertInternalIssuerQuarterlyDistribution(string issuerId, string dataSource, string periodType, decimal percentage, DateTime lastUpdated)
        {
            try
            {
                if (issuerId != null && dataSource != null)
                {
                    ExternalResearchEntity.ModelInsertInternalIssuerQuaterelyDistribution(issuerId, dataSource, periodType, percentage, lastUpdated);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion


        #region UpdateData

        /// <summary>
        /// Update Internal_Issuer
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="lastPrimaryUpload"></param>
        /// <param name="lastIndustryUpload"></param>
        private void UpdateInternalIssuer(string issuerId, DateTime lastPrimaryUpload, DateTime lastIndustryUpload)
        {
            try
            {
                if (issuerId != null)
                {
                    ExternalResearchEntity.ModelUpdateDataInternalIssuer(issuerId, lastPrimaryUpload, lastIndustryUpload);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }
        
        #endregion


        #region FetchData

        /// <summary>
        /// Retrieve List of COA for selected Issuer
        /// </summary>
        /// <param name="issuerId"></param>
        private void RetrieveCOAList(string issuerId)
        {
            try
            {
                List<DataPointsModelUploadData> result = new List<DataPointsModelUploadData>();
                result = ExternalResearchEntity.RetrieveDataPointsModelUpload(issuerId).ToList();
                COACodes = result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Check the Role of the User: Primary/Industry
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <returns></returns>
        private void FetchUserRole(string userName)
        {
            GF_SECURITY_BASEVIEW data = DimensionEntity.GF_SECURITY_BASEVIEW.
                Where(a => a.ASHMOREEMM_PRIMARY_ANALYST.ToUpper() == userName.ToUpper()).FirstOrDefault();
            if (data != null)
            {
                UserRole = "PRIMARY";
                RootSource = "PRIMARY";
            }
            data = DimensionEntity.GF_SECURITY_BASEVIEW.Where(a => a.ASHMOREEMM_INDUSTRY_ANALYST.ToUpper() == userName.ToUpper()).FirstOrDefault();
            if (data != null)
            {
                UserRole = "INDUSTRY";
                RootSource = "INDUSTRY";
            }
            else
            {
                UserRole = "";
            }
        }

        /// <summary>
        /// Internal_Issuer: Retrieve Data
        /// </summary>
        /// <param name="issuerId"></param>
        /// <returns></returns>
        private INTERNAL_ISSUER FetchInternalIssuerData(string issuerId)
        {
            try
            {
                INTERNAL_ISSUER result = new INTERNAL_ISSUER();
                result = ExternalResearchEntity.ModelRetrieveInternalIssuer(issuerId).ToList().FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #endregion

        #endregion

    }
}