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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.DataContracts;



namespace GreenField.Gadgets.Views
{
    public partial class ViewCommodityIndex : ViewBaseUserControl
    {
        # region PRIVATE FIELDS

        private List<FXCommodityData> _commodityInfo;
        private int _NextYear = DateTime.Now.Year + 1;
        private int _TwoYearsFuture = DateTime.Now.Year + 2;

        #endregion

        #region CONSTRUCTOR

        public ViewCommodityIndex(ViewModelCommodityIndex dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;            
            dataContextSource.CommodityDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceCommodityLoadEvent);
            dataContextSource.RetrieveCommodityDataCompleteEvent += new RetrieveCommodityDataCompleteEventHandler(RetrieveCommodityDataCompletedEvent);

        }

        #endregion
        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceCommodityLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }
        public void RetrieveCommodityDataCompletedEvent(RetrieveCommodityDataCompleteEventArgs e)
        {
            _commodityInfo = e.CommodityInfo;
            if (_commodityInfo != null)
            {                
                dgCommodity.Columns[5].Header = "Price(" + _NextYear.ToString() + ")";
                dgCommodity.Columns[6].Header = "Price(" + _TwoYearsFuture.ToString() + ")";
            }
        }
        #endregion

        private void dgCommodity_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
