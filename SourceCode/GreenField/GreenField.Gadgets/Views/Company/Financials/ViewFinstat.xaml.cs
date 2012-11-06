using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// class for view for Finstat
    /// </summary>
    public partial class ViewFinstat : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelFinstat dataContextFinstat;
        public ViewModelFinstat DataContextFinstat
        {
            get { return dataContextFinstat; }
            set { dataContextFinstat = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextFinstat != null)
                { DataContextFinstat.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewFinstat(ViewModelFinstat dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextFinstat = dataContextSource;
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 4, netColumnCount: 7, isQuarterImplemented: false);
            List<string> periodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false);
            PeriodColumns.UpdateColumnInformation(this.dgFinstat, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnNamespace = typeof(ViewModelFinstat).FullName,
                PeriodColumnHeader = periodColumnHeader,
                PeriodIsYearly = true
            }, isQuarterImplemented: false, navigatingColumnStartIndex: 1);
            dgFinstat.Columns[8].Header = "Harmonic Avg " + periodColumnHeader[1] + "-" + periodColumnHeader[3];
            dgFinstat.Columns[9].Header = "Harmonic Avg " + periodColumnHeader[4] + "-" + periodColumnHeader[6];
            SettingGridColumnUniqueNames(periodColumnHeader);

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == this.DataContext.GetType().FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgFinstat, e, false, 1);
                    dgFinstat.Columns[8].Header = "Harmonic Avg " + e.PeriodColumnHeader[1] + "-" + e.PeriodColumnHeader[3];
                    dgFinstat.Columns[9].Header = "Harmonic Avg " + periodColumnHeader[4] + "-" + periodColumnHeader[6];

                    SettingGridColumnUniqueNames(e.PeriodColumnHeader);
                    this.btnExportExcel.IsEnabled = true;
                }
            };           
        } 
        #endregion

        #region Export/Pdf/Print
        #region ExportToExcel
        /// <summary>
        /// handles element exporting when exported to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFinstat_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }

        /// <summary>
        /// catch export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

            RadExportOptionsInfo.Add(new RadExportOptions()
            { 
                ElementName = "Finstat Report",
                Element = this.dgFinstat,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER 
            });
            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Finstat Report");
            childExportOptions.Show(); 
        }
        #endregion

        #region PDFExport
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            PDFExporter.btnExportPDF_Click(this.dgFinstat);
        }
        #endregion

        #region Printing the DataGrid
        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                RichTextBox.Document = PDFExporter.Print(dgFinstat, 6);
            }));
            this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
            RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        }
        #endregion       
        #endregion

        #region Helper Method
        /// <summary>
        /// Method to set unique names for grid columns required in Pdf and Print
        /// </summary>
        /// <param name="periodColumnHeader">List<string> periodColumnHeader</param>
        public void SettingGridColumnUniqueNames(List<string> periodColumnHeader)
        {
            dgFinstat.Columns[0].UniqueName = "";
            dgFinstat.Columns[1].UniqueName = periodColumnHeader[0];
            dgFinstat.Columns[2].UniqueName = periodColumnHeader[1];
            dgFinstat.Columns[3].UniqueName = periodColumnHeader[2];
            dgFinstat.Columns[4].UniqueName = periodColumnHeader[3];
            dgFinstat.Columns[5].UniqueName = periodColumnHeader[4];
            dgFinstat.Columns[6].UniqueName = periodColumnHeader[5];
            dgFinstat.Columns[7].UniqueName = periodColumnHeader[6];
            dgFinstat.Columns[8].UniqueName = "Harmonic Avg " + periodColumnHeader[1] + "-" + periodColumnHeader[3];
            dgFinstat.Columns[9].UniqueName = "Harmonic Avg " + periodColumnHeader[4] + "-" + periodColumnHeader[6];
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelFinstat).Dispose();
            this.DataContext = null;
        } 
        #endregion
    }
}
