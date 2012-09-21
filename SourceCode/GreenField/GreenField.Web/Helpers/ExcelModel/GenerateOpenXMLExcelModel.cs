using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GreenField.DAL;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace GreenField.Web.Helpers
{
    public static class GenerateOpenXMLExcelModel
    {
        public static byte[] GenerateExcel(List<FinancialStatementData> financialData, List<ModelConsensusEstimatesData> consensusData)
        {
            try
            {
                string fileName = GetFileName();

                // Create a spreadsheet document by supplying the filepath.
                // By default, AutoSave = true, Editable = true, and Type = xlsx.
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                    Create(fileName, SpreadsheetDocumentType.Workbook))
                {
                    UInt32 sheetId;
                    // Add a WorkbookPart to the document.
                    WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                    workbookpart.Workbook = new Workbook();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>("rId3");
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    // Add Sheets to the Workbook.
                    spreadsheetDocument.WorkbookPart.Workbook.AppendChild(new Sheets());
                    sheetId = 1;
                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                            {
                                Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                                SheetId = sheetId,
                                Name = "Reuters Repoted"
                            });
                    GenerateReutersHeaders(worksheetPart, financialData);
                    InsertValuesInWorksheet(worksheetPart, financialData);
                    workbookpart.Workbook.Save();

                    // Add a WorksheetPart to the WorkbookPart.
                    WorksheetPart worksheetPartConsensus = workbookpart.AddNewPart<WorksheetPart>("rId2");
                    worksheetPartConsensus.Worksheet = new Worksheet(new SheetData());
                    worksheetPartConsensus.Worksheet.Save();
                    sheetId = 2;

                    spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new DocumentFormat.OpenXml.Spreadsheet.Sheet()
                    {
                        Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPartConsensus),
                        SheetId = sheetId,
                        Name = "Consensus Data"
                    });
                    GenerateConsensusHeaders(worksheetPartConsensus, consensusData);
                    InsertConsensusDataInWorksheet(worksheetPartConsensus, consensusData);
                    workbookpart.Workbook.Save();
                    workbookpart.Workbook.Save();

                    // Close the document.
                    spreadsheetDocument.Close();
                    return GetBytsForFile(fileName);
                }
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="consensusData"></param>
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
                    var cell = CreateTextCell(Convert.ToString(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_ID).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(Convert.ToString(consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_DESC).FirstOrDefault()));
                    row.InsertAt(cell, 1);

                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        decimal? ab = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q1").
                            Select(a => a.AMOUNT).FirstOrDefault();

                        if (ab == null)
                        { 
                        
                        }

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

        /// <summary>
        /// Insert Financial Values in WorkSheet
        /// </summary>
        /// <param name="worksheetPart"></param>
        /// <param name="financialData"></param>
        private static void InsertValuesInWorksheet(WorksheetPart worksheetPart, List<FinancialStatementData> financialData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 2 };
            sheetData.Append(row);
            List<string> dataDescriptors = financialData.Select(a => a.Description).Distinct().ToList();
            var maxRowCount = dataDescriptors.Count + 2;
            int rowIndex = 2;
            List<int> financialPeriodYears = financialData.Select(a => a.PeriodYear).OrderBy(a => a).Distinct().ToList();

            int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();

            int numberOfYears = lastYear - firstYear;
            while (row.RowIndex < maxRowCount)
            {
                foreach (string item in dataDescriptors)
                {
                    firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                    var cell = CreateTextCell(Convert.ToString(financialData.Where(a => a.Description == item).Select(a => a.DataId).FirstOrDefault()));
                    row.InsertAt(cell, 0);
                    cell = new Cell();
                    cell = CreateTextCell(financialData.Where(a => a.Description == item).Select(a => a.Description).FirstOrDefault());
                    row.InsertAt(cell, 1);

                    for (int i = 0; i <= numberOfYears * 5; i = i + 5)
                    {
                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q1").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 2);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q2").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 3);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q3").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 4);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q4").
            Select(a => a.Amount).FirstOrDefault());
                        row.InsertAt(cell, i + 5);

                        cell = new Cell();
                        cell = CreateNumberCell(financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "A").
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

        private static void GenerateReutersHeaders(WorksheetPart worksheetPart, List<FinancialStatementData> financialData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 1 };
            sheetData.Append(row);

            int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
            int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            var cell = new Cell();

            cell = CreateTextCell("Data Id");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateTextCell("Data Description");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " A");
                row.InsertAt(cell, i + 6);
                firstYear++;
            }
        }

        private static void GenerateConsensusHeaders(WorksheetPart worksheetPart, List<ModelConsensusEstimatesData> consensusData)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var row = new Row { RowIndex = 1 };
            sheetData.Append(row);

            int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
            int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
            int numberOfYears = lastYear - firstYear;

            var cell = new Cell();

            cell = CreateTextCell("Data Id");
            row.InsertAt(cell, 0);

            cell = new Cell();
            cell = CreateTextCell("Data Description");
            row.InsertAt(cell, 1);

            for (int i = 0; i <= numberOfYears * 5; i = i + 5)
            {
                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q1");
                row.InsertAt(cell, i + 2);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q2");
                row.InsertAt(cell, i + 3);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q3");
                row.InsertAt(cell, i + 4);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " Q4");
                row.InsertAt(cell, i + 5);

                cell = new Cell();
                cell = CreateTextCell(firstYear + " A");
                row.InsertAt(cell, i + 6);
                firstYear++;
            }
        }

        private static Cell CreateTextCell(string cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(cellValue);
            return cell;
        }

        private static Cell CreateNumberCell(Decimal? cellValue)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.Number;
            cell.CellValue = new CellValue(Convert.ToString(cellValue));
            return cell;
        }

        private static string GetFileName()
        {
            string fileName = Path.GetTempPath() + Guid.NewGuid() + "_Model.xlsx";
            return fileName;
        }

        /// <summary>
        /// Generate byte-Array from Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

    }
}