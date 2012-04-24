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
using Telerik.Windows.Data;
using System.ComponentModel;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRegionBreakdown : ViewBaseUserControl
    {
        #region Property
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRegionBreakdown _dataContextRegionBreakdown;
        public ViewModelRegionBreakdown DataContextRegionBreakdown
        {
            get { return _dataContextRegionBreakdown; }
            set { _dataContextRegionBreakdown = value; }
        } 
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRegionBreakdown(ViewModelRegionBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRegionBreakdown = dataContextSource;
            dataContextSource.RegionBreakdownDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRegionBreakdownLoadEvent);
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceRegionBreakdownLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }

        #endregion

        /// <summary>
        /// Flipping between Grid & PieChart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtRegionBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtRegionBreakdown, this.dgRegionBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgRegionBreakdown, this.crtRegionBreakdown);
            }
        }

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRegionBreakdown.Dispose();
            this.DataContextRegionBreakdown.RegionBreakdownDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRegionBreakdownLoadEvent);
            this.DataContextRegionBreakdown = null;
            this.DataContext = null;
        } 
        #endregion
    }
}
