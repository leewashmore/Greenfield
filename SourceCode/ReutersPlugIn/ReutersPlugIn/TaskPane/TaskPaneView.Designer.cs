namespace ReutersPlugIn
{
    partial class TaskPaneView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.rtbLogs = new System.Windows.Forms.RichTextBox();
            this.cmsTaskPane = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearScrenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsTaskPane.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbLogs
            // 
            this.rtbLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLogs.BackColor = System.Drawing.SystemColors.Window;
            this.rtbLogs.Enabled = false;
            this.rtbLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLogs.Location = new System.Drawing.Point(0, 0);
            this.rtbLogs.Margin = new System.Windows.Forms.Padding(0);
            this.rtbLogs.Name = "rtbLogs";
            this.rtbLogs.Size = new System.Drawing.Size(150, 150);
            this.rtbLogs.TabIndex = 0;
            this.rtbLogs.Text = "";
            // 
            // cmsTaskPane
            // 
            this.cmsTaskPane.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearScrenToolStripMenuItem});
            this.cmsTaskPane.Name = "cmsTaskPane";
            this.cmsTaskPane.Size = new System.Drawing.Size(153, 48);
            // 
            // clearScrenToolStripMenuItem
            // 
            this.clearScrenToolStripMenuItem.Name = "clearScrenToolStripMenuItem";
            this.clearScrenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.clearScrenToolStripMenuItem.Text = "Clear Screen";
            this.clearScrenToolStripMenuItem.Click += new System.EventHandler(this.clearScrenToolStripMenuItem_Click);
            // 
            // TaskPaneView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbLogs);
            this.Name = "TaskPaneView";
            this.cmsTaskPane.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbLogs;
        private System.Windows.Forms.ContextMenuStrip cmsTaskPane;
        private System.Windows.Forms.ToolStripMenuItem clearScrenToolStripMenuItem;

    }
}
