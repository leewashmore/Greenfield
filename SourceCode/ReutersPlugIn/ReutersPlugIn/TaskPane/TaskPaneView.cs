using System;
using System.Drawing;
using System.Windows.Forms;

namespace ReutersPlugIn
{
    /// <summary>
    /// Custom Reuters Task Pane
    /// </summary>
    public partial class TaskPaneView : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TaskPaneView()
        {
            InitializeComponent();
            this.rtbLogs.ReadOnly = true;
        }

        /// <summary>
        /// AppendMessageToRtb Callback delegate
        /// </summary>
        /// <param name="message">message to be appended</param>
        /// <param name="newLineCount">additional lines to be posted before message</param>
        /// <param name="isMessageAlert">True to highlight message as alert</param>
        delegate void AppendMessageToRtbCallback(String message, Int32 newLineCount = 1, Boolean isMessageAlert = false);

        /// <summary>
        /// IsTaskPaneEnabled Callback delegate
        /// </summary>
        /// <param name="value"></param>
        delegate void IsTaskPaneEnabledCallback(Boolean value);

        /// <summary>
        /// Appends message to task pane
        /// </summary>
        /// <param name="message">message to be appended</param>
        /// <param name="newLineCount">additional lines to be posted before message</param>
        /// <param name="isMessageAlert">True to highlight message as alert</param>
        public void AppendMessageToRtb(String message, Int32 newLineCount = 1, Boolean isMessageAlert = false)
        {
            if (this.rtbLogs.InvokeRequired)
            {
                AppendMessageToRtbCallback callback = new AppendMessageToRtbCallback(AppendMessageToRtb);
                this.Invoke(callback, new object[] { message, newLineCount, isMessageAlert });
            }
            else
            {
                if (String.IsNullOrWhiteSpace(message))
                {
                    return;
                }
                int indexStart = this.rtbLogs.TextLength;
                for (int i = 0; i < newLineCount; i++)
                {
                    this.rtbLogs.AppendText("\n");
                }
                this.rtbLogs.AppendText(message);
                if (isMessageAlert)
                {
                    int indexEnd = this.rtbLogs.TextLength;
                    this.rtbLogs.Select(indexStart, indexEnd - indexStart);
                    this.rtbLogs.SelectionColor = Color.Red;
                    this.rtbLogs.SelectionFont = new Font("Arial", 7, FontStyle.Italic);
                    this.rtbLogs.SelectionLength = 0;
                }
            }
        }        

        /// <summary>
        /// Modifies reuters task pane enabled status
        /// </summary>
        /// <param name="value">True to enable task pane</param>
        public void IsTaskPaneEnabled(Boolean value)
        {
            if (this.rtbLogs.InvokeRequired)
            {
                IsTaskPaneEnabledCallback callback = new IsTaskPaneEnabledCallback(IsTaskPaneEnabled);
                this.Invoke(callback, new object[] { value });
            }
            else
            {
                if (value)
                {
                    this.rtbLogs.Enabled = value;
                }
            }
        }

        /// <summary>
        /// clearScrenToolStripMenuItem Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearScrenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.rtbLogs.Clear();
        }
    }
}
