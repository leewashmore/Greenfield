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

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Portfolio Risk Returns Gadget that has ViewModelPortfolioRiskReturns as its data source
    /// </summary>
    public partial class ViewPortfolioRiskReturns : UserControl
    {
        /// <summary>
        /// Constructor for the class having ViewModelPortfolioRiskReturns as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPortfolioRiskReturns(ViewModelPortfolioRiskReturns dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
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

        #endregion
    }
}
