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
using GreenField.Gadgets.Helpers;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewTopHoldings : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelTopHoldings _dataContextViewModelTopHoldings;
        public ViewModelTopHoldings DataContextViewModelTopHoldings
        {
            get { return _dataContextViewModelTopHoldings; }
            set { _dataContextViewModelTopHoldings = value; }
        }        

        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewTopHoldings(ViewModelTopHoldings dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.TopHoldingsDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceTopHoldingsLoadedevent);
            this.DataContextViewModelTopHoldings = dataContextSource;
        } 
        #endregion

        private void btn_DetailsClick(object sender, RoutedEventArgs e)
        {
           
        }

        #region Event
        /// <summary>
        ///  event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceTopHoldingsLoadedevent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.gridBusyIndicator.IsBusy = true;
            }
            else
            {
                this.gridBusyIndicator.IsBusy = false;
            }
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelTopHoldings.Dispose();
            this.DataContextViewModelTopHoldings.TopHoldingsDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceTopHoldingsLoadedevent);
            this.DataContextViewModelTopHoldings = null;
            this.DataContext = null;
        } 
        #endregion
       
    }
}
