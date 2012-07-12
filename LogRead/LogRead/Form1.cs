using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using ADODB;

namespace LogRead
{
    public partial class Form1 : Form
    {
        private DateTime _SelectedDateTime = DateTime.Today;
        private String _SelectedDirectory = String.Empty;

        public Form1()
        {
            InitializeComponent();

            this.dgLogDetails.Columns[0].ValueType = typeof(Boolean);
            this.dgLogDetails.Columns[1].ValueType = typeof(String);
            this.dgLogDetails.Columns[2].ValueType = typeof(String);
            this.dgLogDetails.Columns[3].ValueType = typeof(String);
            this.dgLogDetails.Columns[4].ValueType = typeof(Boolean);
            this.dgLogDetails.Columns[5].ValueType = typeof(String);
            this.dgLogDetails.Columns[6].ValueType = typeof(String);
            this.dgLogDetails.Columns[7].ValueType = typeof(String);
            this.dgLogDetails.Columns[8].ValueType = typeof(String);
            this.dgLogDetails.Columns[9].ValueType = typeof(String);
            this.dgLogDetails.Columns[10].ValueType = typeof(String);
            this.dgLogDetails.Columns[11].ValueType = typeof(String);
            this.dgLogDetails.Columns[12].ValueType = typeof(String);
            this.dgLogDetails.Columns[13].ValueType = typeof(String);

            this.dgLogDetails.DataError += (se, e) =>
                {
                };

            this.Load += (se, e) =>
                {
                    this.dgcSelect.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    this.dgcSelect.AutoSizeMode = DataGridViewAutoSizeColumnMode.NotSet;
                    this.dgcSelect.Width = 20;

                    Rectangle rect = this.dgLogDetails.GetCellDisplayRectangle(0, -1, true);
                    CheckBox chkbox = new CheckBox() { Size = new Size(18, 18)
                        , Location = new Point(rect.Location.X + 3, rect.Location.Y + 8), Checked = true};
                    this.dgLogDetails.Controls.Add(chkbox);
                    
                    chkbox.CheckedChanged += (_se, _e) =>
                        {
                            foreach (DataGridViewRow row in this.dgLogDetails.Rows)
                            {
                                row.Cells[0].Value = chkbox.Checked;
                            }

                            this.dgLogDetails.EndEdit();
                        };
                };
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                Description = "Select folder where log files are placed",
                RootFolder = Environment.SpecialFolder.MyComputer
            };

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tbxLogFolderLoc.Text = dialog.SelectedPath;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (this.tbxLogFolderLoc.Text.Equals(null) || this.tbxLogFolderLoc.Text.Equals(String.Empty))
            {
                MessageBox.Show("Root Folder not selected");
                return;
            }

            if (Directory.Exists(this.tbxLogFolderLoc.Text).Equals(false))
            {
                MessageBox.Show("Directory does not exist");
               this.tbxLogFolderLoc.Text =  _SelectedDirectory;
               this.dtpLogDate.Value = _SelectedDateTime;
                return;
            }
            
            string searchPattern = this.dtpLogDate.Checked 
                ? "logfile-" + this.dtpLogDate.Value.ToString("dd.MM.yyyy") + ".log*"
                : "*.log*";

            List<string> logFiles = Directory.GetFiles(this.tbxLogFolderLoc.Text, searchPattern).ToList();

            if (logFiles.Count.Equals(0))
            {
                MessageBox.Show("No Log files exist");
                this.tbxLogFolderLoc.Text = _SelectedDirectory;
                this.dtpLogDate.Value = _SelectedDateTime;
                return;
            }

            _SelectedDateTime = this.dtpLogDate.Value;
            _SelectedDirectory = this.tbxLogFolderLoc.Text;

            List<LogDetails> logdetails = new List<LogDetails>();

            if (Directory.Exists(@"C:\Log_Temp"))
                Directory.Delete(@"C:\Log_Temp", true);

            Directory.CreateDirectory(@"C:\Log_Temp");

            foreach (String logFile in logFiles)
            {
                string cpyLogFile = @"C:\Log_Temp" + logFile.Replace(this.tbxLogFolderLoc.Text, "");

                if (!Directory.Exists(@"C:\Log_Temp"))
                    Directory.CreateDirectory(@"C:\Log_Temp");

                File.Copy(logFile, cpyLogFile);

                Encoding encoding = GetFileEncoding(cpyLogFile);
                using (StreamReader logFileReader = new StreamReader(cpyLogFile, encoding))
                {
                    #region Log File Read
                    do
                    {
                        String textLine = logFileReader.ReadLine();

                        String subString = String.Empty;
                        LogDetails logDetail = new LogDetails();

                        logDetail.LogSelection = true;

                        logDetail.LogDateTime = new DateTime(Convert.ToInt32(textLine.Substring(0, 4))
                            , Convert.ToInt32(textLine.Substring(5, 2)), Convert.ToInt32(textLine.Substring(8, 2))
                            , Convert.ToInt32(textLine.Substring(11, 2)), Convert.ToInt32(textLine.Substring(14, 2))
                            , Convert.ToInt32(textLine.Substring(17, 2)), Convert.ToInt32(textLine.Substring(20, 3))).ToString("dd/MM/yyyy HH:mm:ss:fff");

                        subString = textLine.Substring(24, textLine.Length - 24);
                        int breakerIndex = subString.IndexOf("?");

                        logDetail.LogCategory = textLine.Substring(24, (subString.IndexOf("?") == -1 ? subString.IndexOf(" ") : subString.IndexOf("?") - 1)).Trim();

                        logDetail.LogUserIsLogged = subString.IndexOf("|User[(") > 0;


                        string logUserSubString = logDetail.LogUserIsLogged
                            ? subString.Substring(subString.IndexOf("|User[("), subString.Length - subString.IndexOf("|User[("))
                            : subString.Substring(subString.IndexOf("|LoginID[("), subString.Length - subString.IndexOf("|LoginID[("));

                        if (logUserSubString.IndexOf(")]").Equals(-1))
                            logDetail.LogUserName = logUserSubString.Substring(logUserSubString.IndexOf("[(") + 2, logUserSubString.Length - logUserSubString.IndexOf("[(") - 1);
                        else
                            logDetail.LogUserName = logUserSubString.Substring(logUserSubString.IndexOf("[(") + 2, logUserSubString.IndexOf(")]") - logUserSubString.IndexOf("[(") - 2);


                        if (subString.IndexOf("|Type[(").Equals(-1))
                        {
                            logDetail.LogType = null;
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|Type[("), subString.Length - subString.IndexOf("|Type[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogType = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogType = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }


                        if (subString.IndexOf("|Message[(").Equals(-1))
                        {
                            logDetail.LogExceptionMessage = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|Message[("), subString.Length - subString.IndexOf("|Message[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogExceptionMessage = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogExceptionMessage = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|StackTrace[(").Equals(-1))
                        {
                            logDetail.LogExceptionStackTrace = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|StackTrace[("), subString.Length - subString.IndexOf("|StackTrace[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogExceptionStackTrace = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogExceptionStackTrace = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|MethodNameSpace[(").Equals(-1))
                        {
                            logDetail.LogMethodNameSpace = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|MethodNameSpace[("), subString.Length - subString.IndexOf("|MethodNameSpace[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogMethodNameSpace = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogMethodNameSpace = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|ArgumentIndex[(").Equals(-1))
                        {
                            logDetail.LogArgumentIndex = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|ArgumentIndex[("), subString.Length - subString.IndexOf("|ArgumentIndex[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogArgumentIndex = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogArgumentIndex = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|ArgumentValue[(").Equals(-1))
                        {
                            logDetail.LogArgumentValue = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|ArgumentValue[("), subString.Length - subString.IndexOf("|ArgumentValue[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogArgumentValue = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogArgumentValue = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|Account[(").Equals(-1))
                        {
                            logDetail.LogAccountDetail = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|Account[("), subString.Length - subString.IndexOf("|Account[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogAccountDetail = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogAccountDetail = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|Role[(").Equals(-1))
                        {
                            logDetail.LogRoleDetail = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|Role[("), subString.Length - subString.IndexOf("|Role[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogRoleDetail = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogRoleDetail = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        if (subString.IndexOf("|TimeStamp[(").Equals(-1))
                        {
                            logDetail.LogTimeStamp = "N/A";
                        }
                        else
                        {
                            string logSubString = subString.Substring(subString.IndexOf("|TimeStamp[("), subString.Length - subString.IndexOf("|TimeStamp[("));
                            if (logSubString.IndexOf(")]").Equals(-1))
                                logDetail.LogTimeStamp = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.Length - logSubString.IndexOf("[(") - 1);
                            else
                                logDetail.LogTimeStamp = logSubString.Substring(logSubString.IndexOf("[(") + 2, logSubString.IndexOf(")]") - logSubString.IndexOf("[(") - 2);
                        }

                        logdetails.Add(logDetail);


                    } while (logFileReader.Peek() != -1);
                    #endregion                     
                }                
                
                File.Delete(cpyLogFile);
            }

            if (Directory.Exists(@"C:\Log_Temp"))
                Directory.Delete(@"C:\Log_Temp", true);

            this.dgcDate.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcCategory.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcLoggedIn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcUser.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcType.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcArgumentIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcAccountDetail.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcRoleDetail.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            BindingSource bindingSource = new BindingSource();
            //bindingSource.SupportsFiltering = true;
            this.dgLogDetails.DataSource = bindingSource;
            bindingSource.DataSource = logdetails.OrderByDescending(record => record.LogTimeStamp).ToList().ToDataTable().AsDataView();
            //foreach (var item in logdetails.OrderByDescending(record => record.LogDateTime).ToList().ToDataTable().AsDataView())
            //{
            //    bindingSource.Add(item);
            //}

            //this.dgLogDetails.DataSource = bindingSource;

        }

        private Encoding GetFileEncoding(String filePath)
        {
            Encoding enc = null;
            
            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (file.CanSeek)
                {
                    byte[] bom = new byte[4]; // Get the byte-order mark, if there is one  
                    file.Read(bom, 0, 4);
                    if ((bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) || // utf-8  
                        (bom[0] == 0xff && bom[1] == 0xfe) || // ucs-2le, ucs-4le, and ucs-16le  
                        (bom[0] == 0xfe && bom[1] == 0xff) || // utf-16 and ucs-2  
                        (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)) // ucs-4  
                    {
                        enc = System.Text.Encoding.Unicode;
                    }
                    else
                    {
                        enc = System.Text.Encoding.ASCII;
                    }

                    // Now reposition the file cursor back to the start of the file  
                    file.Seek(0, System.IO.SeekOrigin.Begin);
                }
                else
                {
                    // The file cannot be randomly accessed, so you need to decide what to set the default to  
                    // based on the data provided. If you're expecting data from a lot of older applications,  
                    // default your encoding to Encoding.ASCII. If you're expecting data from a lot of newer  
                    // applications, default your encoding to Encoding.Unicode. Also, since binary files are  
                    // single byte-based, so you will want to use Encoding.ASCII, even though you'll probably  
                    // never need to use the encoding then since the Encoding classes are really meant to get  
                    // strings from the byte array that is the file.  

                    enc = System.Text.Encoding.ASCII;
                } 
            }

            
            return enc;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportReport("Log Report " + DateTime.Now.ToString().Replace(":", "."), this.dgLogDetails);
        }

        private void ExportReport(string ReportName, DataGridView dataGridView)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlBook = xlApp.Workbooks.Add();
            Excel.Worksheet xlSheet = xlBook.Worksheets.Add();

            xlSheet.Name = ReportName;
            int xlRow = 1;
            Recordset recordSet = new Recordset();

            try
            {
                try
                {
                    xlSheet.Columns.HorizontalAlignment = 2;
                    xlSheet.Columns.Font.Name = "Times New Roman";
                    xlSheet.Rows.Item[xlRow].Font.Bold = 1;
                    xlSheet.Rows.Item[xlRow].Interior.ColorIndex = 15;
                    recordSet = GetRecordSet(dataGridView);
                    if (recordSet.State == 0) recordSet.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message : " + ex.Message + "\nStacktrace : " + ex.StackTrace);
                    goto PROC_EXIT;
                }

                for (int xlColumn = 1; xlColumn < recordSet.Fields.Count; xlColumn++)
                {
                    xlSheet.Cells.Item[xlRow, xlColumn] = recordSet.Fields[xlColumn].Name;
                    Excel.Range range = xlSheet.Cells[xlRow, xlColumn] as Excel.Range;
                    range.BorderAround2(1, Excel.XlBorderWeight.xlHairline, Excel.XlColorIndex.xlColorIndexAutomatic);
                }

                for (int xlColumn = 1; xlColumn < recordSet.Fields.Count; xlColumn++)
                    xlSheet.Columns[xlColumn].NumberFormat = TranslateNumberFormat(recordSet.Fields[xlColumn].Type);

                for (int recordCount = 0; recordCount < recordSet.RecordCount; recordCount++)
                {
                    xlRow++;
                    if (recordSet.Fields[0].Value == true)
                    {
                        for (int fieldCount = 1; fieldCount < recordSet.Fields.Count; fieldCount++)
                            xlSheet.Cells.Item[xlRow, fieldCount] = recordSet.Fields[fieldCount].Value;
                    }
                    recordSet.MoveNext();                    
                }

                xlSheet.PageSetup.LeftHeader = "&[Page]" + " of " + "&[Pages]";
                xlSheet.PageSetup.RightHeader = "&[Date]" + " &[Time]";
                xlSheet.PageSetup.HeaderMargin = 5;
                xlSheet.PageSetup.BottomMargin = 5;
                xlSheet.PageSetup.LeftMargin = 5;
                xlSheet.PageSetup.RightMargin = 5;
                xlSheet.PageSetup.TopMargin = 2;
                xlSheet.Columns.AutoFit();
                xlSheet.Rows.AutoFit();
                xlApp.UserControl = true;
                xlApp.Visible = true;
            }
            catch (Exception ex) { MessageBox.Show("Message : " + ex.Message + "\nStacktrace : " + ex.StackTrace); }

        PROC_EXIT:

            xlSheet = null;
            xlBook = null;
            xlApp = null;
            GC.Collect();
        }

        public static DataTypeEnum TranslateType(Type columnType)
        {
            if (columnType == null)
            {
                return DataTypeEnum.adVarChar;
            }
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Boolean":
                    return DataTypeEnum.adBoolean;
                case "System.DateTime":
                    return DataTypeEnum.adDate;
                case "System.Double":
                    return DataTypeEnum.adDouble;
                case "System.Int32":
                    return DataTypeEnum.adInteger;
                case "System.String":
                default:
                    return DataTypeEnum.adVarChar;
            }
        }

        public static Type TranslateDataTypeEnum(DataTypeEnum columnType)
        {
            switch (columnType)
            {
                case DataTypeEnum.adDate:
                    return typeof(DateTime);
                case DataTypeEnum.adDouble:
                    return typeof(Double);
                case DataTypeEnum.adInteger:
                    return typeof(Int32);
                case DataTypeEnum.adVarChar:
                default:
                    return typeof(String);
            }
        }

        public static string TranslateNumberFormat(DataTypeEnum dataType)
        {
            switch (dataType)
            {
                case DataTypeEnum.adDate:
                    return "m/d/yyyy h:mm AM/PM";
                case DataTypeEnum.adDouble:
                    return "0.00_ ;[Red]-0.00";
                case DataTypeEnum.adInteger:
                    return "0;[Red]0";
                case DataTypeEnum.adVarChar:
                default:
                    return "@";
            }
        }

        public static Recordset GetRecordSet(DataGridView dataGridView)
        {
            //string mthdNamespace = "ReportsLibrary.DGVRecordSet.GetRecordSet";
            if (dataGridView == null)
            {
                //Logging.LogEvent("ArgumentNull", mthdNamespace, new List<Logging.Arguments> { new Logging.Arguments() { ArgumentName = "dataGridView", ArgumentValue = "Null" } });
                return null;
            }

            try
            {
                Recordset result = new Recordset();
                result.CursorLocation = CursorLocationEnum.adUseClient;
                Fields resultFields = result.Fields;
                DataGridViewColumnCollection iColumns = dataGridView.Columns;



                foreach (DataGridViewColumn iColumn in iColumns)
                    if (iColumn.Visible == true)
                    {
                        DataTypeEnum fieldType = TranslateType(iColumn.ValueType);
                        if (fieldType == DataTypeEnum.adVarChar) resultFields.Append(iColumn.HeaderText, fieldType, 300);
                        else
                        {
                            iColumn.HeaderText = iColumn.HeaderText == "" ? "{Empty}" : iColumn.HeaderText;
                            resultFields.Append(iColumn.HeaderText, fieldType);
                        }
                    }

                result.Open();

                foreach (DataGridViewRow iRow in dataGridView.Rows)
                {
                    result.AddNew();
                    foreach (DataGridViewColumn iColumn in dataGridView.Columns)
                        if (iColumn.Visible == true)
                        {
                            if (iRow.Cells[iColumn.Index].Value == null)
                            {
                                if (iColumn.ValueType == typeof(String))
                                {
                                    iRow.Cells[iColumn.Index].Value = "";
                                }

                                if (iColumn.ValueType == typeof(Boolean))
                                {
                                    iRow.Cells[iColumn.Index].Value = false;
                                }
                            }

                            if ( iRow.Cells[iColumn.Index].Value.GetType() == typeof(DBNull))
                            {
                                if (iColumn.ValueType == typeof(String))
                                {
                                    iRow.Cells[iColumn.Index].Value = "";
                                }

                                if (iColumn.ValueType == typeof(Boolean))
                                {
                                    iRow.Cells[iColumn.Index].Value = false;
                                }
                            }

                            
                            resultFields[iColumn.HeaderText].Value = iRow.Cells[iColumn.Index].Value;
                        }
                }

                if (!result.BOF) result.MoveFirst();
                return result;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception");
                //Logging.LogException(ex);
                return null;
            }
        }
    }

    public static class ConvertTo
    {
        public static DataTable ToDataTable<T>(this IList<T> data) 
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T)); 
            DataTable table = new DataTable(); 
            foreach (PropertyDescriptor prop in properties)        
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data) 
            { 
                DataRow row = table.NewRow(); 
                foreach (PropertyDescriptor prop in properties)   
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value; table.Rows.Add(row);
            }
            return table; 
        } 
    }
}
