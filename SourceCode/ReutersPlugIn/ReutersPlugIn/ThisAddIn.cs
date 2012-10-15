using System;
using System.ComponentModel;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools;
using ReutersPlugIn.ModelRefreshDefinitions;

namespace ReutersPlugIn
{
    /// <summary>
    /// Reuters Plugin startup class
    /// </summary>
    public partial class ThisAddIn
    {
        #region Fields
        /// <summary>
        /// Background worker for fetching data from service asynchronously
        /// </summary>
        private BackgroundWorker dataFetchWorker;

        /// <summary>
        /// Constant worksheet names
        /// </summary>
        internal const string REUTERS_REPORTED_WORKSHEET_NAME = "Reuters Reported";
        internal const string CONSENSUS_DATA_WORKSHEET_NAME = "Consensus Data";
        internal const string MODEL_UPLOAD_WORKSHEET_NAME = "Model Upload";
        internal const string MODEL_REFERENCE_WORKSHEET_NAME = "Model Reference"; 
        #endregion

        #region Properties
        /// <summary>
        /// Custom reuters task pane
        /// </summary>
        private CustomTaskPane _taskPane;
        internal CustomTaskPane TaskPane
        {
            get { return _taskPane; }
        }
        #endregion

        #region Internal Task Pane Methods
        /// <summary>
        /// Modifies reuters task pane enabled status
        /// </summary>
        /// <param name="value">True to enable task pane</param>
        internal void ModifyTaskPaneEnabled(Boolean value)
        {
            var taskPaneView = TaskPane.Control as TaskPaneView;
            if (taskPaneView != null)
            {
                taskPaneView.IsTaskPaneEnabled(value);
            }
        }
        
        /// <summary>
        /// Appends message to task pane
        /// </summary>
        /// <param name="message">message to be appended</param>
        internal void AppendMessage(String message)
        {
            var taskPaneView = TaskPane.Control as TaskPaneView;
            if (taskPaneView != null)
            {
                taskPaneView.AppendMessageToRtb(message);
            }
        }

        /// <summary>
        /// Appends message to task pane
        /// </summary>
        /// <param name="message">message to be appended</param>
        /// <param name="newLineCount">additional lines to be posted before message</param>
        internal void AppendMessage(String message, Int32 newLineCount)
        {
            var taskPaneView = TaskPane.Control as TaskPaneView;
            if (taskPaneView != null)
            {
                taskPaneView.AppendMessageToRtb(message, newLineCount: newLineCount);
            }
        }

        /// <summary>
        /// Appends message to task pane
        /// </summary>
        /// <param name="message">message to be appended</param>
        /// <param name="isMessageAlert">True to highlight message as alert</param>
        internal void AppendMessage(String message, Boolean isMessageAlert)
        {
            var taskPaneView = TaskPane.Control as TaskPaneView;
            if (taskPaneView != null)
            {
                taskPaneView.AppendMessageToRtb(message, isMessageAlert: isMessageAlert);
            }
        }

        /// <summary>
        /// Appends message to task pane
        /// </summary>
        /// <param name="message">message to be appended</param>
        /// <param name="isMessageAlert">True to highlight message as alert</param>
        /// /// <param name="newLineCount">additional lines to be posted before message</param>
        internal void AppendMessage(String message, Boolean isMessageAlert, Int32 newLineCount)
        {
            var taskPaneView = TaskPane.Control as TaskPaneView;
            if (taskPaneView != null)
            {
                taskPaneView.AppendMessageToRtb(message, isMessageAlert: isMessageAlert, newLineCount: newLineCount);
            }
        }
        #endregion        

        #region Event Handlers
        /// <summary>
        /// ThisAddIn Startup EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            var taskPaneView = new TaskPaneView();
            this._taskPane = this.CustomTaskPanes.Add(taskPaneView, "Reuters");
            this._taskPane.Visible = false;

            AppendMessage("Application Startup", 0);
            dataFetchWorker = new BackgroundWorker();
            dataFetchWorker.DoWork += new DoWorkEventHandler(dataFetchWorker_DoWork);
            dataFetchWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(dataFetchWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// ThisAddIn Shutdown EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            dataFetchWorker.DoWork -= new DoWorkEventHandler(dataFetchWorker_DoWork);
            dataFetchWorker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(dataFetchWorker_RunWorkerCompleted);
        }

        /// <summary>
        /// dataFetchWorker DoWork EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataFetchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                InputData inputData = e.Argument as InputData;
                if (inputData == null)
                {
                    throw new Exception("Unable to parse async argument to input data type...");
                }

                ExcelModelRefreshOperationsClient client = new ExcelModelRefreshOperationsClient();
                ExcelModelData outputData = client.RetrieveStatementData(inputData.IssuerID);

                if (outputData == null)
                {
                    throw new Exception("Unable to retrieve data from service...");
                }

                e.Result = outputData;
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message, true);
                return;
            }
        }

        /// <summary>
        /// dataFetchWorker RunWorkerCompleted EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dataFetchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    throw new Exception(e.Error.Message);
                }

                ExcelModelData outputData = e.Result as ExcelModelData;
                if (outputData == null)
                {
                    throw new Exception("Unable to parse async result to output data type...");
                }

                Globals.ThisAddIn.AppendMessage("Model data retrieval complete...");
                Globals.ThisAddIn.AppendMessage("Updating workbook data...");
                Boolean result = Model.Update(outputData);
                if (result == false)
                {
                    throw new Exception("");
                }
                Globals.ThisAddIn.AppendMessage("Refresh process complete");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message, true);
                AppendMessage("Import Failed!");
                return;
            }
            finally
            {
                ModifyTaskPaneEnabled(true);
            }
        }
        #endregion

        #region VSTO generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        #endregion

        #region Functionality Methods
        /// <summary>
        /// Validates workbook
        /// </summary>
        /// <returns></returns>
        private Boolean ValidateWorkBook()
        {
            try
            {
                AppendMessage("Validating workbook...");

                var excelApp = this.Application;
                Int32 workSheetCount = excelApp.Worksheets.Count;

                if (workSheetCount != 4)
                {
                    throw new Exception(String.Format("Worksheet count mismatch. There should be four worksheets :{0}, {1}, {2} and {3}",
                        REUTERS_REPORTED_WORKSHEET_NAME, CONSENSUS_DATA_WORKSHEET_NAME, MODEL_UPLOAD_WORKSHEET_NAME, MODEL_REFERENCE_WORKSHEET_NAME));
                }

                Boolean isReutersReportedSheetPresent = false;
                Boolean isConsensusDataSheetPresent = false;
                Boolean isModelUploadSheetPresent = false;
                Boolean isModelReferenceSheetPresent = false;

                for (int i = 1; i <= workSheetCount; i++)
                {
                    switch (((Excel.Worksheet)excelApp.Worksheets[i]).Name)
                    {
                        case REUTERS_REPORTED_WORKSHEET_NAME:
                            isReutersReportedSheetPresent = true;
                            break;
                        case CONSENSUS_DATA_WORKSHEET_NAME:
                            isConsensusDataSheetPresent = true;
                            break;
                        case MODEL_UPLOAD_WORKSHEET_NAME:
                            isModelUploadSheetPresent = true;
                            break;
                        case MODEL_REFERENCE_WORKSHEET_NAME:
                            isModelReferenceSheetPresent = true;
                            break;
                    }
                }

                if (!(isReutersReportedSheetPresent && isConsensusDataSheetPresent
                    && isModelUploadSheetPresent && isModelReferenceSheetPresent))
                {
                    throw new Exception(String.Format("One or more worksheets missing. There should be four worksheets :{0}, {1}, {2} and {3}",
                        REUTERS_REPORTED_WORKSHEET_NAME, CONSENSUS_DATA_WORKSHEET_NAME, MODEL_UPLOAD_WORKSHEET_NAME, MODEL_REFERENCE_WORKSHEET_NAME));
                }

                //Model reference sheet content validation
                Excel.Worksheet modelReferenceWorkSheet = excelApp.Worksheets[MODEL_REFERENCE_WORKSHEET_NAME];

                if ((modelReferenceWorkSheet.Cells[1, 1] as Excel.Range).Value != "Issuer ID")
                {
                    throw new Exception(MODEL_REFERENCE_WORKSHEET_NAME + ":'Issuer ID' notation cell is missing at [1,1]");
                }

                if ((modelReferenceWorkSheet.Cells[2, 1] as Excel.Range).Value != "Issuer Name")
                {
                    throw new Exception(MODEL_REFERENCE_WORKSHEET_NAME + ":'Issuer Name' notation cell is missing at [2,1]");
                }

                if ((modelReferenceWorkSheet.Cells[3, 1] as Excel.Range).Value != "COA Type")
                {
                    throw new Exception(MODEL_REFERENCE_WORKSHEET_NAME + ":'COA Type' notation cell is missing at [3,1]");
                }

                //Model upload sheet content validation
                Excel.Worksheet modelUploadWorkSheet = excelApp.Worksheets[MODEL_UPLOAD_WORKSHEET_NAME];

                for (int j = 1; j <= 2; j++)
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        switch (j)
                        {
                            case 1:
                                {
                                    if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value) != String.Empty)
                                    {
                                        throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                            + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    switch (i)
                                    {
                                        case 1:
                                            if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value)
                                                != String.Empty)
                                            {
                                                throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                                    + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                            }
                                            break;
                                        case 2:
                                            if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value)
                                                != "Period EndDate")
                                            {
                                                throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                                    + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                            }
                                            break;
                                        case 3:
                                            if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value)
                                                != "Period Length (In Months)")
                                            {
                                                throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                                    + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                            }
                                            break;
                                        case 4:
                                            if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value)
                                                != "Actual(Reported) Override")
                                            {
                                                throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                                    + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                            }
                                            break;
                                        case 5:
                                            if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value)
                                                != "Commodity Measure")
                                            {
                                                throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                                    + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                            }
                                            break;
                                        case 6:
                                            if (Convert.ToString((modelUploadWorkSheet.Cells[i, j] as Excel.Range).Value)
                                                != "Commodity Forecast Used")
                                            {
                                                throw new Exception(MODEL_UPLOAD_WORKSHEET_NAME
                                                    + ":Worksheet format do not match export model sheet at [" + i + "," + j + "]");
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                                }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message, true);
                return false;
            }

            return true;
        }

        private InputData RetrieveInputData()
        {
            try
            {
                AppendMessage("Retrieving model reference data...");
                InputData result = new InputData();
                var excelApp = this.Application;

                result.IssuerID = Convert.ToString((((Excel.Worksheet)excelApp.Worksheets[MODEL_REFERENCE_WORKSHEET_NAME]).Cells[1, 2] as Excel.Range).Value);
                result.IssueName = Convert.ToString((((Excel.Worksheet)excelApp.Worksheets[MODEL_REFERENCE_WORKSHEET_NAME]).Cells[2, 2] as Excel.Range).Value);
                result.COAType = Convert.ToString((((Excel.Worksheet)excelApp.Worksheets[MODEL_REFERENCE_WORKSHEET_NAME]).Cells[3, 2] as Excel.Range).Value);

                return result;
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message, true);
                return null;
            }
        }

        internal void GetData()
        {
            try
            {
                ModifyTaskPaneEnabled(false);

                AppendMessage(String.Format("-----------------------------------------", DateTime.Now));
                AppendMessage(String.Format("Refresh request [{0:MM/dd/yyyy HH:mm:ss}]", DateTime.Now));

                Boolean isWorkBookValid = ValidateWorkBook();

                if (!isWorkBookValid)
                {
                    throw new Exception("Validation Failed...");
                }
                InputData inputData = RetrieveInputData();
                if (inputData == null)
                {
                    throw new Exception("Input Failed...");
                }
                AppendMessage("Issuer Id: " + inputData.IssuerID);
                AppendMessage("Retrieving model data...");
                dataFetchWorker.RunWorkerAsync(inputData);
                return;
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message, true);
                AppendMessage("Import Failed!");
                ModifyTaskPaneEnabled(true);
                return;
            }
        }         
        #endregion
    }
}
