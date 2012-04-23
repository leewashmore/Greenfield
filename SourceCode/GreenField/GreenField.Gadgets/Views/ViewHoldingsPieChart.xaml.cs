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
using Telerik.Windows.Controls.Charting;
using GreenField.Gadgets.Helpers;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class of Holdings Pie Chart
    /// </summary>
    public partial class ViewHoldingsPieChart : ViewBaseUserControl
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelHoldingsPieChart as the data context</param>
        public ViewHoldingsPieChart(ViewModelHoldingsPieChart dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextHoldingsPieChart = dataContextSource;
            dataContextSource.holdingsPieChartDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_holdingsPieChartDataLoadedEvent);
            
        }
        #endregion

        private ViewModelHoldingsPieChart _dataContextHoldingsPieChart;
        public ViewModelHoldingsPieChart DataContextHoldingsPieChart
        {
            get { return _dataContextHoldingsPieChart; }
            set { _dataContextHoldingsPieChart = value; }
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_holdingsPieChartDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            this.DataContextHoldingsPieChart.holdingsPieChartDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_holdingsPieChartDataLoadedEvent);
            this.DataContextHoldingsPieChart.Dispose();
            this.DataContextHoldingsPieChart = null;
            this.DataContext = null;
        }
        #endregion
    }
}
