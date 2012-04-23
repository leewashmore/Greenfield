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

        private ViewModelTopHoldings _dataContextViewModelTopHoldings;
        public ViewModelTopHoldings DataContextViewModelTopHoldings
        {
            get { return _dataContextViewModelTopHoldings; }
            set { _dataContextViewModelTopHoldings = value; }
        }        

        #endregion

        #region Constructor
        public ViewTopHoldings(ViewModelTopHoldings dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.TopHoldingsDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceTopHoldingsLoadedevent);
            this.DataContextViewModelTopHoldings = dataContextSource;
        } 
        #endregion

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

        #region Dispose Method
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
