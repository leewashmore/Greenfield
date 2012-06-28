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

namespace GreenField.Gadgets.Views
{
    public partial class ViewConsensusEstimateSummary : ViewBaseUserControl
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelConsensusEstimateSummary as the data context</param>
        public ViewConsensusEstimateSummary(ViewModelConsensusEstimateSummary dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextConsensusEstimatesSummary = dataContextSource;
            dataContextSource.consensusEstimatesSummaryDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_consensusEstimatesSummaryDataLoadedEvent);
            int currentYear = DateTime.Today.Year;
            this.dgConsensusEstimatesSummary.Columns[0].Header = "Net Income in Currency in #2 (Millions)";
            this.dgConsensusEstimatesSummary.Columns[1].Header = (currentYear - 1).ToString();
            this.dgConsensusEstimatesSummary.Columns[2].Header = (currentYear).ToString();
            this.dgConsensusEstimatesSummary.Columns[3].Header = (currentYear + 1).ToString();
            this.dgConsensusEstimatesSummary.Columns[4].Header = (currentYear + 2).ToString();
            this.dgConsensusEstimatesSummary.Columns[5].Header = (currentYear + 3).ToString();
        }
        #endregion

        private ViewModelConsensusEstimateSummary _dataContextConsensusEstimatesSummary;
        public ViewModelConsensusEstimateSummary DataContextConsensusEstimatesSummary
        {
            get { return _dataContextConsensusEstimatesSummary; }
            set { _dataContextConsensusEstimatesSummary = value; }
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_consensusEstimatesSummaryDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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
    }
}
