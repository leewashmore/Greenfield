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

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Portfolio Risk Returns Gadget that has ViewModelPortfolioRiskReturns as its data source
    /// </summary>
    public partial class ViewPortfolioRiskReturns : ViewBaseUserControl
    {
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

        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelPortfolioRiskReturns _dataContextRiskReturn;
        public ViewModelPortfolioRiskReturns DataContextRiskReturn
        {
            get { return _dataContextRiskReturn; }
            set { _dataContextRiskReturn = value; }
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

        private void dgPortfolioRiskReturn_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
