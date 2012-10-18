using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using X14 = DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using GreenField.DAL;
using GreenField.Web.DataContracts;

namespace GreenField.Web.ExcelModel
{
    /// <summary>
    /// Class to generate ExcelModel
    /// </summary>
    public static class GenerateOpenXMLExcelModel
    {
        #region PropertyDeclaration

        /// <summary>
        /// Object of type ExcelModelData
        /// </summary>
        private static ExcelModelData modelData;
        public static ExcelModelData ModelData
        {
            get { return modelData; }
            set { modelData = value; }
        }

        /// <summary>
        /// DataPoints of ExcelModel- ModelUpload sheet
        /// </summary>
        private static List<DataPointsModelUploadData> modelUploadDataPoints;
        public static List<DataPointsModelUploadData> ModelUploadDataPoints
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
        /// object of type ModelReferenceDataPoints-ModelReferenceSheet
        /// </summary>
        private static ModelReferenceDataPoints modelReferenceData;
        public static ModelReferenceDataPoints ModelReferenceData
        {
            get { return modelReferenceData; }
            set { modelReferenceData = value; }
        }

        /// <summary>
        /// List of Currencies
        /// </summary>
        private static List<string> currencies;
        public static List<string> Currencies
        {
            get { return currencies; }
            set { currencies = value; }
        }

        /// <summary>
        /// List of Commodities
        /// </summary>
        private static List<string> commodities;
        public static List<string> Commodities
        {
            get
            {
                if (commodities == null)
                {
                    commodities = new List<string>();
                }
                return commodities;
            }
            set { commodities = value; }
        }

        /// <summary>
        /// MaxWidth of a Column
        /// </summary>
        private static DoubleValue maxWidth;
        public static DoubleValue MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }


        #endregion

        #region Public Methods
        /// <summary>
        /// Method to Generate Byte[] for the Excel File
        /// </summary>
        /// <param name="financialData">FinancialStatement Data</param>
        /// <param name="consensusData">Consensus Data</param>
        /// <returns>Byte Stream of Excel Sheet</returns>
        public static byte[] GenerateExcel(List<FinancialStatementDataModels> financialData, List<ModelConsensusEstimatesData> consensusData, string currencyReuters, string currencyConsensus, ExcelModelData modelData)
        {
            try
            {
                string fileName = GetFileName();
                ModelData = modelData;
                ModelReferenceData = modelData.ModelReferenceData;
                ModelUploadDataPoints = modelData.ModelUploadDataPoints;
                Commodities = modelData.Commodities;
                Currencies = modelData.Currencies;
                // create a spreadsheet document by supplying the filepath.
                // by default, AutoSave = true, Editable = true, and Type = xlsx.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                    Create(fileName, SpreadsheetDocumentType.Workbook))
                {
                    UInt32 sheetId;
                    // add a WorkbookPart to the document.
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    // add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>("rId3");
                    worksheetPart.Worksheet = new Worksheet();

                    // add styles to sheet
                    WorkbookStylesPart wbsp = workbookpart.AddNewPart<WorkbookStylesPart>();
                    wbsp.Stylesheet = CreateStylesheet();
                    wbsp.Stylesheet.Save();

                    #region Reuters Reported

                    // Add Sheets to the Workbook.
                    spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                    sheetId = 1;
                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                            {
                                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                                SheetId = sheetId,
                                Name = "Reuters Reported"
                            });
                    GenerateReutersHeaders(worksheetPart, financialData, currencyReuters);
                    InsertValuesInWorksheet(worksheetPart, financialData);
                    workbookpart.Workbook.Save();

                    #endregion

                    #region ConsensusData
                    //generating 2nd WorkSheet
                    // add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPartConsensus = workbookpart.AddNewPart<WorksheetPart>("rId2");
                    worksheetPartConsensus.Worksheet = new Worksheet();
                    worksheetPartConsensus.Worksheet.Save();
                    sheetId = 2;

                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPartConsensus),
                        SheetId = sheetId,
                        Name = "Consensus Data"
                    });
                    GenerateConsensusHeaders(worksheetPartConsensus, consensusData, currencyConsensus);
                    InsertConsensusDataInWorksheet(worksheetPartConsensus, consensusData);
                    #endregion

                    #region ModelUpload

                    // add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPartModelUpload = workbookpart.AddNewPart<WorksheetPart>("rId1");
                    worksheetPartModelUpload.Worksheet = new Worksheet();
                    worksheetPartModelUpload.Worksheet.Save();
                    sheetId = 3;

                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPartModelUpload),
                        SheetId = sheetId,
                        Name = "Model Upload"
                    });
                    GenerateModelUploadHeaders(worksheetPartModelUpload);
                    GenerateDataModelUpload(worksheetPartModelUpload);
                    #endregion

                    #region Model Reference

                    // add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPartModelReference = workbookpart.AddNewPart<WorksheetPart>("rId0");
                    worksheetPartModelReference.Worksheet = new Worksheet();
                    worksheetPartModelReference.Worksheet.Save();
                    sheetId = 4;

                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPartModelReference),
                        SheetId = sheetId,
                        Name = "Model Reference"
                    });
                    GenerateModelReferenceData(worksheetPartModelReference);
                    #endregion

                    workbookpart.Workbook.Save();

                    // close the document.
                    spreadsheetDocument.Close();
                    return GetBytsForFile(fileName);
                }
            }
            catch (Exception ex)
            {
                GreenField.Web.Helpers.ExceptionTrace.LogException(ex);
                return null;
            }
        }
        #endregion

        #region ConsensusEstimates Sheet
        /// <summary>
        /// Generate Headers of Consensus Sheet
        /// </summary>
        /// <param name="worksheetPart">Worksheetpart- Worksheet Consensus</param>
        /// <param name="consensusData">Consensus Data from DataBase</param>
        private static void GenerateConsensusHeaders(WorksheetPart worksheetPart, List<ModelConsensusEstimatesData> consensusData, string currency)
        {
            var worksheet = worksheetPart.Worksheet;
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            var row = new Row { RowIndex = 1 };
            SheetFormatProperties sheetFormatProperties1;
            sheetData.Append(row);
            Columns sheetColumns = new Columns();
            Column mergeColumns;
            string maxLengthStr;

            if (consensusData.Count != 0)
            {
                var maxLength = consensusData.Max(s => s.ESTIMATE_DESC.Length);
                maxLengthStr = consensusData.FirstOrDefault(s => s.ESTIMATE_DESC.Length == maxLength).ESTIMATE_DESC;
            }
            else
            {
                maxLengthStr = "Data Description";
            }

            DoubleValue maxWidth = GetColumnWidth(maxLengthStr);
            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = maxWidth };
            sheetColumns.Append(firstColumn);

            int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
            int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;
            var cell = new Cell();
            cell = CreateHeaderCell("Data Id");
            row.InsertAt(cell, 0);
            cell = new Cell();
            cell = CreateHeaderCell("Data in " + Convert.ToString(currency) + " (Millions)");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " A");
                row.InsertAt(cell, i + 6);
                firstYear++;
                mergeColumns = new Column() { Min = Convert.ToUInt32(i + 3), Max = Convert.ToUInt32(i + 6), CustomWidth = true, OutlineLevel = 1, Hidden = false };
                sheetColumns.Append(mergeColumns);
            }
            sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(sheetColumns);
            worksheet.Append(sheetData);
        }

        /// <summary>
        /// Generate Data in ConsensusSheet
        /// </summary>
        /// <param name="worksheetPart">Worksheetpart-WroksheetConsensus</param>
        /// <param name="consensusData">ConsensusData from Database</param>
        private static void InsertConsensusDataInWorksheet(WorksheetPart worksheetPart, List<ModelConsensusEstimatesData> consensusData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 2 };
            sheetData.Append(row);
            int rowIndex = 2;
            List<string> dataDescriptors = consensusData.Select(a => a.ESTIMATE_DESC).Distinct().ToList();
            var maxRowCount = dataDescriptors.Count + 2;
            List<int> financialPeriodYears = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).Distinct().ToList();

            int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
            int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            while (row.RowIndex < maxRowCount)
            {
                foreach (string item in dataDescriptors)
                {
                    firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
                    var cell = CreateNumberCell(Convert.ToDecimal(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_ID).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(Convert.ToString(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_DESC).FirstOrDefault()));
                    row.InsertAt(cell, 1);
                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q1").
                            Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 2);

                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q2")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 3);

                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q3")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 4);

                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q4")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 5);

                        cell = new Cell();
                        cell = CreateNumberCell(consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "A")
                            .Select(a => a.AMOUNT).FirstOrDefault());
                        row.InsertAt(cell, i + 6);

                        firstYear++;
                    }
                    ++rowIndex;
                    row = new Row { RowIndex = Convert.ToUInt32(rowIndex) };
                    sheetData.Append(row);
                }
            }
        }

        #endregion

        #region Reuters Reported

        /// <summary>
        /// Insert Financial Values in WorkSheet
        /// </summary>
        /// <param name="worksheetPart">WorkSheetpart</param>
        /// <param name="financialData">FinancialStatementData</param>
        private static void InsertValuesInWorksheet(WorksheetPart worksheetPart, List<FinancialStatementDataModels> financialData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            //SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            var row = new Row { RowIndex = 2 };
            sheetData.Append(row);
            List<int> dataIds = financialData.Select(a => Convert.ToInt32((a.DataId))).Distinct().ToList();
            var maxRowCount = dataIds.Count + 2;
            int rowIndex = 2;
            List<int?> financialPeriodYears = financialData.Select(a => a.PeriodYear).OrderBy(a => a).Distinct().ToList();

            int? firstYear = financialData.Where(a => a.PeriodYear != null).Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int? lastYear = financialData.Where(a => a.PeriodYear != null).Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();

            int? numberOfYears = lastYear - firstYear;
            while (row.RowIndex < maxRowCount)
            {
                foreach (int item in dataIds)
                {
                    firstYear = financialData.Where(a => a.PeriodYear != null).Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                    var cell = CreateNumberCell(Convert.ToDecimal(financialData.Where(a => a.DataId == item).Select(a => a.DataId).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(financialData.Where(a => a.DataId == item).Select(a => a.Description).FirstOrDefault());
                    row.InsertAt(cell, 1);

                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q1").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 2);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q2").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 3);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q3").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 4);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "Q4").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 5);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.DataId == item && a.PeriodType.Trim() == "A").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 6);
                        firstYear++;
                    }
                    ++rowIndex;
                    row = new Row { RowIndex = Convert.ToUInt32(rowIndex) };
                    sheetData.Append(row);
                }
            }
        }

        /// <summary>
        /// Generate Headers in Reuters Sheet
        /// </summary>
        /// <param name="worksheetPart">WorkSheetPart</param>
        /// <param name="financialData">FinancialStatementData</param>
        private static void GenerateReutersHeaders(WorksheetPart worksheetPart, List<FinancialStatementDataModels> financialData, string currency)
        {
            var worksheet = worksheetPart.Worksheet;
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            SheetFormatProperties sheetFormatProperties1;
            var row = new Row { RowIndex = 1 };
            sheetData.Append(row);
            int? firstYear = financialData.Where(a => a.PeriodYear != null).Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int? lastYear = financialData.Where(a => a.PeriodYear != null).Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();
            int? numberOfYears = lastYear - firstYear;
            string maxLengthStr;
            if (financialData.Count != 0)
            {
                var maxLength = financialData.Max(s => s.Description.Length);
                maxLengthStr = financialData.FirstOrDefault(s => s.Description.Length == maxLength).Description;
            }
            else
            {
                maxLengthStr = "Data Description";
            }

            DoubleValue maxWidth = GetColumnWidth(maxLengthStr);
            MaxWidth = maxWidth;
            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = maxWidth };

            Columns sheetColumns = new Columns();
            Column mergeColumns;

            sheetColumns.Append(firstColumn);

            var cell = new Cell();

            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Data In " + Convert.ToString(currency) + " (Millions)");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                Columns column = new DocumentFormat.OpenXml.Spreadsheet.Columns();
                mergeColumns = new DocumentFormat.OpenXml.Spreadsheet.Column();

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateHeaderCell(firstYear + " A");
                row.InsertAt(cell, i + 6);
                firstYear++;

                mergeColumns = new Column() { Min = Convert.ToUInt32(i + 3), Max = Convert.ToUInt32(i + 6), CustomWidth = true, OutlineLevel = 1, Hidden = false };
                sheetColumns.Append(mergeColumns);
            }
            sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(sheetColumns);
            worksheet.Append(sheetData);
        }

        #endregion

        #region ExcelModel- Model Upload

        /// <summary>
        /// Generate Headers for Model Upload Sheet
        /// </summary>
        /// <param name="worksheetPart">WorkSheetpart</param>
        private static void GenerateModelUploadHeaders(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            var row = new Row { RowIndex = 1 };
            sheetData.Append(row);
            int firstYear = DateTime.Today.Year;
            SheetFormatProperties sheetFormatProperties1;
            Columns sheetColumns = new Columns();
            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = MaxWidth };
            sheetColumns.Append(firstColumn);

            var cell = new Cell();

            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 1);

            for (int i = 1; i <= 6; i++)
            {
                cell = new Cell();
                cell = CreateHeaderCell((firstYear + (i - 1)).ToString());
                row.InsertAt(cell, i + 1);
            }
            DataValidations dataValidations1 = new DataValidations() { Count = (UInt32Value)1U };
            dataValidations1 = ValidationsCommodityMeasure(dataValidations1);

            sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(sheetColumns);
            worksheet.Append(sheetData);
            worksheet.Append(dataValidations1);
            GenerateSecondRowModelUpload(worksheetPart);
            GenerateThirdRowModelUpload(worksheetPart);
            GenerateFourthRowModelUpload(worksheetPart);
            GenerateFifthRowModelUpload(worksheetPart);
            GenerateSixthRowModelUpload(worksheetPart);
        }

        /// <summary>
        /// generate Row for PeriodEndDate
        /// </summary>
        /// <param name="worksheetPart">Worksheetpart</param>
        private static void GenerateSecondRowModelUpload(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 2 };
            sheetData.Append(row);

            #region PeriodEndDate

            var cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Period EndDate");
            row.InsertAt(cell, 1);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 2);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 3);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 4);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 5);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 6);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 7);

            #endregion
        }

        /// <summary>
        /// Generate Row for Periodlenght
        /// </summary>
        /// <param name="worksheetPart">Worksheetpart</param>
        private static void GenerateThirdRowModelUpload(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 3 };
            sheetData.Append(row);

            #region PeriodLength

            var cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Period Length (In Months) ");
            row.InsertAt(cell, 1);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 2);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 3);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 4);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 5);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 6);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 7);

            #endregion
        }

        /// <summary>
        /// Generate Row for Acutal Override
        /// </summary>
        /// <param name="worksheetPart">Worksheetpart</param>
        private static void GenerateFourthRowModelUpload(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 4 };
            sheetData.Append(row);

            #region ActualOverride

            var cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Actual(Reported) Override");
            row.InsertAt(cell, 1);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 2);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 3);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 4);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 5);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 6);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 7);

            #endregion
        }

        /// <summary>
        /// Generate Row for CommodityMeasure
        /// </summary>
        /// <param name="worksheetPart">Worksheet part</param>
        private static void GenerateFifthRowModelUpload(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 5 };
            sheetData.Append(row);

            #region CommodityMeasure

            var cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Commodity Measure");
            row.InsertAt(cell, 1);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 2);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 3);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 4);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 5);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 6);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 7);

            #endregion
        }

        /// <summary>
        /// Generate Row for CommodityForecastUsed
        /// </summary>
        /// <param name="worksheetPart">WorkSheetpart</param>
        private static void GenerateSixthRowModelUpload(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 6 };
            sheetData.Append(row);

            #region PeriodLength

            var cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateHeaderCell("Commodity Forecast Used");
            row.InsertAt(cell, 1);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 2);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 3);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 4);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 5);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 6);

            cell = new Cell();
            cell = CreateHeaderCell(" ");
            row.InsertAt(cell, 7);

            #endregion
        }

        /// <summary>
        /// Generate modelUpload sheet from Row7
        /// </summary>
        /// <param name="worksheetPart">Worksheetpart</param>
        private static void GenerateDataModelUpload(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 7 };

            UInt32 rowIndex = 7;
            foreach (DataPointsModelUploadData item in ModelUploadDataPoints)
            {
                var cell = CreateTextCell(item.COA);
                row.InsertAt(cell, 0);
                cell = new Cell();
                cell = CreateTextCell(item.DATA_DESCRIPTION);
                row.InsertAt(cell, 1);
                sheetData.Append(row);
                ++rowIndex;
                row = new Row { RowIndex = rowIndex };
            }
        }

        /// <summary>
        /// Apply validations Commodity Measure
        /// </summary>
        /// <param name="validations">DataValidations applied in Sheet</param>
        /// <returns>DataValidations</returns>
        private static DataValidations ValidationsCommodityMeasure(DataValidations validations)
        {
            string commodities = "";
            foreach (string item in Commodities)
            {
                if (Commodities[0] != item)
                {
                    commodities = commodities + "," + item;
                }
                else
                {
                    commodities = item;
                }
            }
            StringBuilder commodityList = new StringBuilder();
            commodityList.Append('"');
            commodityList.Append(commodities);
            commodityList.Append('"');

            #region DataValidations

            DataValidation dataValidation1 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "C5" } };
            DataValidation dataValidation2 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "D5" } };
            DataValidation dataValidation3 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "E5" } };
            DataValidation dataValidation4 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "F5" } };
            DataValidation dataValidation5 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "G5" } };
            DataValidation dataValidation6 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "H5" } };
            Formula1 formula11 = new Formula1();
            formula11.Text = commodityList.ToString();
            Formula1 formula12 = new Formula1();
            formula12.Text = commodityList.ToString();
            Formula1 formula13 = new Formula1();
            formula13.Text = commodityList.ToString();
            Formula1 formula14 = new Formula1();
            formula14.Text = commodityList.ToString();
            Formula1 formula15 = new Formula1();
            formula15.Text = commodityList.ToString();
            Formula1 formula16 = new Formula1();
            formula16.Text = commodityList.ToString();
            dataValidation1.Append(formula11);
            dataValidation2.Append(formula12);
            dataValidation3.Append(formula13);
            dataValidation4.Append(formula14);
            dataValidation5.Append(formula15);
            dataValidation6.Append(formula16);

            validations.Append(dataValidation1);
            validations.Append(dataValidation2);
            validations.Append(dataValidation3);
            validations.Append(dataValidation4);
            validations.Append(dataValidation5);
            validations.Append(dataValidation6);

            validations = ValidationOverride(validations);
            return validations;
            #endregion
        }

        /// <summary>
        /// Add Data validation for OverrideValue
        /// </summary>
        /// <param name="validations">DataValidations added to sheet</param>
        /// <returns>DataValidations</returns>
        private static DataValidations ValidationOverride(DataValidations validations)
        {
            string formulaText = "Yes,No";
            StringBuilder overideValues = new StringBuilder();
            overideValues.Append('"');
            overideValues.Append(formulaText);
            overideValues.Append('"');

            DataValidation dataValidation7 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "C4" } };
            DataValidation dataValidation8 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "D4" } };
            DataValidation dataValidation9 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "E4" } };
            DataValidation dataValidation10 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "F4" } };
            DataValidation dataValidation11 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "G4" } };
            DataValidation dataValidation12 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, 
                ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "H4" } };

            Formula1 formula11 = new Formula1();
            formula11.Text = overideValues.ToString();
            Formula1 formula12 = new Formula1();
            formula12.Text = overideValues.ToString();
            Formula1 formula13 = new Formula1();
            formula13.Text = overideValues.ToString();
            Formula1 formula14 = new Formula1();
            formula14.Text = overideValues.ToString();
            Formula1 formula15 = new Formula1();
            formula15.Text = overideValues.ToString();
            Formula1 formula16 = new Formula1();
            formula16.Text = overideValues.ToString();

            dataValidation7.Append(formula11);
            dataValidation8.Append(formula12);
            dataValidation9.Append(formula13);
            dataValidation10.Append(formula14);
            dataValidation11.Append(formula15);
            dataValidation12.Append(formula16);

            validations.Append(dataValidation7);
            validations.Append(dataValidation8);
            validations.Append(dataValidation9);
            validations.Append(dataValidation10);
            validations.Append(dataValidation11);
            validations.Append(dataValidation12);

            return validations;
        }

        #endregion

        #region ExcelModel- ModelReference

        /// <summary>
        /// Method to generate content of ModelReference Sheet
        /// </summary>
        /// <param name="worksheetPart"></param>
        private static void GenerateModelReferenceData(WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;

            worksheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetData sheetData = new DocumentFormat.OpenXml.Spreadsheet.SheetData();
            SheetFormatProperties sheetFormatProperties1;
            Columns sheetColumns = new Columns();
            Column firstColumn = new Column() { Min = 2U, Max = 2U, Width = MaxWidth };
            Column secondColumn = new Column() { Min = 2U, Max = 2U, Width = 1.5 * MaxWidth };
            sheetColumns.Append(firstColumn);
            sheetColumns.Append(secondColumn);
            sheetFormatProperties1 = new SheetFormatProperties() { DefaultRowHeight = 15D, OutlineLevelColumn = 1, DyDescent = 0.25D };

            var row = new Row { RowIndex = 1 };
            var cell = CreateHeaderCell("Issuer ID");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(ModelReferenceData.IssuerId);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 2 };
            cell = CreateHeaderCell("Issuer Name");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(ModelReferenceData.IssuerName);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 3 };
            cell = CreateHeaderCell("COA Type");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(ModelReferenceData.COATypes);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 4 };
            cell = CreateHeaderCell("Currency");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(ModelReferenceData.Currencies.FirstOrDefault());
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 5 };
            cell = CreateHeaderCell("Units");
            row.InsertAt(cell, 0);
            sheetData.Append(row);

            row = new Row { RowIndex = 6 };
            cell = CreateHeaderCell("Q1 OverRide %");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(string.Empty);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 7 };
            cell = CreateHeaderCell("Q2 OverRide %");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(string.Empty);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 8 };
            cell = CreateHeaderCell("Q3 OverRide %");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(string.Empty);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            row = new Row { RowIndex = 9 };
            cell = CreateHeaderCell("Q4 OverRide %");
            row.InsertAt(cell, 0);
            cell = CreateTextCell(string.Empty);
            row.InsertAt(cell, 1);
            sheetData.Append(row);

            DataValidations dataValidations1 = new DataValidations() { Count = (UInt32Value)1U };
            DataValidation dataValidation2 = new DataValidation() { Type = DataValidationValues.List, AllowBlank = true, ShowInputMessage = true, ShowErrorMessage = true, SequenceOfReferences = new ListValue<StringValue>() { InnerText = "B5" } };
            Formula1 formula12 = new Formula1();
            formula12.Text = "\"Units,Thousands,Millions,Billions\"";

            dataValidation2.Append(formula12);

            dataValidations1.Append(dataValidation2);
            worksheet.Append(sheetFormatProperties1);
            worksheet.Append(sheetColumns);
            worksheet.Append(sheetData);
            worksheet.Append(dataValidations1);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Create Text Cell
        /// </summary>
        /// <param name="cellValue">CellValue</param>
        /// <returns>Cell</returns>
        private static Cell CreateTextCell(string cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(cellValue);
            return cell;
        }

        /// <summary>
        /// Creating Number Cell
        /// </summary>
        /// <param name="cellValue">CellValue</param>
        /// <returns>Cell</returns>
        private static Cell CreateNumberCell(Decimal? cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.Number;
            cell.CellValue = new CellValue(Convert.ToString(cellValue));
            return cell;
        }

        /// <summary>
        /// Creating Header Cell
        /// </summary>
        /// <param name="cellValue">value to fill in teh cell</param>
        /// <returns></returns>
        private static Cell CreateHeaderCell(String cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(cellValue);
            cell.StyleIndex = (UInt32Value)2U;
            return cell;
        }

        /// <summary>
        /// Get Width of the column for longest string
        /// </summary>
        /// <param name="sILT">longest String</param>
        /// <returns>width</returns>
        private static DoubleValue GetColumnWidth(string sILT)
        {
            double fSimpleWidth = 0.0f;
            double fWidthOfZero = 0.0f;
            double fDigitWidth = 0.0f;
            double fMaxDigitWidth = 0.0f;
            double fTruncWidth = 0.0f;

            System.Drawing.Font drawfont = new System.Drawing.Font("Calibri", 11);
            Graphics g = Graphics.FromImage(new Bitmap(200, 200));
            fWidthOfZero = (double)g.MeasureString("0", drawfont).Width;
            fSimpleWidth = (double)g.MeasureString(sILT, drawfont).Width;
            fSimpleWidth = fSimpleWidth / fWidthOfZero;

            for (int i = 0; i < 10; ++i)
            {
                fDigitWidth = (double)g.MeasureString(i.ToString(), drawfont).Width;
                if (fDigitWidth > fMaxDigitWidth)
                {
                    fMaxDigitWidth = fDigitWidth;
                }
            }
            g.Dispose();
            fTruncWidth = Math.Truncate((sILT.ToCharArray().Count() * fMaxDigitWidth + 5.0) / fMaxDigitWidth * 256.0) / 256.0;

            return fTruncWidth;
        }

        /// <summary>
        /// StyleSheet for the excel File
        /// </summary>
        /// <returns>StyleSheet</returns>
        private static Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts1 = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };

            DocumentFormat.OpenXml.Spreadsheet.Font font1 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontSize fontSize1 = new FontSize() { Val = 11D };
            DocumentFormat.OpenXml.Spreadsheet.Color color1 = new DocumentFormat.OpenXml.Spreadsheet.Color() { Theme = (UInt32Value)1U };
            FontName fontName1 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering1 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme1 = new FontScheme() { Val = FontSchemeValues.Minor };

            font1.Append(fontSize1);
            font1.Append(color1);
            font1.Append(fontName1);
            font1.Append(fontFamilyNumbering1);
            font1.Append(fontScheme1);

            DocumentFormat.OpenXml.Spreadsheet.Font font2 = new DocumentFormat.OpenXml.Spreadsheet.Font();
            FontSize fontSize2 = new FontSize() { Val = 11D };
            DocumentFormat.OpenXml.Spreadsheet.Color color2 = new DocumentFormat.OpenXml.Spreadsheet.Color() { Theme = (UInt32Value)1U };
            FontName fontName2 = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering2 = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme2 = new FontScheme() { Val = FontSchemeValues.Minor };
            Bold bold2 = new Bold();

            font2.Append(fontSize2);
            font2.Append(color2);
            font2.Append(fontName2);
            font2.Append(fontFamilyNumbering2);
            font2.Append(fontScheme2);
            font2.Append(bold2);

            fonts1.Append(font1);
            fonts1.Append(font2);

            Fills fills1 = new Fills() { Count = (UInt32Value)5U };

            // FillId = 0
            Fill fill1 = new Fill();
            PatternFill patternFill1 = new PatternFill() { PatternType = PatternValues.None };
            fill1.Append(patternFill1);

            // FillId = 1
            Fill fill2 = new Fill();
            PatternFill patternFill2 = new PatternFill() { PatternType = PatternValues.Gray125 };
            fill2.Append(patternFill2);

            // FillId = 2,RED
            Fill fill3 = new Fill();
            PatternFill patternFill3 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor1 = new ForegroundColor() { Rgb = "FFFF0000" };
            BackgroundColor backgroundColor1 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill3.Append(foregroundColor1);
            patternFill3.Append(backgroundColor1);
            fill3.Append(patternFill3);

            // FillId = 3,BLUE
            Fill fill4 = new Fill();
            PatternFill patternFill4 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor2 = new ForegroundColor() { Theme = (UInt32Value)3U, Tint = 0.79998168889431442D };// { Rgb = "FF0070C0" };
            BackgroundColor backgroundColor2 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill4.Append(foregroundColor2);
            patternFill4.Append(backgroundColor2);
            fill4.Append(patternFill4);

            // FillId = 4,YELLO
            Fill fill5 = new Fill();
            PatternFill patternFill5 = new PatternFill() { PatternType = PatternValues.Solid };
            ForegroundColor foregroundColor3 = new ForegroundColor() { Rgb = "FFFFFF00" };
            BackgroundColor backgroundColor3 = new BackgroundColor() { Indexed = (UInt32Value)64U };
            patternFill5.Append(foregroundColor3);
            patternFill5.Append(backgroundColor3);
            fill5.Append(patternFill5);

            fills1.Append(fill1);
            fills1.Append(fill2);
            fills1.Append(fill3);
            fills1.Append(fill4);
            fills1.Append(fill5);

            Borders borders1 = new Borders() { Count = (UInt32Value)1U };

            Border border1 = new Border();
            LeftBorder leftBorder1 = new LeftBorder();
            RightBorder rightBorder1 = new RightBorder();
            TopBorder topBorder1 = new TopBorder();
            BottomBorder bottomBorder1 = new BottomBorder();
            DiagonalBorder diagonalBorder1 = new DiagonalBorder();

            border1.Append(leftBorder1);
            border1.Append(rightBorder1);
            border1.Append(topBorder1);
            border1.Append(bottomBorder1);
            border1.Append(diagonalBorder1);

            borders1.Append(border1);

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats1 = new CellFormats() { Count = (UInt32Value)4U };
            CellFormat cellFormat2 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, 
                BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U };
            CellFormat cellFormat3 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)2U, 
                BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat4 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)1U, FillId = (UInt32Value)3U, 
                BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };
            CellFormat cellFormat5 = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)4U, 
                BorderId = (UInt32Value)0U, FormatId = (UInt32Value)0U, ApplyFill = true };

            cellFormats1.Append(cellFormat2);
            cellFormats1.Append(cellFormat3);
            cellFormats1.Append(cellFormat4);
            cellFormats1.Append(cellFormat5);

            CellStyles cellStyles1 = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = (UInt32Value)0U };
            TableStyles tableStyles1 = new TableStyles() { Count = (UInt32Value)0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleMedium9" };

            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            X14.SlicerStyles slicerStyles1 = new X14.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };

            stylesheetExtension1.Append(slicerStyles1);

            stylesheetExtensionList1.Append(stylesheetExtension1);

            stylesheet1.Append(fonts1);
            stylesheet1.Append(fills1);
            stylesheet1.Append(borders1);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats1);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);
            stylesheet1.Append(stylesheetExtensionList1);
            return stylesheet1;
        }

        /// <summary>
        /// Generate Filename
        /// </summary>
        /// <returns>Name of File</returns>
        private static string GetFileName()
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid() + "_Model.xlsx";
            return fileName;
        }

        /// <summary>
        /// Generate byte-Array from Excel
        /// </summary>
        /// <param name="filePath">Path of the File</param>
        /// <returns>Byte Stream of the File</returns>
        private static byte[] GetBytsForFile(string filePath)
        {
            try
            {
                FileStream fileStream;
                byte[] fileByte;
                using (fileStream = File.OpenRead(filePath))
                {
                    fileByte = new byte[fileStream.Length];
                    fileStream.Read(fileByte, 0, Convert.ToInt32(fileStream.Length));
                }
                return fileByte;
            }
            catch (Exception ex)
            {
                GreenField.Web.Helpers.ExceptionTrace.LogException(ex);
                return null;
            }
        }

        #endregion
    }
}