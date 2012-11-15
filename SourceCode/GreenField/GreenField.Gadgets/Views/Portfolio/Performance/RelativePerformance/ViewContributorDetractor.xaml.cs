using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using System.Windows;
using System.Collections.Generic;
using GreenField.Common;
using System;
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View for ContributorDetractor class
    /// </summary>
    public partial class ViewContributorDetractor : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelContributorDetractor _dataContextContributorDetractor;
        public ViewModelContributorDetractor DataContextContributorDetractor
        {
            get { return _dataContextContributorDetractor; }
            set { _dataContextContributorDetractor = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextContributorDetractor != null)
                {
                    DataContextContributorDetractor.IsActive = _isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewContributorDetractor(ViewModelContributorDetractor dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextContributorDetractor = dataContextSource;
        } 
        #endregion       

        #region ExportToExcel/PDF/Print

        #region ExcelExport
        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string PerformanceAttributionUI = "Performance Attribution";
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgContributorDetractor.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                  
                      new RadExportOptions() { ElementName = "Performance Attribution", Element = this.dgContributorDetractor, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER },                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_ATTRIBUTION);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Element Exporting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExcelElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }
        #endregion

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.PerformanceAttributionUI,
                    Element = this.dgContributorDetractor,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR);
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.PerformanceAttributionUI,
                    Element = this.dgContributorDetractor,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR);
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }
                
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextContributorDetractor.Dispose();
            this.DataContextContributorDetractor = null;
            this.DataContext = null;
        }
        #endregion
    }
}
