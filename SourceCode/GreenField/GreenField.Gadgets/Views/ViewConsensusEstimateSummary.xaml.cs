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
            dataContextSource.ConsensusEstimatesSummaryDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_consensusEstimatesSummaryDataLoadedEvent);
            dataContextSource.RetrieveConsensusEstimatesSummaryDataCompletedEvent += new 
                RetrieveConsensusEstimatesSummaryCompleteEventHandler(dataContextSource_RetrieveConsensusEstimatesSummaryDataCompletedEvent);
            int currentYear = DateTime.Today.Year;
            this.dgConsensusEstimatesSummary.Columns[0].Header = "Net Income (Millions)";
            this.dgConsensusEstimatesSummary.Columns[1].Header = (currentYear - 1).ToString();
            this.dgConsensusEstimatesSummary.Columns[2].Header = (currentYear).ToString();
            this.dgConsensusEstimatesSummary.Columns[3].Header = (currentYear + 1).ToString();
            this.dgConsensusEstimatesSummary.Columns[4].Header = (currentYear + 2).ToString();
            this.dgConsensusEstimatesSummary.Columns[5].Header = (currentYear + 3).ToString();
        }        
        #endregion

        #region Properties
        /// <summary>
        /// Data Context Property
        /// </summary>
        private ViewModelConsensusEstimateSummary dataContextConsensusEstimatesSummary;
        public ViewModelConsensusEstimateSummary DataContextConsensusEstimatesSummary
        {
            get { return dataContextConsensusEstimatesSummary; }
            set { dataContextConsensusEstimatesSummary = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (this.DataContext != null)
                {
                    ((ViewModelConsensusEstimateSummary)this.DataContext).IsActive = isActive;
                }
            }
        }
        #endregion

        #region Event Handler
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

        /// <summary>
        /// Data Completion Event
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_RetrieveConsensusEstimatesSummaryDataCompletedEvent(RetrieveConsensusSummaryCompletedEventsArgs e)
        {
            if (e.ConsensusInfo != null)
            {
                this.dgConsensusEstimatesSummary.Columns[0].Header = "Net Income in " + e.ConsensusInfo[0].currency + " (Millions)";
            }
        }
        #endregion

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextConsensusEstimatesSummary.ConsensusEstimatesSummaryDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_consensusEstimatesSummaryDataLoadedEvent);
            this.DataContextConsensusEstimatesSummary.Dispose();
            this.DataContextConsensusEstimatesSummary = null;
            this.DataContext = null;
        }
        #endregion
    }
}
