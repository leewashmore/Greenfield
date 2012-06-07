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
    public partial class ViewRelativePerformanceSectorActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceSectorActivePosition _dataContextRelativePerformanceSectorActivePosition;
        public ViewModelRelativePerformanceSectorActivePosition DataContextRelativePerformanceSectorActivePosition
        {
            get { return _dataContextRelativePerformanceSectorActivePosition; }
            set { _dataContextRelativePerformanceSectorActivePosition = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceSectorActivePosition(ViewModelRelativePerformanceSectorActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.SectorActivePositionDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceSectorActivePositionLoadEvent);
            this.DataContextRelativePerformanceSectorActivePosition = dataContextSource;
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceRelativePerformanceSectorActivePositionLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceSectorActivePosition.Dispose();
            this.DataContextRelativePerformanceSectorActivePosition.SectorActivePositionDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceSectorActivePositionLoadEvent);
            this.DataContextRelativePerformanceSectorActivePosition = null;
            this.DataContext = null;
        }
        #endregion

        private void dgRelativePerformance_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
