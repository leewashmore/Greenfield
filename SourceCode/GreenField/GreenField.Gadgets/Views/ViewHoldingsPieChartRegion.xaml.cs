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
    /// <summary>
    /// View Class of Holdings Pie Chart
    /// </summary>
    public partial class ViewHoldingsPieChartRegion : ViewBaseUserControl
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelHoldingsPieChart as the data context</param>
        public ViewHoldingsPieChartRegion(ViewModelHoldingsPieChartRegion dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextHoldingsPieChartRegion = dataContextSource;
            dataContextSource.holdingsPieChartForRegionDataLoadedEvent +=
                new DataRetrievalProgressIndicatorEventHandler(dataContextSource_holdingsPieChartRegionDataLoadedEvent);

        }
        #endregion

        private ViewModelHoldingsPieChartRegion _dataContextHoldingsPieChartRegion;
        public ViewModelHoldingsPieChartRegion DataContextHoldingsPieChartRegion
        {
            get { return _dataContextHoldingsPieChartRegion; }
            set { _dataContextHoldingsPieChartRegion = value; }
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_holdingsPieChartRegionDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorChart.IsBusy = true;
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorChart.IsBusy = false;
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        #region RemoveEvents

        public override void Dispose()
        {
            this.DataContextHoldingsPieChartRegion.holdingsPieChartForRegionDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_holdingsPieChartRegionDataLoadedEvent);
            this.DataContextHoldingsPieChartRegion.Dispose();
            this.DataContextHoldingsPieChartRegion = null;
            this.DataContext = null;
        }
        #endregion
    }
}
