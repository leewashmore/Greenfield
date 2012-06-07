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
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Views
{
    public partial class ViewSectorBreakdown : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSectorBreakdown _dataContextSectorBreakdown;
        public ViewModelSectorBreakdown DataContextSectorBreakdown
        {
            get { return _dataContextSectorBreakdown; }
            set { _dataContextSectorBreakdown = value; }
        }
        

        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSectorBreakdown(ViewModelSectorBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSectorBreakdown = dataContextSource;
            dataContextSource.SectorBreakdownDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceSectorBreakdownLoadEvent);
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceSectorBreakdownLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
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
        private void dgSectorBreakdown_Rowloaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e); 
        }
        #endregion

        #region Flip Method
        /// <summary>
        /// Flipping between Grid & PieChart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtSectorBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtSectorBreakdown, this.dgSectorBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgSectorBreakdown, this.crtSectorBreakdown);
            }
        } 
        #endregion

        #region Dispose Method

        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextSectorBreakdown.Dispose();
            this.DataContextSectorBreakdown.SectorBreakdownDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceSectorBreakdownLoadEvent);
            this.DataContextSectorBreakdown = null;
            this.DataContext = null;
        } 
        #endregion

       
    }
}
