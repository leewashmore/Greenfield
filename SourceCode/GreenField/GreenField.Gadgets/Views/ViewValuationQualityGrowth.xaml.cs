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
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewValuationQualityGrowth : ViewBaseUserControl
    {
          #region Constructor
        /// <summary>
        /// Constructor for the class having ViewModelPerformanceGadget as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewValuationQualityGrowth(ViewModelValuationQualityGrowth dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewQualityGrowth = dataContextSource;
            dataContextSource.valuationQualityGrowthDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_valuationQualityGrowthDataLoadedEvent);
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
                if (DataContextViewQualityGrowth != null)
                    DataContextViewQualityGrowth.IsActive = _isActive;
            }
        }

        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelValuationQualityGrowth _dataContextViewQualityGrowth;
        public ViewModelValuationQualityGrowth DataContextViewQualityGrowth
        {
            get { return _dataContextViewQualityGrowth; }
            set { _dataContextViewQualityGrowth = value; }
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_valuationQualityGrowthDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgValuation.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                    {
                          new RadExportOptions() { ElementName = "Valuation,Quality and Growth", Element = this.dgValuation, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_VALUATION_QUALITY_GROWTH_MEASURES);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void dgValuationGrid_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        #endregion

        private void ViewBaseUserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
