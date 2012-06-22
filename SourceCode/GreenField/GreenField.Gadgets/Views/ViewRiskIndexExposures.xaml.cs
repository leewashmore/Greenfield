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
    public partial class ViewRiskIndexExposures : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRiskIndexExposures _dataContextViewModelTopHoldings;
        public ViewModelRiskIndexExposures DataContextViewModelTopHoldings
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
        public ViewRiskIndexExposures(ViewModelRiskIndexExposures dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
           // dataContextSource.RiskIndexExposuresDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRiskIndexExposuresLoadedevent);
            this.DataContextViewModelTopHoldings = dataContextSource;
        }
        #endregion

        #region Event
        /// <summary>
        ///  event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceRiskIndexExposuresLoadedevent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                //this.chartBusyIndicator.IsBusy = true;
            }
            else
            {
                //this.chartBusyIndicator.IsBusy = false;
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
           // this.DataContextViewModelTopHoldings.RiskIndexExposuresDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRiskIndexExposuresLoadedevent);
            this.DataContextViewModelTopHoldings = null;
            this.DataContext = null;
        }
        #endregion

        private void chartRelativerisk_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelRiskIndexExposures != null)
            {
                if ((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo != null)
                {
                    (this.DataContext as ViewModelRiskIndexExposures).AxisXMinValue = Convert.ToDecimal(((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo.OrderBy(a => a.Value)).
                        Select(a => a.Value).FirstOrDefault());
                    (this.DataContext as ViewModelRiskIndexExposures).AxisXMaxValue = Convert.ToDecimal(((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo.OrderByDescending(a => a.Value)).
                        Select(a => a.Value).FirstOrDefault());
                    int dataCount = (this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo.Count;
                    if (dataCount != 0)
                    {
                        this.chartRelativerisk.DefaultView.ChartArea.AxisY.Step = dataCount / 4;
                    }
                }
            }
        }
    }
}
