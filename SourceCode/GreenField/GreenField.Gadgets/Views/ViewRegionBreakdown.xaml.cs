using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;


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

        /// <summary>
        /// Disabling the indentation when grouping is applied in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRegionBreakdown_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            //var row = e.Row as GridViewRow;
            //if (row != null)
            //{
            //    var indent = row.ChildrenOfType<GridViewIndentCell>().FirstOrDefault();
            //    if (indent != null)
            //    { indent.Visibility = Visibility.Collapsed; }
            //}
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
