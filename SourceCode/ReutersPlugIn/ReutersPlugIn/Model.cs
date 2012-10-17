using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Interop.Excel;
using ReutersPlugIn.ModelRefreshDefinitions;

namespace ReutersPlugIn
{
    /// <summary>
    /// Update workbook by posting retrieved model data from service
    /// </summary>
    public static class Model
    {
        /// <summary>
        /// Generate Excel
        /// </summary>
        /// <param name="financialData"></param>
        public static Boolean Update(ExcelModelData excelModelData)
        {
            try
            {
                Globals.ThisAddIn.Application.ScreenUpdating = false;

                Globals.ThisAddIn.AppendMessage("Validating model data received from service...");
                List<FinancialStatementDataModels> financialData = excelModelData.ReutersData;
                List<ModelConsensusEstimatesData> consensusData = excelModelData.ConsensusEstimateData;
                List<DataPointsModelUploadData> modelUploadData = excelModelData.ModelUploadDataPoints;

                if (financialData == null || consensusData == null || modelUploadData == null)
                {
                    throw new Exception("Model data received from service is corrupt");
                }

                var excelApp = Globals.ThisAddIn.Application;
                Workbook workBook = excelApp.ActiveWorkbook;//excelApp.Workbooks[1];

                Globals.ThisAddIn.AppendMessage("Validating reuters reported data...");
                List<int?> distinctPeriodYears = financialData.Where(a => a.PeriodYear != null)
                    .Select(a => a.PeriodYear).OrderBy(a => a).ToList().Distinct().ToList();
                if (distinctPeriodYears == null || distinctPeriodYears.Count == 0)
                {
                    throw new Exception("No reuters data available");
                }

                int? firstYear = distinctPeriodYears[0];
                int? lastYear = distinctPeriodYears[distinctPeriodYears.Count - 1];
                int? numberOfYears = lastYear - firstYear;

                //'Reuters Reported' worksheet
                Worksheet workSheetReuters = workBook.Worksheets[ThisAddIn.REUTERS_REPORTED_WORKSHEET_NAME];
                Globals.ThisAddIn.AppendMessage("Updating reuters reported headers...");
                workSheetReuters.Cells.ClearContents();
                workSheetReuters = UpdateReutersHeaders(workSheetReuters, Convert.ToInt32(firstYear), Convert.ToInt32(lastYear));
                if (workSheetReuters == null)
                {
                    throw new Exception("An Error occured while updating reuters reported headers");
                }
                Globals.ThisAddIn.AppendMessage("Updating reuters reported data...");                
                workSheetReuters = UpdateReutersData(workSheetReuters, financialData);
                if (workSheetReuters == null)
                {
                    throw new Exception("An Error occured while updating reuters reported data");
                }
                Globals.ThisAddIn.AppendMessage("Reuters reported data update complete");

                //'Consensus Data' worksheet
                Worksheet workSheetConsensus = workBook.Worksheets[ThisAddIn.CONSENSUS_DATA_WORKSHEET_NAME];
                Globals.ThisAddIn.AppendMessage("Updating consensus headers...");
                workSheetConsensus.Cells.ClearContents();
                workSheetConsensus = UpdateConsensusHeaders(workSheetConsensus, Convert.ToInt32(firstYear), Convert.ToInt32(lastYear));
                if (workSheetConsensus == null)
                {
                    throw new Exception("An Error occured while updating consensus headers");
                }
                Globals.ThisAddIn.AppendMessage("Updating consensus data...");                
                workSheetConsensus = UpdateConsensusData(workSheetConsensus, consensusData);
                if (workSheetConsensus == null)
                {
                    throw new Exception("An Error occured while updating consensus data");
                }
                Globals.ThisAddIn.AppendMessage("Consensus data update complete");

                //'Model Upload' worksheet
                Worksheet workSheetModelUpload = workBook.Worksheets[ThisAddIn.MODEL_UPLOAD_WORKSHEET_NAME];
                Globals.ThisAddIn.AppendMessage("Updating model upload data...");
                workSheetModelUpload = UpdateModelUploadData(workSheetModelUpload, modelUploadData);
                if (workSheetModelUpload == null)
                {
                    throw new Exception("An Error occured while updating model upload data");
                }
                Globals.ThisAddIn.AppendMessage("Model upload data update complete");
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.AppendMessage(ex.Message + "|" + ex.StackTrace, true);
                return false;
            }
            finally
            {
                Globals.ThisAddIn.Application.ScreenUpdating = true;
            }
            return true;
        }

        /// <summary>
        /// Generate Column Headers for the Sheet
        /// </summary>
        /// <param name="workSheet">Current Excel Worksheet</param>
        /// <param name="firstYear">First Year in the Data-Set</param>
        /// <param name="lastYear">Last Period year in the Data-Set</param>
        /// <returns>The Worksheet with Column Headers</returns>
        private static Worksheet UpdateReutersHeaders(Worksheet workSheet, int firstYear, int lastYear)
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
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.AppendMessage(ex.Message + "|" + ex.StackTrace, true);
            }
            return workSheet;
        }

        /// <summary>
        /// Method to Pivot the Unpivoted Data
        /// </summary>
        /// <param name="worksheet">Current Excel Worksheet</param>
        /// <param name="financialData">list of type FinancialStatementData</param>
        /// <returns>The Worksheet with Pivoted Data</returns>
        private static Worksheet UpdateReutersData(Worksheet worksheet, List<FinancialStatementDataModels> financialData)
        {
            try
            {
                List<int?> distinctPeriodYears = financialData.Where(a => a.PeriodYear != null)
                    .Select(a => a.PeriodYear).OrderBy(a => a).ToList().Distinct().ToList();
                if (distinctPeriodYears == null || distinctPeriodYears.Count == 0)
                {
                    throw new Exception("No reuters data available.");
                }

                int? firstYear = distinctPeriodYears[0];
                int? lastYear = distinctPeriodYears[distinctPeriodYears.Count - 1];
                int? numberOfYears = lastYear - firstYear;

                List<string> distinctDescriptors = financialData.Select(a => a.Description).Distinct().ToList();

                var row = 2;
                var maxRowCount = distinctDescriptors.Count + 2;

                while (row < maxRowCount)
                {
                    foreach (string item in distinctDescriptors)
                    {
                        firstYear = financialData.Where(a => a.PeriodYear != null).Select(a => a.PeriodYear).OrderBy(a => a).FirstOrDefault();
                        worksheet.Cells[row, 1] = financialData.Where(a => a.Description == item).Select(a => a.DataId).FirstOrDefault();
                        worksheet.Cells[row, 2] = financialData.Where(a => a.Description == item).Select(a => a.Description).FirstOrDefault();

                        string aa = financialData.Where(a => a.Description == item).Select(a => a.DataId).FirstOrDefault().ToString();
                        for (int i = 1; i <= numberOfYears * 5 + 1; i = i + 5)
                        {
                            worksheet.Cells[row, i + 2] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item &&
                                a.PeriodType.Trim() == "Q1").Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 3] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item &&
                                a.PeriodType.Trim() == "Q2").Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 4] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item &&
                                a.PeriodType.Trim() == "Q3").Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 5] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item &&
                                a.PeriodType.Trim() == "Q4").Select(a => a.Amount).FirstOrDefault();
                            worksheet.Cells[row, i + 6] = financialData.Where(a => a.PeriodYear == (firstYear) && a.Description == item &&
                                a.PeriodType.Trim() == "A").Select(a => a.Amount).FirstOrDefault();
                            firstYear++;
                        }
                        row++;
                    }
                }
                return worksheet;
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.AppendMessage(ex.Message + "|" + ex.StackTrace, true);
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
        private static Worksheet UpdateConsensusHeaders(Worksheet workSheet, int firstYear, int lastYear)
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
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.AppendMessage(ex.Message + "|" + ex.StackTrace, true);
                return null;
            }
            return workSheet;
        }

        /// <summary>
        /// Update consensus data
        /// </summary>
        /// <param name="worksheet">consensus data worksheet</param>
        /// <param name="financialData">list of type ModelConsensusEstimatesData</param>
        /// <returns>worksheet with updated Data</returns>
        private static Worksheet UpdateConsensusData(Worksheet worksheet, List<ModelConsensusEstimatesData> consensusData)
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
                            worksheet.Cells[row, i + 2] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item &&
                                a.PERIOD_TYPE.Trim() == "Q1").Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 3] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item &&
                                a.PERIOD_TYPE.Trim() == "Q2").Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 4] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item &&
                                a.PERIOD_TYPE.Trim() == "Q3").Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 5] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item &&
                                a.PERIOD_TYPE.Trim() == "Q4").Select(a => a.AMOUNT).FirstOrDefault();
                            worksheet.Cells[row, i + 6] = consensusData.Where(a => a.PERIOD_YEAR == (firstYear) && a.ESTIMATE_DESC == item &&
                                a.PERIOD_TYPE.Trim() == "A").Select(a => a.AMOUNT).FirstOrDefault();
                            firstYear++;
                        }
                        row++;
                    }
                }
                return worksheet;
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.AppendMessage(ex.Message + "|" + ex.StackTrace, true);
                return null;
            }
        }

        /// <summary>
        /// Update model upload data points
        /// </summary>
        /// <param name="worksheet">model upload worksheet</param>
        /// <param name="modelUploadData">list of DataPointsModelUploadData</param>
        /// <returns>worksheet with updated data</returns>
        private static Worksheet UpdateModelUploadData(Worksheet worksheet, List<DataPointsModelUploadData> modelUploadData)
        {
            try
            {
                int rowCount = 7;
                foreach (DataPointsModelUploadData record in modelUploadData.OrderBy(g => g.SORT_ORDER)
                    .Where(g => g.COA != null && g.DATA_DESCRIPTION != null))
                {
                    String coaType = (worksheet.Cells[rowCount, 1] as Range).Value;
                    String dataDescription = (worksheet.Cells[rowCount, 2] as Range).Value;

                    if (coaType == null || dataDescription == null ||
                        (modelUploadData.Any(g => g.COA.ToLower().Trim() == coaType.ToLower().Trim() &&
                            g.DATA_DESCRIPTION.ToLower().Trim() == dataDescription.ToLower().Trim()) == false))
                    {
                        if (coaType != null && dataDescription != null)
                        {
                            Globals.ThisAddIn.AppendMessage(ThisAddIn.MODEL_UPLOAD_WORKSHEET_NAME
                                + ": COA '" + coaType.Trim() + "', Description '" + dataDescription.Trim()
                                + "' no longer exists and has been removed", true);
                            (worksheet.Rows[rowCount] as Range).Delete(XlDeleteShiftDirection.xlShiftUp);
                        }
                        coaType = (worksheet.Cells[rowCount, 1] as Range).Value;
                        dataDescription = (worksheet.Cells[rowCount, 2] as Range).Value;
                    }

                    if (coaType.ToLower().Trim() != record.COA.ToLower().Trim() ||
                        dataDescription.ToLower().Trim() != record.DATA_DESCRIPTION.ToLower().Trim())
                    {
                        (worksheet.Rows[rowCount + 1] as Range).Insert(XlInsertShiftDirection.xlShiftDown, System.Type.Missing);
                        (worksheet.Rows[rowCount] as Range).Copy(worksheet.Rows[rowCount + 1]);
                        (worksheet.Rows[rowCount] as Range).Clear();

                        (worksheet.Cells[rowCount, 1] as Range).Value = record.COA.Trim();
                        (worksheet.Cells[rowCount, 2] as Range).Value = record.DATA_DESCRIPTION.Trim();

                        Globals.ThisAddIn.AppendMessage(ThisAddIn.MODEL_UPLOAD_WORKSHEET_NAME 
                            + ": New COA/Description inserted at row " + rowCount, true);
                    }

                    rowCount++;
                }
                Range deleteRange = worksheet.Range[worksheet.Cells[rowCount, 1]
                    , worksheet.Cells.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing)];
                if (deleteRange != null)
                {
                    deleteRange.Clear();
                }

                return worksheet;
            }
            catch (Exception ex)
            {
                Globals.ThisAddIn.AppendMessage(ex.Message + "|" + ex.StackTrace, true);
                return null;
            }
        }
    }
}

