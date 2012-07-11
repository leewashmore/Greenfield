namespace LogRead
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblLogFolderLoc = new System.Windows.Forms.Label();
            this.dtpLogDate = new System.Windows.Forms.DateTimePicker();
            this.tbxLogFolderLoc = new System.Windows.Forms.TextBox();
            this.lblLogDate = new System.Windows.Forms.Label();
            this.dgLogDetails = new System.Windows.Forms.DataGridView();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.dgcSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgcDate = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcTimeStamp = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcCategory = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcLoggedIn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgcUser = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcType = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcExceptionMessage = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcStackTrace = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcMethodNamespace = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcArgumentIndex = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcArgumentValue = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcAccountDetail = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            this.dgcRoleDetail = new DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgLogDetails)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLogFolderLoc
            // 
            this.lblLogFolderLoc.AutoSize = true;
            this.lblLogFolderLoc.Location = new System.Drawing.Point(3, 5);
            this.lblLogFolderLoc.Name = "lblLogFolderLoc";
            this.lblLogFolderLoc.Size = new System.Drawing.Size(80, 13);
            this.lblLogFolderLoc.TabIndex = 0;
            this.lblLogFolderLoc.Text = "Folder Location";
            // 
            // dtpLogDate
            // 
            this.dtpLogDate.Checked = false;
            this.dtpLogDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpLogDate.Location = new System.Drawing.Point(455, 3);
            this.dtpLogDate.Name = "dtpLogDate";
            this.dtpLogDate.ShowCheckBox = true;
            this.dtpLogDate.Size = new System.Drawing.Size(200, 20);
            this.dtpLogDate.TabIndex = 2;
            // 
            // tbxLogFolderLoc
            // 
            this.tbxLogFolderLoc.Location = new System.Drawing.Point(90, 3);
            this.tbxLogFolderLoc.Name = "tbxLogFolderLoc";
            this.tbxLogFolderLoc.Size = new System.Drawing.Size(297, 20);
            this.tbxLogFolderLoc.TabIndex = 3;
            // 
            // lblLogDate
            // 
            this.lblLogDate.AutoSize = true;
            this.lblLogDate.Location = new System.Drawing.Point(419, 5);
            this.lblLogDate.Name = "lblLogDate";
            this.lblLogDate.Size = new System.Drawing.Size(30, 13);
            this.lblLogDate.TabIndex = 4;
            this.lblLogDate.Text = "Date";
            // 
            // dgLogDetails
            // 
            this.dgLogDetails.AllowUserToAddRows = false;
            this.dgLogDetails.AllowUserToDeleteRows = false;
            this.dgLogDetails.AllowUserToResizeRows = false;
            this.dgLogDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgLogDetails.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgLogDetails.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dgLogDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgLogDetails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgcSelect,
            this.dgcDate,
            this.dgcTimeStamp,
            this.dgcCategory,
            this.dgcLoggedIn,
            this.dgcUser,
            this.dgcType,
            this.dgcExceptionMessage,
            this.dgcStackTrace,
            this.dgcMethodNamespace,
            this.dgcArgumentIndex,
            this.dgcArgumentValue,
            this.dgcAccountDetail,
            this.dgcRoleDetail});
            this.dgLogDetails.Location = new System.Drawing.Point(6, 28);
            this.dgLogDetails.Name = "dgLogDetails";
            this.dgLogDetails.Size = new System.Drawing.Size(1007, 631);
            this.dgLogDetails.TabIndex = 5;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(662, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 6;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(393, 3);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 23);
            this.btnBrowse.TabIndex = 7;
            this.btnBrowse.Text = "..";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(743, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 8;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // dgcSelect
            // 
            this.dgcSelect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcSelect.DataPropertyName = "LogSelection";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "False";
            this.dgcSelect.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgcSelect.HeaderText = "";
            this.dgcSelect.Name = "dgcSelect";
            this.dgcSelect.Width = 5;
            // 
            // dgcDate
            // 
            this.dgcDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcDate.DataPropertyName = "LogDateTime";
            dataGridViewCellStyle2.NullValue = "N/A";
            this.dgcDate.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgcDate.HeaderText = "Date";
            this.dgcDate.Name = "dgcDate";
            this.dgcDate.ReadOnly = true;
            this.dgcDate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcDate.Width = 76;
            // 
            // dgcTimeStamp
            // 
            this.dgcTimeStamp.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcTimeStamp.DataPropertyName = "LogTimeStamp";
            this.dgcTimeStamp.HeaderText = "Time Stamp";
            this.dgcTimeStamp.Name = "dgcTimeStamp";
            this.dgcTimeStamp.ReadOnly = true;
            this.dgcTimeStamp.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcTimeStamp.Width = 102;
            // 
            // dgcCategory
            // 
            this.dgcCategory.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcCategory.DataPropertyName = "LogCategory";
            dataGridViewCellStyle3.NullValue = "N/A";
            this.dgcCategory.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgcCategory.HeaderText = "Category";
            this.dgcCategory.Name = "dgcCategory";
            this.dgcCategory.ReadOnly = true;
            this.dgcCategory.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcCategory.Width = 95;
            // 
            // dgcLoggedIn
            // 
            this.dgcLoggedIn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcLoggedIn.DataPropertyName = "LogUserIsLogged";
            this.dgcLoggedIn.HeaderText = "Logged In";
            this.dgcLoggedIn.Name = "dgcLoggedIn";
            this.dgcLoggedIn.ReadOnly = true;
            this.dgcLoggedIn.Width = 55;
            // 
            // dgcUser
            // 
            this.dgcUser.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcUser.DataPropertyName = "LogUserName";
            dataGridViewCellStyle4.NullValue = "N/A";
            this.dgcUser.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgcUser.HeaderText = "User";
            this.dgcUser.Name = "dgcUser";
            this.dgcUser.ReadOnly = true;
            this.dgcUser.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcUser.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgcUser.Width = 56;
            // 
            // dgcType
            // 
            this.dgcType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcType.DataPropertyName = "LogType";
            dataGridViewCellStyle5.NullValue = "N/A";
            this.dgcType.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgcType.HeaderText = "Type";
            this.dgcType.Name = "dgcType";
            this.dgcType.ReadOnly = true;
            this.dgcType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcType.Width = 77;
            // 
            // dgcExceptionMessage
            // 
            this.dgcExceptionMessage.DataPropertyName = "LogExceptionMessage";
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.IndianRed;
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.NullValue = "N/A";
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcExceptionMessage.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgcExceptionMessage.HeaderText = "Exception Message";
            this.dgcExceptionMessage.Name = "dgcExceptionMessage";
            this.dgcExceptionMessage.ReadOnly = true;
            this.dgcExceptionMessage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcExceptionMessage.Width = 300;
            // 
            // dgcStackTrace
            // 
            this.dgcStackTrace.DataPropertyName = "LogExceptionStackTrace";
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.IndianRed;
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.NullValue = "N/A";
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcStackTrace.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgcStackTrace.HeaderText = "Stack Trace";
            this.dgcStackTrace.Name = "dgcStackTrace";
            this.dgcStackTrace.ReadOnly = true;
            this.dgcStackTrace.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcStackTrace.Width = 300;
            // 
            // dgcMethodNamespace
            // 
            this.dgcMethodNamespace.DataPropertyName = "LogMethodNameSpace";
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.ForestGreen;
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.NullValue = "N/A";
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcMethodNamespace.DefaultCellStyle = dataGridViewCellStyle8;
            this.dgcMethodNamespace.HeaderText = "Method Namespace";
            this.dgcMethodNamespace.Name = "dgcMethodNamespace";
            this.dgcMethodNamespace.ReadOnly = true;
            this.dgcMethodNamespace.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcMethodNamespace.Width = 300;
            // 
            // dgcArgumentIndex
            // 
            this.dgcArgumentIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgcArgumentIndex.DataPropertyName = "LogArgumentIndex";
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.ForestGreen;
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle9.NullValue = "N/A";
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgcArgumentIndex.DefaultCellStyle = dataGridViewCellStyle9;
            this.dgcArgumentIndex.HeaderText = "Method Argument Index";
            this.dgcArgumentIndex.Name = "dgcArgumentIndex";
            this.dgcArgumentIndex.ReadOnly = true;
            this.dgcArgumentIndex.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcArgumentIndex.Width = 109;
            // 
            // dgcArgumentValue
            // 
            this.dgcArgumentValue.DataPropertyName = "LogArgumentValue";
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.ForestGreen;
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle10.NullValue = "N/A";
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcArgumentValue.DefaultCellStyle = dataGridViewCellStyle10;
            this.dgcArgumentValue.HeaderText = "Method Argument Value";
            this.dgcArgumentValue.Name = "dgcArgumentValue";
            this.dgcArgumentValue.ReadOnly = true;
            this.dgcArgumentValue.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgcArgumentValue.Width = 300;
            // 
            // dgcAccountDetail
            // 
            this.dgcAccountDetail.DataPropertyName = "LogAccountDetail";
            dataGridViewCellStyle11.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle11.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle11.NullValue = "N/A";
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgcAccountDetail.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgcAccountDetail.HeaderText = "Account Detail";
            this.dgcAccountDetail.Name = "dgcAccountDetail";
            this.dgcAccountDetail.ReadOnly = true;
            this.dgcAccountDetail.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // dgcRoleDetail
            // 
            this.dgcRoleDetail.DataPropertyName = "LogRoleDetail";
            dataGridViewCellStyle12.BackColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle12.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle12.NullValue = "N/A";
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.LightBlue;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgcRoleDetail.DefaultCellStyle = dataGridViewCellStyle12;
            this.dgcRoleDetail.HeaderText = "Role Detail";
            this.dgcRoleDetail.Name = "dgcRoleDetail";
            this.dgcRoleDetail.ReadOnly = true;
            this.dgcRoleDetail.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 665);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.dgLogDetails);
            this.Controls.Add(this.lblLogDate);
            this.Controls.Add(this.tbxLogFolderLoc);
            this.Controls.Add(this.dtpLogDate);
            this.Controls.Add(this.lblLogFolderLoc);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Log Read";
            ((System.ComponentModel.ISupportInitialize)(this.dgLogDetails)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblLogFolderLoc;
        private System.Windows.Forms.DateTimePicker dtpLogDate;
        private System.Windows.Forms.TextBox tbxLogFolderLoc;
        private System.Windows.Forms.Label lblLogDate;
        private System.Windows.Forms.DataGridView dgLogDetails;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgcSelect;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcDate;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcTimeStamp;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcCategory;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgcLoggedIn;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcUser;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcType;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcExceptionMessage;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcStackTrace;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcMethodNamespace;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcArgumentIndex;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcArgumentValue;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcAccountDetail;
        private DataGridViewAutoFilter.DataGridViewAutoFilterTextBoxColumn dgcRoleDetail;
    }
}

