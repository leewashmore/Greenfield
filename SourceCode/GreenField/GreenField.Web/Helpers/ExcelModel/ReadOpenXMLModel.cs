using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using GreenField.DAL;
using GreenField.Web.DataContracts;
using GreenField.Web.DimensionEntitiesService;
using GreenField.Web.Services;

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
                {
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));
                }
                return dimensionEntity;
            }
        }

        /// <summary>
        /// ICPresentation Entity
        /// </summary>
        private ICPresentationEntities ICPresentationEntityp;
        public ICPresentationEntities ICPresentationEntity
        {
            get
            {
                if (ICPresentationEntityp == null)
                {
                    ICPresentationEntityp = new ICPresentationEntities();
                }
                return ICPresentationEntityp;
            }
            set
            {
                ICPresentationEntityp = value;
            }
        }

        /// <summary>
        /// Entity of ExternalResearchDataModel
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
        /// Username
        /// </summary>
        private string userName;
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Role of the User
        /// </summary>
        private string userRole;
        public string UserRole
        {
            get { return userRole; }
            set { userRole = value; }
        }

        /// <summary>
        /// Root-Source of Data
        /// </summary>
        private string rootSource = "";
        public string RootSource
        {
            get { return rootSource; }
            set { rootSource = value; }
        }

        /// <summary>
        /// Time-Stamp to be put in Each Data.
        /// </summary>
        private DateTime timeStamp = DateTime.Now;
        public DateTime TimeStamp
        {
            get
            {
                return timeStamp;
            }
            set
            {
                timeStamp = value;
            }
        }

        /// <summary>
        /// List for REF values
        /// </summary>
        private List<string> REFp;
        public List<string> REF
        {
            get
            {
                if (REFp == null)
                {
                    REFp = new List<string>();
                }
                return REFp;
            }
            set { REFp = value; }
        }

        /// <summary>
        /// Number of Years for which Data to Load
        /// </summary>
        private int numberOfYears;
        public int NumberOfYears
        {
            get { return numberOfYears; }
            set { numberOfYears = value; }
        }

        /// <summary>
        /// Unique Reference Number: stored per year in Internal_Statement
        /// </summary>
        private string uniqueRefNumber;
        public string UniqueRefNumber
        {
            get { return uniqueRefNumber; }
            set { uniqueRefNumber = value; }
        }

        /// <summary>
        /// Dictionary to store Year with ReferenceNumber
        /// </summary>
        private Dictionary<string, string> recordYearRefNo;
        public Dictionary<string, string> RecordYearRefNo
        {
            get
            {
                if (recordYearRefNo == null)
                {
                    recordYearRefNo = new Dictionary<string, string>();
                }
                return recordYearRefNo;
            }
            set { recordYearRefNo = value; }
        }

        /// <summary>
        /// DocumentId
        /// </summary>
        private long documentId = 0;
        public long DocumentId
        {
            get { return documentId; }
            set { documentId = value; }
        }

        #endregion

        /// <summary>
        /// The byte stream of file to be uploaded
        /// </summary>
        private byte[] fileBytes;
        public byte[] FileBytes
        {
            get { return fileBytes; }
            set { fileBytes = value; }
        }

        private string fileURI;
        public string FileURI
        {
            get { return fileURI; }
            set { fileURI = value; }
        }

        private List<string> distinctCurrency;
        public List<string> DistinctCurrency
        {
            get
            {
                if (distinctCurrency == null)
                {
                    distinctCurrency = new List<string>();
                }
                return distinctCurrency;
            }
            set { distinctCurrency = value; }
        }

        /// <summary>
        /// The message to show in case of an Exception
        /// </summary>
        private string exceptionMessage = "";
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; }
        }

        /// <summary>
        /// The invalid Value
        /// </summary>
        private string invalidValue = "";
        public string InvalidValue
        {
            get
            {
                return invalidValue;
            }
            set
            {
                invalidValue = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ExcelModelUploadSheet uploadData;
        public ExcelModelUploadSheet UploadData
        {
            get { return uploadData; }
            set { uploadData = value; }
        }

        /// <summary>
        /// The name of the file
        /// </summary>
        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        /// <summary>
        /// The path where file is stored
        /// </summary>
        private string filepath;
        public string Filepath
        {
            get { return filepath; }
            set { filepath = value; }
        }

        #region ModelUpload

        private IEnumerable<Sheet> sheetModelUpload;
        public IEnumerable<Sheet> SheetModelUpload
        {
            get { return sheetModelUpload; }
            set { sheetModelUpload = value; }
        }

        private WorksheetPart workSheetPartModelUpload;
        public WorksheetPart WorkSheetPartModelUpload
        {
            get { return workSheetPartModelUpload; }
            set { workSheetPartModelUpload = value; }
        }

        private SheetData sheetDataModelUpload;
        public SheetData SheetDataModelUpload
        {
            get { return sheetDataModelUpload; }
            set { sheetDataModelUpload = value; }
        }

        private List<DataPointsModelUploadData> COACodesp;
        public List<DataPointsModelUploadData> COACodes
        {
            get
            {
                if (COACodesp == null)
                {
                    COACodesp = new List<DataPointsModelUploadData>();
                }
                return COACodesp;
            }
            set
            {
                COACodesp = value;
            }
        }

        private List<ExcelModelDataUpload> modelUploadData;
        public List<ExcelModelDataUpload> ModelUploadData
        {
            get { return modelUploadData; }
            set { modelUploadData = value; }
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
            set
            {
                _yearsToLoad = value;
            }
        }

        #endregion

        #endregion

        #region ModelReference

        /// <summary>
        /// ModelReferenceSheet
        /// </summary>
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

        /// <summary>
        /// if user has populated QuarterelyOverride Values
        /// </summary>
        private bool _quarterlyOverrideEnabled;
        public bool QuarterelyOverrideEnabled
        {
            get { return _quarterlyOverrideEnabled; }
            set { _quarterlyOverrideEnabled = value; }
        }


        #endregion

        #endregion

        #region PublicMethods

        /// <summary>
        /// Read Excel File public method
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="userName"></param>
        public string ReadExcelData(byte[] fileStream, string userName)
        {
            try
            {
                FileBytes = fileStream;
                Filepath = GetFileName();
                CreateTempFile(fileStream);
                UserName = userName;
                DistinctCurrency = GetDistinctCurrency();
                ExternalResearchEntity.CommandTimeout = 300;
                using (SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(Filepath, true))
                {
                    WorkbookPart workbookPart = myWorkbook.WorkbookPart;
                    bool checkWorkSheetExists = CheckSheetsExist(workbookPart);
                    if (!checkWorkSheetExists)
                    {
                        throw new Exception("Sheets doesn't Exist, check Workbook");
                    }
                    string modelUploadSheetId = SheetModelUpload.First().Id.Value;
                    string modelReferenceSheetId = SheetModelReference.First().Id.Value;
                    WorkSheetPartModelUpload = (WorksheetPart)myWorkbook.WorkbookPart.GetPartById(modelUploadSheetId);
                    WorkSheetPartModelReference = (WorksheetPart)myWorkbook.WorkbookPart.GetPartById(modelReferenceSheetId);

                    ReadSheetData(WorkSheetPartModelUpload, WorkSheetPartModelReference);
                    NumberOfYears = FindNumberOfYearsToLoad(SheetDataModelUpload, workbookPart);

                    ReadModelReferenceSheetData(workbookPart);
                    ReadModelUploadSheetData(workbookPart);
                    bool isSheetValid = ValidateSheetData();
                    if (!isSheetValid)
                    {
                        throw new Exception();
                    }
                    DBDeleteOperations();
                    DBInsertOperations();
                    return "Sheet Uploaded Successfully";
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\n" + ex.Message;
                }
                return ExceptionMessage;
            }
        }

        #endregion

        #region OpearationalMethods

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
                ExceptionMessage += "\nError while Reading Model Reference Sheet";
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
                ExceptionMessage += "\nError while Reading Model Upload Sheet";
            }
        }

        /// <summary>
        /// Validate the Data in both the Sheets
        /// </summary>
        private bool ValidateSheetData()
        {
            try
            {
                bool modelReferenceDataValid = ValidateModelReferenceData(ModelReferenceData);
                if (!modelReferenceDataValid)
                {
                    throw new Exception();
                }
                bool modelUploadSheetValid = ValidateModelUploadData();
                if (!modelUploadSheetValid)
                {
                    throw new Exception();
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nData in Model Reference Sheet is not Valid";
                }
                throw;
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
                    DeleteInternalIssuerQuarterelyDistribution(ModelReferenceData.IssuerId, RootSource);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Operations is DB
        /// </summary>
        private void DBInsertOperations()
        {
            try
            {
                InsertIntoInternalStatementData();
                InsertIntoInternalData();
                InsertIntoInternalCommodityAssumptions();
                CheckInternalIssuer();
                if (QuarterelyOverrideEnabled)
                {
                    DeleteInternalIssuerQuarterelyDistribution(ModelReferenceData.IssuerId, RootSource);
                    InsertIntoInternalIssuerQuarterelyDistribution();
                }
                else
                {
                    DeleteInternalIssuerQuarterelyDistribution(ModelReferenceData.IssuerId, RootSource);
                }
                CheckInternalIssuer();
                SetInterimAmountsServiceCall(ModelReferenceData.IssuerId, RootSource);
                GetDataServiceCall(ModelReferenceData.IssuerId, "Y");
                CheckInternalModelLoad();
                CheckInternalCOAChanges();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Data in Internal_Statement
        /// </summary>
        private bool InsertIntoInternalStatementData()
        {
            try
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
                        InsertInternalStatementServiceMethod(ModelReferenceData.IssuerId, UniqueRefNumber, year, RootSource, TimeStamp,
                            periodLength, periodEndDate, ModelReferenceData.Currencies.FirstOrDefault(), amountType);
                        RecordYearRefNo.Add(item.Key, UniqueRefNumber);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Data into Internal_Data
        /// </summary>
        private bool InsertIntoInternalData()
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
                            convertFlag = Convert.ToString(COACodes.Where(a => a.COA.ToUpper().Trim() == data.COA.ToUpper().Trim()).Select(a => a.CONVERT_FLAG).FirstOrDefault());
                            if (convertFlag != null)
                            {
                                if (convertFlag.ToUpper().Trim() == "Y")
                                {
                                    amount = ConvertAmount(ModelReferenceData.Units.First().ToUpper().Trim(), Convert.ToDecimal(amount));
                                }
                            }
                            InsertInternalDataRecords(issuerId, UniqueRefNumber, periodType, data.COA, amount);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Check if Data exists in Internal_Issuer
        /// </summary>
        private bool CheckInternalIssuer()
        {
            try
            {
                string issuerId = ModelReferenceData.IssuerId;
                string coa = ModelReferenceData.COATypes;
                string rootSource = RootSource;
                DateTime? lastUpdatePrimary = DateTime.Today;
                DateTime? lastUpdateIndustry = DateTime.Today;

                INTERNAL_ISSUER issuerData = FetchInternalIssuerData(issuerId);
                if (RootSource == "PRIMARY")
                {
                    lastUpdatePrimary = TimeStamp;
                    lastUpdateIndustry = null;
                }
                else if (RootSource == "INDUSTRY")
                {
                    lastUpdateIndustry = TimeStamp;
                    lastUpdatePrimary = null;
                }

                if (issuerData != null)
                {
                    UpdateInternalIssuer(issuerId, lastUpdatePrimary, lastUpdateIndustry);
                }
                else
                {
                    InsertInternalIssuerData(issuerId, ModelReferenceData.COATypes, lastUpdatePrimary, lastUpdateIndustry);
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Data into Internal_Commodity_Assumptions
        /// </summary>
        private bool InsertIntoInternalCommodityAssumptions()
        {
            try
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
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Insert Values into Internal_Issuer-Quartererly_Distribution
        /// </summary>
        private bool InsertIntoInternalIssuerQuarterelyDistribution()
        {
            try
            {
                string issuerId = ModelReferenceData.IssuerId;
                string data_source = RootSource;
                string periodType = "Q";
                List<decimal> percentage = new List<decimal>() { (decimal)ModelReferenceData.Q1Override, (decimal)ModelReferenceData.Q2Override, 
                (decimal)ModelReferenceData.Q3Override, (decimal)ModelReferenceData.Q4Override };
                if (QuarterelyOverrideEnabled)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        InsertInternalIssuerQuarterlyDistribution(issuerId, RootSource, periodType + (i + 1).ToString(), percentage[i] / 100M, TimeStamp);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Check Internal_Model_Load
        /// </summary>
        private void CheckInternalModelLoad()
        {
            try
            {
                string issuerId = ModelReferenceData.IssuerId;
                string rootSource = RootSource;
                DateTime loadTime = TimeStamp;
                long documentId = DocumentId;
                Internal_Model_Load record = new Internal_Model_Load();
                List<Internal_Model_Load> data = FetchInternalModelLoadData(issuerId, rootSource, DateTime.Today);
                if (data != null)
                {
                    if (data.Any(a => a.LOAD_TIME.Date == DateTime.Today.Date))
                    {
                        record = data.Where(a => a.LOAD_TIME.Date == DateTime.Today.Date).FirstOrDefault();
                        documentId = record.DOCUMENT_ID;
                        DocumentWorkspaceOperations service = new DocumentWorkspaceOperations();
                        String nameOFFile = "Model_" + issuerId + TimeStamp.ToString("ddMMyyyy") + ".xls";
                        List<FileMaster> fileRecords = ICPresentationEntity.FileMasters.Where(a => a.Type.Trim().ToLower() ==
                          "Models".Trim().ToLower() && a.Name.Trim().ToLower() == nameOFFile.Trim().ToLower()).ToList();
                        foreach (FileMaster filerec in fileRecords)
                        {
                            String deleteUrl = filerec.Location;
                            service.DeleteDocument(deleteUrl);
                            ICPresentationEntity.DeleteFileMaster(filerec.FileID);
                        }
                        FileURI = service.UploadDocument(ModelReferenceData.IssuerId + ".xls", FileBytes, string.Empty);
                        bool fileRecordCreated = service.SetUploadFileInfo(UserName, "Model_" + issuerId + TimeStamp.ToString("ddMMyyyy") + ".xls",
                            FileURI, ModelReferenceData.IssuerName, null, null
                            , "Models", null, null);
                        if (fileRecordCreated)
                        {
                            FILE_MASTER fileRecord = new FILE_MASTER();
                            fileRecord = FetchFileId(FileURI).FirstOrDefault();
                            if (fileRecord != null)
                            {
                                documentId = fileRecord.FileID;
                                DocumentId = documentId;
                            }
                        }
                        UpdateInternalModelLoad(record.LOAD_ID, issuerId, rootSource, UserName, loadTime, documentId);
                    }
                    else
                    {
                        DocumentWorkspaceOperations service = new DocumentWorkspaceOperations();
                        FileURI = service.UploadDocument(ModelReferenceData.IssuerId + ".xls", FileBytes, string.Empty);
                        bool fileRecordCreated = service.SetUploadFileInfo(UserName, "Model_" + issuerId + TimeStamp.ToString("ddMMyyyy") + ".xls",
                            FileURI, ModelReferenceData.IssuerName, null, null
                            , "Models", null, null);
                        if (fileRecordCreated)
                        {
                            FILE_MASTER fileRecord = new FILE_MASTER();
                            fileRecord = FetchFileId(FileURI).FirstOrDefault();
                            if (fileRecord != null)
                            {
                                documentId = fileRecord.FileID;
                                DocumentId = documentId;
                            }
                        }
                        if (documentId != null)
                        {
                            InsertInternalModelLoadData(issuerId, rootSource, UserName, loadTime, documentId);
                        }
                    }
                }
                else
                {
                    DocumentWorkspaceOperations service = new DocumentWorkspaceOperations();
                    FileURI = service.UploadDocument(ModelReferenceData.IssuerId + ".xls", FileBytes, string.Empty);
                    bool fileRecordCreated = service.SetUploadFileInfo(UserName, "Model_" + issuerId + TimeStamp.ToString("ddMMyyyy") + ".xls",
                        FileURI, ModelReferenceData.IssuerName, null, null
                        , "Models", null, null);
                    if (fileRecordCreated)
                    {
                        FILE_MASTER fileRecord = new FILE_MASTER();
                        fileRecord = FetchFileId(FileURI).FirstOrDefault();
                        if (fileRecord != null)
                        {
                            documentId = fileRecord.FileID;
                            DocumentId = documentId;
                        }
                    }
                    if (documentId != null)
                    {
                        InsertInternalModelLoadData(issuerId, rootSource, UserName, loadTime, documentId);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage += "\n" + ex.Message;
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Updated Data in INTERNAL_COA_CHANGES
        /// </summary>
        private void CheckInternalCOAChanges()
        {
            try
            {
                string issuerId = ModelReferenceData.IssuerId;
                string rootSource = RootSource;
                string currency = ModelReferenceData.Currencies.First();
                List<TrackedCOA> trackedCOA = FetchTrackedCOA();
                int periodYear = 0;
                string convertFlag = "";
                decimal? amount;
                decimal? fetchedAmount;
                List<INTERNAL_COA_CHANGES> internalCOAChangesData = new List<INTERNAL_COA_CHANGES>();
                foreach (TrackedCOA item in trackedCOA)
                {
                    foreach (var year in YearsToLoad)
                    {
                        periodYear = Convert.ToInt32(year.Value);
                        internalCOAChangesData = FetchInternalCOAChangesData(issuerId, rootSource, item.COA, periodYear, currency); //Pull the current values for this COA from SQL
                        if (ModelUploadData.Any(a => a.COA == item.COA && a.Year == year.Key))
                        {
                            amount = Decimal.Parse(ModelUploadData.Where(a => a.COA == item.COA && a.Year == year.Key).Select(a => a.Amount).FirstOrDefault() as string);
                        }
                        else
                        {
                            amount = null;
                        }
                        DateTime? periodEndDate = PeriodEndDate.Where(a => a.Key == year.Key).Select(a => a.Value).FirstOrDefault();
                        if (amount != null)
                        {
                            convertFlag = Convert.ToString(COACodes.Where(a => a.COA.ToUpper().Trim() == item.COA.ToUpper().Trim()).Select(a => a.CONVERT_FLAG).FirstOrDefault());
                            if (convertFlag != null)
                            {
                                if (convertFlag.ToUpper().Trim() == "Y")
                                {
                                    amount = ConvertAmount(ModelReferenceData.Units.First().ToUpper().Trim(), Convert.ToDecimal(amount));
                                }
                            }

                            if (internalCOAChangesData == null || internalCOAChangesData.Count == 0) //If there was no current values pulled, then insert a new values in SQL
                            {
                                InsertInternalCOAChangesData(issuerId, rootSource, DocumentId, currency, item.COA, periodYear, periodEndDate, TimeStamp, null, (decimal)amount, "M");
                            }
                            else //If there were current values and they were different from new values, then update with End Date and insert a new values.  
                            {
                                fetchedAmount = internalCOAChangesData.Select(a => a.AMOUNT).FirstOrDefault();
                                // if (fetchedAmount != amount) - replaced to correct matching issue when precision is different - Request ID : 18420 Model upload "COA" value change tracking 
                                if (Math.Round((decimal)fetchedAmount, 6, MidpointRounding.AwayFromZero) != Math.Round((decimal)amount, 6, MidpointRounding.AwayFromZero))
                                {
                                    UpdateInternalCOAChanges(issuerId, rootSource, currency, item.COA, periodYear, TimeStamp);
                                    InsertInternalCOAChangesData(issuerId, rootSource, DocumentId, currency, item.COA, periodYear, periodEndDate, TimeStamp, null, (decimal)amount, "M");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        #endregion

        #region Common Methods

        /// <summary>
        /// Check if both Sheets Exist
        /// </summary>
        /// <param name="workbookPart"></param>
        /// <returns></returns>
        public bool CheckSheetsExist(WorkbookPart workbookPart)
        {
            try
            {
                IEnumerable<Sheet> sheetModelUpload = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Model Upload");
                SheetModelUpload = sheetModelUpload;
                if (sheetModelUpload.Count() == 0)
                {
                    ExceptionMessage += "\nModel Upload sheet is not present";
                    return false;
                }
                IEnumerable<Sheet> sheetModelReference = workbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().Where(s => s.Name == "Model Reference");
                SheetModelReference = sheetModelReference;
                if (sheetModelReference.Count() == 0)
                {
                    ExceptionMessage += "\nModel Reference sheet is not present";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Get the name of the file
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid() + "_Model.xls";
            return fileName;
        }

        /// <summary>
        /// Create Excel File in Temp Folder
        /// </summary>
        /// <param name="file"></param>
        private void CreateTempFile(byte[] file)
        {
            byte[] fileBytes = file;
            File.WriteAllBytes(Filepath, fileBytes);
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
            try
            {
                int i = 0;
                string[] rowData = new string[18];

                foreach (Row r in sheetData.Elements<Row>())
                {
                    int cellCount = 0;
                    List<Cell> cellList = r.Elements<Cell>().ToList();

                    if (cellList.Count == 1)
                    {
                        rowData[i] = cellList[0].InnerText;
                        if (cellList[0].DataType != null)
                        {
                            if (cellList[0].DataType == CellValues.SharedString)
                            {
                                var stringTable = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                                if (stringTable != null)
                                {
                                    rowData[i] = stringTable.SharedStringTable.ElementAt(int.Parse(rowData[i])).InnerText;
                                }
                            }
                        }
                        rowData[++i] = string.Empty;
                        ++i;
                    }
                    else
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
                            cellCount++;
                            ++i;
                        }
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
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
            try
            {
                if (ModelReferenceData.COATypes == null || ModelReferenceData.COATypes.Trim() == "" ||
                  ModelReferenceData.Currencies == null || ModelReferenceData.Currencies.FirstOrDefault().Trim() == "")
                {
                    throw new Exception("There is no value specified in currency Field");
                }

                bool issuerIdValidity = CheckIssuerIdExist(ModelReferenceData.IssuerId);
                if (!issuerIdValidity)
                {
                    throw new Exception("The value specified in Issuer Field is not Valid");
                }

                bool COAValidity = CheckCOATypeValidity(ModelReferenceData.IssuerId, ModelReferenceData.COATypes);
                if (!COAValidity)
                {
                    throw new Exception("The value COA in ModelReference sheet is not Valid");
                }

                bool quarterelyValid = CheckOverrideValues();
                if (!quarterelyValid)
                {
                    throw new Exception("Quarterely Override values are not Valid");
                }
                bool currencyValid = CheckCurrency();
                if (!currencyValid)
                {
                    throw new Exception("Entered Currency is not Valid");
                }
                bool unitsValid = CheckUnits();
                if (!unitsValid)
                {
                    throw new Exception("Entered Value of Units in Model Reference sheet is not Valid");
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\n" + ex.Message;
                throw;
            }
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
            try
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Check the Override Values
        /// </summary>
        /// <returns></returns>
        private bool CheckOverrideValues()
        {
            try
            {
                decimal value;
                string q1 = Convert.ToString(ModelReferenceData.Q1Override);
                string q2 = Convert.ToString(ModelReferenceData.Q2Override);
                string q3 = Convert.ToString(ModelReferenceData.Q3Override);
                string q4 = Convert.ToString(ModelReferenceData.Q4Override);

                if (q1.Trim() == "" && q2.Trim() == "" && q3.Trim() == "" && q4.Trim() == "")
                {
                    QuarterelyOverrideEnabled = false;
                    return true;
                }

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
                    QuarterelyOverrideEnabled = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Validate Currency
        /// </summary>
        /// <returns></returns>
        private bool CheckCurrency()
        {
            try
            {
                if (DistinctCurrency.Where(a => a.ToUpper().Trim() == ModelReferenceData.Currencies.First().ToUpper().Trim()).ToList().Count == 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionMessage += "\nThe Entered currency is not Valid";
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// check the value in Units field of ModelReferenceSheet
        /// </summary>
        /// <returns></returns>
        private bool CheckUnits()
        {
            try
            {
                if (ModelReferenceData.Units.FirstOrDefault() == null)
                {
                    throw new Exception();
                }
                if (ModelReferenceData.Units.FirstOrDefault().ToUpper().Trim() == "UNITS" || ModelReferenceData.Units.FirstOrDefault().ToUpper().Trim() == "MILLIONS"
                    || ModelReferenceData.Units.FirstOrDefault().ToUpper().Trim() == "THOUSANDS" || ModelReferenceData.Units.FirstOrDefault().ToUpper().Trim() == "BILLIONS")
                {
                    return true;
                }
                else
                {
                    throw new Exception("The value of Units is not valid");
                }
            }
            catch (Exception ex)
            {
                ExceptionMessage += "\nThe specified Units is not Valid";
                ExceptionTrace.LogException(ex);
                throw;
            }
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
            try
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
                                //rowData[i] = c.InnerText;
                                if (c.CellValue != null && !string.IsNullOrEmpty(c.CellValue.Text))
                                    rowData[i] = Double.Parse(c.CellValue.Text).ToString();

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
                                if (!string.IsNullOrEmpty(item))
                                    Double.Parse(item);
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nInvalid cell value in Model Upload sheet.";
            }
        }

        /// <summary>
        /// Find Number of years to Load Data for
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        /// <returns></returns>
        private int FindNumberOfYearsToLoad(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nThe values in ModelUpload header row are not valid";
                return 0;
            }
        }

        /// <summary>
        /// Retrieve Years
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataFirstRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
            {
                string[] rowData = new string[NumberOfYears + 2];
                int i = 0;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (r.RowIndex == 1)
                    {
                        while (i < NumberOfYears + 2)
                        {
                            foreach (Cell c in r.Elements<Cell>())
                            {
                                if (c != null)
                                {
                                    //rowData[i] = c.InnerText;
                                    if (c.CellValue != null && !string.IsNullOrEmpty(c.CellValue.Text))
                                        rowData[i] = Int32.Parse(c.CellValue.Text).ToString();

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
                }
                YearList = SetPeriodYears(rowData);
                if (YearsToLoad.Count == 0)
                {
                    ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nThe values in ModelUpload Period year are not valid";
                }
            }
        }

        /// <summary>
        /// Retrieve Period EndDate
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataSecondRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
            {
                object[] rowData = new object[NumberOfYears + 2];
                double data;
                int i = 0;
                int j = 0;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (r.RowIndex == 2)
                    {
                        while (i < NumberOfYears + 2)
                        {
                            foreach (Cell c in r.Elements<Cell>())
                            {
                                if (c != null && j > 1)
                                {
                                    //rowData[i] = c.InnerText;
                                    rowData[i] = new DateTime(1900, 1, 31);
                                    if (c.CellValue != null && !string.IsNullOrEmpty(c.CellValue.Text))
                                        rowData[i] = DateTime.FromOADate(Double.Parse(c.CellValue.Text));

                                    /*
                                    if (Double.TryParse((string)rowData[i], out data))
                                    {
                                        rowData[i] = DateTime.FromOADate(data);
                                    }
                                    else
                                    {
                                        rowData[i] = new DateTime(1900, 1, 31);
                                    }
                                     */
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
                }
                PeriodEndDate = SetPeriodEndDate(rowData);
                if (YearsToLoad.Count == 0)
                {
                    ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nThe values in ModelUpload Period End Dates are not valid";
                }
            }
        }

        /// <summary>
        /// Retrieve Period Length
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataThirdRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
            {
                object[] rowData = new object[NumberOfYears + 2];
                int i = 0;
                int j = 0;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (r.RowIndex == 3)
                    {
                        while (i < NumberOfYears + 2)
                        {
                            foreach (Cell c in r.Elements<Cell>())
                            {
                                if (c != null && j > 1)
                                {
                                    //rowData[i] = c.InnerText;
                                    if (c.CellValue != null && !string.IsNullOrEmpty(c.CellValue.Text))
                                        rowData[i] = Int32.Parse(c.CellValue.Text).ToString();

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
                }
                PeriodLength = SetPeriodLength(rowData);
                if (YearsToLoad.Count == 0)
                {
                    ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nThe values in ModelUpload Period Length are not valid";
                }
            }
        }

        /// <summary>
        /// Retrieve Actual Override
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataFourthRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
            {
                object[] rowData = new object[NumberOfYears + 2];
                int i = 0;
                int j = 0;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (r.RowIndex == 4)
                    {
                        while (i < NumberOfYears + 2)
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
                }
                ActualOverride = SetOverride(rowData);
                if (YearsToLoad.Count == 0)
                {
                    ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nThe values in ModelUpload Period Length are not valid";
                }
            }
        }

        /// <summary>
        /// Retrieve Commodity Measure
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataFifthRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
            {
                object[] rowData = new object[NumberOfYears + 2];
                int i = 0;
                int j = 0;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (r.RowIndex == 5)
                    {
                        while (i < NumberOfYears + 2)
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
                }
                CommodityMeasure = SetMeasure(rowData);
                if (YearsToLoad.Count == 0)
                {
                    ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nThe values in ModelUpload Period Length are not valid";
                }
            }
        }

        /// <summary>
        /// Retrieve Commodity Forecast Used
        /// </summary>
        /// <param name="sheetData"></param>
        /// <param name="workbookPart"></param>
        private void ReadModelUploadDataSixthRow(SheetData sheetData, WorkbookPart workbookPart)
        {
            try
            {
                object[] rowData = new object[NumberOfYears + 2];
                int i = 0;
                int j = 0;
                foreach (Row r in sheetData.Elements<Row>())
                {
                    if (r.RowIndex == 6)
                    {
                        while (i < NumberOfYears + 2)
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
                }
                CommodityForecastUsed = SetCommodityForecast(rowData);
                if (YearsToLoad.Count == 0)
                {
                    ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                if (ExceptionMessage == null || ExceptionMessage == "")
                {
                    ExceptionMessage += "\nThe values in ModelUpload Period Length are not valid";
                }
            }
        }

        /// <summary>
        /// Fill up the Years
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, int?> SetPeriodYears(string[] data)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
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
            try
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return 0;
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
            try
            {
                bool result = true;
                foreach (DataPointsModelUploadData item in COAList)
                {
                    if ((modelUploadData.Where(a => a.COA.Trim() == item.COA.Trim()).ToList().Count) == 0)
                    {
                        InvalidValue = item.COA;
                        throw new Exception();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nThe List of COA don't match: " + InvalidValue;
                return false;
            }
        }

        /// <summary>
        /// Validate Data for Period Years
        /// </summary>
        /// <param name="periodYearData"></param>
        private bool ValidatePeriodYears(Dictionary<string, int?> periodYearData)
        {
            try
            {
                if (periodYearData != null)
                {
                    List<string> badYears = periodYearData.Where(a => a.Value == null).Select(a => a.Key).ToList();
                    foreach (string item in badYears)
                    {
                        RemoveBadYearData(item);
                    }

                    if (YearsToLoad.Count == 0)
                    {
                        ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                        throw new Exception();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// ValidateEndDates
        /// </summary>
        /// <param name="periodEndDates"></param>
        private bool ValidateEndDates(Dictionary<string, DateTime?> periodEndDates)
        {
            try
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
                    if (YearsToLoad.Count == 0)
                    {
                        ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                        throw new Exception();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Validate the PeriodLength
        /// </summary>
        /// <param name="periodLength"></param>
        private bool ValidatePeriodLength(Dictionary<string, int?> periodLength)
        {
            try
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
                    if (YearsToLoad.Count == 0)
                    {
                        ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                        throw new Exception();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Validate the values of Reported Override
        /// </summary>
        /// <param name="actualOverride"></param>
        private bool ValidateActualOverride(Dictionary<string, string> actualOverride)
        {
            try
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
                    if (YearsToLoad.Count == 0)
                    {
                        ExceptionMessage += "\nMissing Data in Model Upload Worksheet";
                        throw new Exception();
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Remove Data for Year which is Corrupt
        /// </summary>
        /// <param name="yearName"></param>
        private void RemoveBadYearData(string yearName)
        {
            try
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
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
            }
        }

        /// <summary>
        /// Validate the Values of Amount in the Excel
        /// </summary>
        private bool ValidateAmountValues()
        {
            try
            {
                decimal value;
                ModelUploadData = ModelUploadData.Where(a => Convert.ToString(a.Amount).Trim() != "" && Convert.ToString(a.Amount).Trim() != "-").ToList();
                if (ModelUploadData.Any(a => !Decimal.TryParse(a.Amount as string, out value)))
                {
                    ExceptionMessage += "\nThe Data for Amount values is not valid";
                    throw new Exception();
                }
                ModelUploadData = ModelUploadData.Where(a => Decimal.TryParse(a.Amount as string, out value)).ToList();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// Validate ModelUpload Sheet Data
        /// </summary>
        private bool ValidateModelUploadData()
        {
            try
            {
                bool isValid = true;
                if (ModelReferenceData != null || ModelReferenceData.IssuerId != null)
                {
                    RetrieveCOAList(ModelReferenceData.IssuerId);
                }

                isValid = FetchUserRole(UserName);
                if (!isValid)
                {

                    throw new Exception();
                }

                bool validateCOACodes = CheckCOACodes(ModelUploadData, COACodes);
                if (!validateCOACodes)
                {
                    throw new Exception();
                }
                isValid = ValidatePeriodYears(YearList);
                if (!isValid)
                {
                    throw new Exception();
                }
                isValid = ValidateEndDates(PeriodEndDate);
                if (!isValid)
                {
                    throw new Exception();
                }
                isValid = ValidatePeriodLength(PeriodLength);
                if (!isValid)
                {
                    throw new Exception();
                }
                isValid = ValidateActualOverride(ActualOverride);
                if (!isValid)
                {
                    throw new Exception();
                }
                isValid = ValidateAmountValues();
                if (!isValid)
                {
                    throw new Exception();
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
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
                InvalidValue = issuerId;
                REF = ExternalResearchEntity.ModelDeleteInteralStatement(issuerId, rootSource).ToList();
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Deleting Data from Internal_Statement for: " + InvalidValue;
                throw;
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
                        InvalidValue = item;
                        ExternalResearchEntity.ModelDeleteInternalData(issuerId, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Deleting Data from Internal_Data for Ref= " + InvalidValue;
                throw;
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
                        InvalidValue = item;
                        ExternalResearchEntity.ModelDeleteInternalCommodityAssumptions(issuerId, item);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Deleting Data from Internal_Data for Ref= " + InvalidValue;
                throw;
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
                    InvalidValue = issuerId;
                    ExternalResearchEntity.ModelDeleteInternalIssuer(issuerId);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Deleting Data from Internal_Issuer for Issuer= " + InvalidValue;
                throw;
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
                    InvalidValue = "Issuer Name= " + issuerId + ", Root Source= " + dataSource;
                    ExternalResearchEntity.ModelDeleteInternalIssuerQuarterlyDistribution(issuerId, dataSource);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Deleting Data from Internal_Issuer_Quarterely Distribution for Issuer= " + InvalidValue;
                throw;
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
                    InvalidValue = "Issuer id= " + issuerId + ", period year= " + periodYear.ToString();
                    ExternalResearchEntity.ModelInsertInternalStatement(issuerId, refNo, periodYear, rootSource, rootSourceDate,
                        periodLength, periodEndDate, currency, amountType);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Inserting Data in Internal_Statement  for Issuer= " + InvalidValue;
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
                    InvalidValue = "Issuer Id= " + issuerId + " Ref Number: " + refNo;
                    ExternalResearchEntity.ModelInsertInternalData(issuerId, refNo, periodType, COA, amount);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Inserting Data in Internal_Data for " + InvalidValue;
                throw;
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
                    InvalidValue = " Issuer Id= " + issuerId + ", Ref Number= " + ref_no;
                    ExternalResearchEntity.ModelInsertInternalCommodityAssumptions(issuerId, ref_no, commodityId, amount);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Inserting Data in Internal_Commodity_Assumptions for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Insert Data in Internal_Issuer
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="COA_Type"></param>
        /// <param name="lastPrimaryModelLoad"></param>
        /// <param name="lastIndustryModelLoad"></param>
        private void InsertInternalIssuerData(string issuerId, string COA_Type, DateTime? lastPrimaryModelLoad, DateTime? lastIndustryModelLoad)
        {
            try
            {
                if (issuerId != null)
                {
                    InvalidValue = "Issuer Id= " + issuerId;
                    ExternalResearchEntity.ModelInsertInternalIssuer(issuerId, COA_Type, lastPrimaryModelLoad, lastIndustryModelLoad);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Inserting Data in Internal_Issuer for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Insert data in Internal_Issuer_Quarterely_Distribution
        /// </summary>
        /// <param name="issuerId">Issuer Id</param>
        /// <param name="dataSource">DataSource</param>
        /// <param name="periodType">PeriodType</param>
        /// <param name="percentage">Percentage</param>
        /// <param name="lastUpdated">LastUpdated</param>
        private void InsertInternalIssuerQuarterlyDistribution(string issuerId, string dataSource, string periodType, decimal percentage, DateTime lastUpdated)
        {
            try
            {
                if (issuerId != null && dataSource != null)
                {
                    InvalidValue = "Issuer Id= " + issuerId + " Root Source= " + dataSource;
                    ExternalResearchEntity.ModelInsertInternalIssuerQuaterelyDistribution(issuerId, dataSource, periodType, percentage, lastUpdated);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Inserting Data in Internal_Issuer_Quarterely_Distribution for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Insert Data into Internal_Model_Load
        /// </summary>
        /// <param name="issuerId">Issuer Id</param>
        /// <param name="rootSource">Root Source</param>
        /// <param name="userName">Username</param>
        /// <param name="loadTime">LoadTime</param>
        /// <param name="documentId">Document Id</param>
        private void InsertInternalModelLoadData(string issuerId, string rootSource, string userName, DateTime loadTime, long documentId)
        {
            try
            {
                InvalidValue = "Issuer Id= " + issuerId + "Root Source= " + rootSource + ", Document Id= " + documentId.ToString();
                ExternalResearchEntity.ModelInsertInternalModelLoadData(issuerId, rootSource, userName, loadTime, documentId);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Inserting Data in Internal_Model_Load for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Call Get_Data
        /// </summary>
        /// <param name="issuerId"></param>
        /// <param name="calcLog"></param>
        private void GetDataServiceCall(string issuerId, string calcLog)
        {
            try
            {
                InvalidValue = "Issuer Id= " + issuerId;
                ExternalResearchEntity.Get_Data(issuerId, calcLog, "Y", "F");
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while calling Get_Data for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Service Call for Set_Interim_Amounts
        /// </summary>
        /// <param name="issuerId">IssuerId</param>
        private void SetInterimAmountsServiceCall(string issuerId, string root_Source)
        {
            try
            {
                InvalidValue = "Issuer Id= " + issuerId;
                ExternalResearchEntity.SET_INTERIM_AMOUNTS(issuerId, root_Source);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while calling Set_Interim_Amount for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Insert data in InternalCOAChanges
        /// </summary>
        /// <param name="issuerId">IssuerId</param>
        /// <param name="rootSource">RootSource</param>
        /// <param name="loadId">LoadID</param>
        /// <param name="currency">Currency</param>
        /// <param name="coa">COA</param>
        /// <param name="periodYear">PeriodYear</param>
        /// <param name="periodEndDate">PeriodEndDate</param>
        /// <param name="startDate">StartDate</param>
        /// <param name="endDate">EndDate</param>
        /// <param name="amount">Amount</param>
        /// <param name="units">Units</param>
        private void InsertInternalCOAChangesData(string issuerId, string rootSource, long loadId, string currency, string coa, int periodYear, DateTime? periodEndDate, DateTime? startDate, DateTime? endDate, decimal amount, string units)
        {
            try
            {
                InvalidValue = "Issuer Id= " + issuerId + ", Root Source= " + rootSource + " Load Id= " + loadId;
                ExternalResearchEntity.ModelInsertInternalCOAChanges(issuerId, rootSource, loadId, currency, coa, periodYear, periodEndDate, startDate, endDate, amount, units);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while inserting values in Internal_COA_Changes for " + InvalidValue;
                throw;
            }
        }

        #endregion

        #region UpdateData- Service Methods

        /// <summary>
        /// Update Internal_Issuer
        /// </summary>
        /// <param name="issuerId">IssuerId</param>
        /// <param name="lastPrimaryUpload">LastPrimaryUpload</param>
        /// <param name="lastIndustryUpload">LastIndustryUpload</param>
        private void UpdateInternalIssuer(string issuerId, DateTime? lastPrimaryUpload, DateTime? lastIndustryUpload)
        {
            try
            {
                if (issuerId != null)
                {
                    InvalidValue = "Issuer Id =" + issuerId;
                    ExternalResearchEntity.ModelUpdateDataInternalIssuer(issuerId, lastPrimaryUpload, lastIndustryUpload);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Updating values in Internal_Issuer for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Internal_Model_Upload
        /// </summary>
        /// <param name="issuerId">IssuerId</param>
        /// <param name="rootSource">RootSource</param>
        /// <param name="userName">UserName</param>
        /// <param name="loadTime">LoadTime</param>
        /// <param name="documentId">DocumentId</param>
        private void UpdateInternalModelLoad(int loadId, string issuerId, string rootSource, string userName, DateTime loadTime, long documentId)
        {
            try
            {
                InvalidValue = "Issuer Id= " + issuerId + ", Load Id= " + loadId;
                ExternalResearchEntity.ModelUpdateInternalModelLoadData(loadId, issuerId, rootSource, userName, loadTime, documentId);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Updating values in Internal_Model_Load for " + InvalidValue;
                throw;
            }
        }

        /// <summary>
        /// Updated data into InternalCOAChanges
        /// </summary>
        /// <param name="issuerId">IssuerId</param>
        /// <param name="rootSource">rootSource</param>
        /// <param name="currency">Currency</param>
        /// <param name="coa">COA</param>
        /// <param name="periodYear">PeriodYear</param>
        /// <param name="timeStamp">TimeStamp</param>
        private void UpdateInternalCOAChanges(string issuerId, string rootSource, string currency, string coa, int periodYear, DateTime timeStamp)
        {
            try
            {
                InvalidValue = "Issuer Id= " + issuerId + ", COA= " + coa;
                ExternalResearchEntity.ModelUpdateInternalCOAChanges(issuerId, rootSource, currency, coa, periodYear, timeStamp);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                ExceptionMessage += "\nError while Updating values in Internal_COA_Changes for " + InvalidValue;
                throw;
            }
        }

        #endregion

        #region FetchData- Service Methods

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
                throw;
            }
        }

        /// <summary>
        /// Check the Role of the User: Primary/Industry
        /// </summary>
        /// <param name="userName">UserName</param>
        /// <returns></returns>
        private bool FetchUserRole(string userName)
        {
            try
            {
                //ExternalResearchEntities entities = new ExternalResearchEntities();
                var group = ExternalResearchEntity.MODEL_UPLOAD_USER_GROUP.Where(x => x.MANAGER_NAME.ToUpper() == userName.ToUpper()).Select(x => x.ANALYST_NAME.ToUpper()).ToList();
                var data = ExternalResearchEntity.GF_SECURITY_BASEVIEW_Local.Where(a => 
                    (
                        a.ASHMOREEMM_PRIMARY_ANALYST.ToUpper() == userName.ToUpper() || 
                        a.ASHMOREEMM_INDUSTRY_ANALYST.ToUpper() == userName.ToUpper() ||
                        group.Contains(a.ASHMOREEMM_PRIMARY_ANALYST.ToUpper()) || 
                        group.Contains(a.ASHMOREEMM_INDUSTRY_ANALYST.ToUpper())
                    ) 
                    && 
                    a.ISSUER_ID == ModelReferenceData.IssuerId).ToList();
                if (data.Any())
                {
                    if (data.Exists(x => x.ASHMOREEMM_PRIMARY_ANALYST.ToUpper() == userName.ToUpper() || group.Contains(x.ASHMOREEMM_PRIMARY_ANALYST.ToUpper())))  //If this user can upload for a primary analyst, it needs to go in as primary analyst
                    {
                        UserRole = "PRIMARY";
                        RootSource = "PRIMARY";
                    }
                    else
                    {
                        UserRole = "INDUSTRY";
                        RootSource = "INDUSTRY";
                    }
                    return true;
                }
                else
                {
                    throw new Exception("The user is not assigned data coverage for this company.");
                }
                
            }
            catch (Exception ex)
            {
                ExceptionMessage += "\n" + ex.Message;
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Internal_Issuer: Retrieve Data
        /// </summary>
        /// <param name="issuerId">Issuer Id</param>
        /// <returns>object of type Internal_Issuer</returns>
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

        /// <summary>
        /// Retrieve Data from Internal_Model_Load
        /// </summary>
        /// <param name="issuerId">IssuerId</param>
        /// <param name="rootSource">RootSource</param>
        /// <param name="date">LoadTime</param>
        /// <returns>Record of type Intenal_Model_Load</returns>
        private List<Internal_Model_Load> FetchInternalModelLoadData(string issuerId, string rootSource, DateTime date)
        {
            try
            {
                List<Internal_Model_Load> result = new List<Internal_Model_Load>();
                result = ExternalResearchEntity.ModelRetrieveinternalModelLoadData(issuerId, rootSource, date).ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Fetch FileId from Documentation Store
        /// </summary>
        /// <param name="location">Location of the File</param>
        /// <returns>Details of File Stored on the Server</returns>
        private List<FILE_MASTER> FetchFileId(string location)
        {
            try
            {
                List<FILE_MASTER> result = new List<FILE_MASTER>();
                result = ICPresentationEntity.ModelRetrieveFileId(location).ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Fetch the List of tracked COA's
        /// </summary>
        /// <returns></returns>
        private List<TrackedCOA> FetchTrackedCOA()
        {
            try
            {
                List<TrackedCOA> result = new List<TrackedCOA>();
                result = ExternalResearchEntity.ModelRetrieveTrackCOA().ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Service Method to fetch data from Internal_COA_Changes
        /// </summary>
        /// <param name="issuerId">Issuer Id</param>
        /// <param name="rootSource">RootSource</param>
        /// <param name="coa">COA</param>
        /// <param name="periodYear">PeriodYear</param>
        /// <param name="currency">Currency</param>
        /// <returns>Collection of INTERNAL_COA_CHANGES</returns>
        private List<INTERNAL_COA_CHANGES> FetchInternalCOAChangesData(string issuerId, string rootSource, string coa, int periodYear, string currency)
        {
            try
            {
                List<INTERNAL_COA_CHANGES> result = new List<INTERNAL_COA_CHANGES>();
                result = ExternalResearchEntity.ModelRetrieveInternalCOAChanges(issuerId, rootSource, coa, periodYear, currency).ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Distinct Currency Values
        /// </summary>
        /// <returns></returns>
        private List<string> GetDistinctCurrency()
        {
            try
            {
                List<string> result = new List<string>();
                result = ExternalResearchEntity.RetrieveDistinctFXRates().ToList();
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                throw;
            }
        }

        #endregion

        #endregion
    }
}