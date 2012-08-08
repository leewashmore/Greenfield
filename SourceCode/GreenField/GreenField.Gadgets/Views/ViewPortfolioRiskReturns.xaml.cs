using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Portfolio Risk Returns Gadget that has ViewModelPortfolioRiskReturns as its data source
    /// </summary>
    public partial class ViewPortfolioRiskReturns : ViewBaseUserControl
    {
        #region constructor
        /// <summary>
        /// Constructor for the class having ViewModelPortfolioRiskReturns as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPortfolioRiskReturns(ViewModelPortfolioRiskReturns dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRiskReturn = dataContextSource;           
            dataContextSource.portfolioRiskReturnDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_portfolioRiskReturnDataLoadedEvent);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_portfolioRiskReturnDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        private void dgPortfolioRiskReturn_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
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


                if (this.dgPortfolioRiskReturn.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                  
                      new RadExportOptions() { ElementName = "Portfolio Risk Return", Element = this.dgPortfolioRiskReturn, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_RISK_RETURN);
                    childExportOptions.Show();
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void dgRiskReturnGrid_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelPortfolioRiskReturns _dataContextRiskReturn;
        public ViewModelPortfolioRiskReturns DataContextRiskReturn
        {
            get { return _dataContextRiskReturn; }
            set { _dataContextRiskReturn = value; }
        }
        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextRiskReturn != null)
                    DataContextRiskReturn.IsActive = _isActive;
            }
        }

        #endregion

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRiskReturn.portfolioRiskReturnDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_portfolioRiskReturnDataLoadedEvent);
            this.DataContextRiskReturn.Dispose();
            this.DataContextRiskReturn = null;
            this.DataContext = null;
        }
        #endregion
       
    }
}
