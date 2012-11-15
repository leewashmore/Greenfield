using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using System.Windows;
using System.Collections.Generic;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View for RelativePerformanceSectorActivePosition class
    /// </summary>
    public partial class ViewRelativePerformanceSectorActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceSectorActivePosition dataContextRelativePerformanceSectorActivePosition;
        public ViewModelRelativePerformanceSectorActivePosition DataContextRelativePerformanceSectorActivePosition
        {
            get { return dataContextRelativePerformanceSectorActivePosition; }
            set { dataContextRelativePerformanceSectorActivePosition = value; }
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
                if (DataContextRelativePerformanceSectorActivePosition != null)
                {
                    DataContextRelativePerformanceSectorActivePosition.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceSectorActivePosition(ViewModelRelativePerformanceSectorActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceSectorActivePosition = dataContextSource;
        } 
        #endregion

        #region Event Handlers
        /// <summary>
        /// catches export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = "Sector Active Position",
                Element = this.dgRelativePerformance,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION);
            childExportOptions.Show();
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = "Sector Active Position",
                Element = this.dgRelativePerformance,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION);
            childExportOptions.Show();
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = "Sector Active Position",
                Element = this.dgRelativePerformance,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION);
            childExportOptions.Show();
        }

        /// <summary>
        /// handles element exporting for grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcelElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceSectorActivePosition.Dispose();
            this.DataContextRelativePerformanceSectorActivePosition = null;
            this.DataContext = null;
        }
        #endregion
    }
}
