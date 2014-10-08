using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using System.Windows;
using System;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// code behind for ViewPresentations
    /// </summary>
    public partial class ViewPresentations : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentations _dataContextViewModelPresentations;
        public ViewModelPresentations DataContextViewModelPresentations
        {
            get { return _dataContextViewModelPresentations; }
            set { _dataContextViewModelPresentations = value; }
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
                if (DataContextViewModelPresentations != null)
                {
                    DataContextViewModelPresentations.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPresentations(ViewModelPresentations dataContextSource,DashboardCategoryType dashboardCategoryType)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentations = dataContextSource;
            this.DataContextViewModelPresentations.DashBoardCategoryType = dashboardCategoryType;
            if (dashboardCategoryType == DashboardCategoryType.INVESTMENT_COMMITTEE_IC_PRESENTATION)
            {
                btnICPPresentationsListEdit.Visibility = Visibility.Visible;
                btnICPPresentationsListView.Visibility = Visibility.Collapsed;
                btnICPPresentationsListChangeDate.Visibility = Visibility.Visible;
            }
            else if (dashboardCategoryType == DashboardCategoryType.INVESTMENT_COMMITTEE_IC_VOTE_DECISION && UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN))
            {
                btnICPPresentationsListView.Visibility = Visibility.Visible;
                btnICPPresentationsListDecisionEntry.Visibility = Visibility.Visible;
                btnICPPresentationsListChangeDate.Visibility = Visibility.Visible;
                btnICPPresentationsListWithdrawRequest.Visibility = Visibility.Visible;
                btnICPPresentationsListEdit.Visibility = Visibility.Collapsed;
                

            }
            else if (dashboardCategoryType == DashboardCategoryType.INVESTMENT_COMMITTEE_IC_VOTE_DECISION && UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_VOTING_MEMBER))
            {
                btnICPPresentationsListView.Visibility = Visibility.Visible;
                btnICPPresentationsListDecisionEntry.Visibility = Visibility.Collapsed;
                btnICPPresentationsListEdit.Visibility = Visibility.Collapsed;

            }

            
        }

        public ViewPresentations(ViewModelPresentations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentations = dataContextSource;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Presentation Overview",
                    Element = this.dgICPPresentationsList, 
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Presentation Overview");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                try
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Presentation Overview",
                        Element = this.dgICPPresentationsList,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Presentation Overview");
                    childExportOptions.Show();
                }
                catch (Exception ex)
                {
                    Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                }
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
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Presentation Overview",
                    Element = this.dgICPPresentationsList,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Presentation Overview");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Element Exporting for Export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event object</param>
        private void dgICPPresentationsList_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
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
            this.DataContextViewModelPresentations.Dispose();
            this.DataContextViewModelPresentations = null;
            this.DataContext = null;
        }
        #endregion
    }
}
