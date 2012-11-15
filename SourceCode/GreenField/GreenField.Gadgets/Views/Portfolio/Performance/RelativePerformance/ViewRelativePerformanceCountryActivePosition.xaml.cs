using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using System.Collections.Generic;
using System.Windows;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View model for ViewContributorDetractor class
    /// </summary>
    public partial class ViewRelativePerformanceCountryActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceCountryActivePosition dataContextRelativePerformanceCountryActivePosition;
        public ViewModelRelativePerformanceCountryActivePosition DataContextRelativePerformanceCountryActivePosition
        {
            get { return dataContextRelativePerformanceCountryActivePosition; }
            set { dataContextRelativePerformanceCountryActivePosition = value; }
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
                if (DataContextRelativePerformanceCountryActivePosition != null) 
                {
                    DataContextRelativePerformanceCountryActivePosition.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceCountryActivePosition(ViewModelRelativePerformanceCountryActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceCountryActivePosition = dataContextSource;
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
                ElementName = "Country Active Position",
                Element = this.dgRelativePerformance,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION);
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
                ElementName = "Country Active Position",
                Element = this.dgRelativePerformance,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION);
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
                ElementName = "Country Active Position",
                Element = this.dgRelativePerformance,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                RichTextBox = this.RichTextBox
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION);
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
            this.DataContextRelativePerformanceCountryActivePosition.Dispose();
            this.DataContextRelativePerformanceCountryActivePosition = null;
            this.DataContext = null;
        }
        #endregion

        
    }
}
