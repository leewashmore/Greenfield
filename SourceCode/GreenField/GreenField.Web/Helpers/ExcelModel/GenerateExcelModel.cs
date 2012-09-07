using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Data;
using System.IO;
using Microsoft.Office.Interop.Excel;
using GreenField.DAL;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Generate ExcelModel
    /// </summary>
    public static class GenerateExcelModel
    {
        public static void Test()
        {
            Excel.Application myExcel = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook myworkbook = myExcel.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Sheets mysheets = myworkbook.Worksheets;


            for (int i = 1; i <= 12; i++)
            {
                myworkbook.Sheets.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                Excel._Worksheet oSheet = (Excel._Worksheet)mysheets.get_Item(i);
                oSheet.Name = i.ToString();
                oSheet.Cells[1, 1] = "Date" + i.ToString();
            }
            myExcel.Visible = true;

            string fileName = GetFileName();
            myworkbook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, false, false,
                    Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true,
                    Missing.Value, Missing.Value, Missing.Value);
            myworkbook.Saved = true;
            myworkbook.Close(false, Type.Missing, Type.Missing);


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

        /// <summary>
        /// Generate Excel
        /// </summary>
        /// <param name="financialData"></param>
        public static byte[] GenerateExcel(List<FinancialStatementData> financialData, List<ModelConsensusEstimatesData> consensusData)
        {
            try
            {
                var excelApp = new Excel.Application();
                Workbook workBook = excelApp.Workbooks.Add(Type.Missing);
                //Sheets mySheets = workBook.Worksheets;

                int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();
                int numberOfYears = lastYear - firstYear;

                Excel._Worksheet workSheetReuters;
                Excel._Worksheet workSheetConsensus;
                //Sheets xlsheets = null;
                //xlsheets = workBook.Sheets;

                //workBook.Sheets.Add(Type.Missing, Type.Missing, 2, Type.Missing);

                //mySheets = workBook.Worksheets;

                //workSheetReuters = (Worksheet)xlsheets.Add(xlsheets[1], Type.Missing, Type.Missing, Type.Missing);
                //workSheetReuters = (_Worksheet)mySheets.get_Item(1);

                
                workSheetReuters = workBook.ActiveSheet as Excel._Worksheet;
                workSheetReuters = GenerateReutersColumnHeaders(workSheetReuters, firstYear, lastYear);
                workSheetReuters = DisplayReutersData(workSheetReuters, financialData);
                workSheetReuters.Name = "Reuters Reported";

                //xlsheets = workBook.Sheets;
                //workSheetConsensus = (Worksheet)xlsheets.Add(xlsheets[1], Type.Missing, Type.Missing, Type.Missing);



                workSheetConsensus = workBook.Sheets.Add(Type.Missing, Type.Missing, 1, Type.Missing);
                workSheetConsensus = GenerateConsensusColumnHeaders(workSheetReuters, firstYear, lastYear);
                workSheetConsensus = DisplayConsensusData(workSheetReuters, consensusData);
                workSheetConsensus.Name = "Consensus Reported";

                




                string fileName = GetFileName();


                



                workBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookDefault, Missing.Value, Missing.Value, false, false,
                        Excel.XlSaveAsAccessMode.xlNoChange, Excel.XlSaveConflictResolution.xlUserResolution, true,
                        Missing.Value, Missing.Value, Missing.Value);
                workBook.Saved = true;
                workBook.Close(false, Type.Missing, Type.Missing);

                return GetBytsForFile(fileName);
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Generate Address for the File
        /// </summary>
        /// <returns></returns>
        private static string GetFileName()
        {
            try
            {
                string fileName = Path.GetTempPath() + Guid.NewGuid() + "_Model.xlsx";
                return fileName;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Generate Column Headers for the Sheet
        /// </summary>
        /// <param name="workSheet">Current Excel Worksheet</param>
        /// <param name="firstYear">First Year in the Data-Set</param>
        /// <param name="lastYear">Last Period year in the Data-Set</param>
        /// <returns>The Worksheet with Column Headers</returns>
        private static _Worksheet GenerateReutersColumnHeaders(_Worksheet workSheet, int firstYear, int lastYear)
        {
            try
            {
                var row = 1;
                workSheet.Cells[row, "A"] = "Data Id";
                workSheet.Cells[row, "B"] = "Data Description";
                int numberOfYears = lastYear - firstYear;
                for (int i = 1; i <= numberOfYears * 5 + 1; i = i + 5)
                {
                    workSheet.Cells[row, i + 2] = firstYear + " Q1";
                    workSheet.Cells[row, i + 3] = firstYear + " Q2";
                    workSheet.Cells[row, i + 4] = firstYear + " Q3";
                    workSheet.Cells[row, i + 5] = firstYear + " Q4";
                    workSheet.Cells[row, i + 6] = firstYear + " A";
                    firstYear++;
                }
                return workSheet;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method to Pivot the Unpivoted Data
        /// </summary>
        /// <param name="worksheet">Current Excel Worksheet</param>
        /// <param name="financialData">list of type FinancialStatementData</param>
        /// <returns>The Worksheet with Pivoted Data</returns>
        private static Excel._Worksheet DisplayReutersData(Excel._Worksheet worksheet, List<FinancialStatementData> financialData)
        {
            try
            {
                List<string> dataDescriptors = financialData.Select(a => a.Description).Distinct().ToList();
                var row = 2;
                var maxRowCount = dataDescriptors.Count + 2;
                List<int> financialPeriodYears = financialData.Select(a => a.PeriodYear).OrderBy(a => a).Distinct().ToList();

                int firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                int lastYear = financialData.Select(a => a.PeriodYear).OrderByDescending(a => a).FirstOrDefault();

                int numberOfYears = lastYear - firstYear;

                while (row < maxRowCount)
                {
                    foreach (string item in dataDescriptors)
                    {
                        firstYear = financialData.Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                        worksheet.Cells[row, 1] = financialData.Where(a => a.Description == item).Select(a => a.DataId).FirstOrDefault();
                        worksheet.Cells[row, 2] = financialData.Where(a => a.Description == item).Select(a => a.Description).FirstOrDefault();

                        string aa = financialData.Where(a => a.Description == item).Select(a => a.DataId).FirstOrDefault().ToString();
                        for (int i = 1; i <= numberOfYears * 5 + 1; i = i + 5)
                        {
                            worksheet.Cells[row, i + 2] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q1").
                                Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 3] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q2")
                                .Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 4] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q3")
                                .Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 5] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "Q4")
                                .Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 6] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item && a.PeriodType.Trim() == "A")
                                .Select(a => a.Amount).FirstOrDefault();
                            firstYear++;
                        }
                        row++;
                    }
                }
                return worksheet;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Generate ConsensusData Column Headers
        /// </summary>
        /// <param name="workSheet">Current Excel Worksheet</param>
        /// <param name="firstYear"></param>
        /// <param name="lastYear"></param>
        /// <returns>The Worksheet with Column Headers</returns>
        private static _Worksheet GenerateConsensusColumnHeaders(_Worksheet workSheet, int firstYear, int lastYear)
        {
            try
            {
                var row = 1;
                workSheet.Cells[row, "A"] = "Data Id";
                workSheet.Cells[row, "B"] = "Data Description";
                int numberOfYears = lastYear - firstYear;
                for (int i = 1; i <= numberOfYears * 5 + 1; i = i + 5)
                {
                    workSheet.Cells[row, i + 2] = firstYear + " Q1";
                    workSheet.Cells[row, i + 3] = firstYear + " Q2";
                    workSheet.Cells[row, i + 4] = firstYear + " Q3";
                    workSheet.Cells[row, i + 5] = firstYear + " Q4";
                    workSheet.Cells[row, i + 6] = firstYear + " A";
                    firstYear++;
                }
                return workSheet;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Method to Pivot the Unpivoted Data
        /// </summary>
        /// <param name="worksheet">Current Excel Worksheet</param>
        /// <param name="financialData">List of type ModelConsensusEstimatesData</param>
        /// <returns>The Worksheet with Pivoted Data</returns>
        private static _Worksheet DisplayConsensusData(_Worksheet worksheet, List<ModelConsensusEstimatesData> consensusData)
        {
            try
            {
                List<string> dataDescriptors = consensusData.Select(a => a.ESTIMATE_DESC).Distinct().ToList();
                var row = 2;
                var maxRowCount = dataDescriptors.Count + 2;
                List<int> financialPeriodYears = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).Distinct().ToList();

                int firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
                int lastYear = consensusData.Select(a => a.PERIOD_YEAR).OrderByDescending(a => a).FirstOrDefault();
                int numberOfYears = lastYear - firstYear;

                while (row < maxRowCount)
                {
                    foreach (string item in dataDescriptors)
                    {
                        firstYear = consensusData.Select(a => a.PERIOD_YEAR).OrderBy(a => a).FirstOrDefault();
                        worksheet.Cells[row, 1] = consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_ID).FirstOrDefault();
                        worksheet.Cells[row, 2] = consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_DESC).FirstOrDefault();

                        string aa = consensusData.Where(a => a.ESTIMATE_DESC == item).Select(a => a.ESTIMATE_ID).FirstOrDefault().ToString();
                        for (int i = 1; i <= numberOfYears * 5 + 1; i = i + 5)
                        {
                            worksheet.Cells[row, i + 2] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q1").
                                Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 3] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q2")
                                .Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 4] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q3")
                                .Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 5] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "Q4")
                                .Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 6] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item && a.PERIOD_TYPE.Trim() == "A")
                                .Select(a => a.AMOUNT).FirstOrDefault();
                            firstYear++;
                        }
                        row++;
                    }
                }
                return worksheet;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                return null;
            }
        }
    }
}