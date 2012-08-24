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
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;



namespace GreenField.Gadgets.Views
{
    public partial class ViewCommodityIndex : ViewBaseUserControl
    {
        # region PRIVATE FIELDS

        private List<FXCommodityData> _commodityInfo;
        private int _NextYear = DateTime.Now.Year + 1;
        private int _TwoYearsFuture = DateTime.Now.Year + 2;

        #endregion

        #region PROPERTIES

        private ViewModelCommodityIndex _dataContextSource = null;
        public ViewModelCommodityIndex DataContextSource
        {
            get
            {
                return _dataContextSource;
            }
            set
            {
                if (value != null)
                    _dataContextSource = value;
            }
        }

        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextSource != null) //DataContext instance
                    DataContextSource.IsActive = _isActive;
            }
        }

        #endregion

        #region CONSTRUCTOR

        public ViewCommodityIndex(ViewModelCommodityIndex dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
            //dataContextSource.CommodityDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceCommodityLoadEvent);
            dataContextSource.RetrieveCommodityDataCompleteEvent += new RetrieveCommodityDataCompleteEventHandler(RetrieveCommodityDataCompletedEvent);

        }

        #endregion
        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        //void DataContextSourceCommodityLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        //{
        //    if (e.ShowBusy)
        //        this.gridBusyIndicator.IsBusy = true;
        //    else
        //        this.gridBusyIndicator.IsBusy = false;
        //}
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
        #region ExportToExcel

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

                if (LayoutRoot.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Commodity Data", Element = this.dgCommodity, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });
                    return;
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.MODELS_FX_MACRO_ECONOMICS_COMMODITY_INDEX_RETURN);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        /// <summary>
        /// Event for Grid Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementExportingEvent(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        #endregion
    }
}
