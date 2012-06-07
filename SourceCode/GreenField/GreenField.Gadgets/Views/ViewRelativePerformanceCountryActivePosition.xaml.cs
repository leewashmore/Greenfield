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
    public partial class ViewRelativePerformanceCountryActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceCountryActivePosition _dataContextRelativePerformanceCountryActivePosition;
        public ViewModelRelativePerformanceCountryActivePosition DataContextRelativePerformanceCountryActivePosition
        {
            get { return _dataContextRelativePerformanceCountryActivePosition; }
            set { _dataContextRelativePerformanceCountryActivePosition = value; }
        }

        #endregion

        #region Constructor
        public ViewRelativePerformanceCountryActivePosition(ViewModelRelativePerformanceCountryActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.CountryActivePositionDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceCountryActivePositionLoadEvent);
            this.DataContextRelativePerformanceCountryActivePosition = dataContextSource;
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceRelativePerformanceCountryActivePositionLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            this.DataContextRelativePerformanceCountryActivePosition.Dispose();
            this.DataContextRelativePerformanceCountryActivePosition.CountryActivePositionDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceCountryActivePositionLoadEvent);
            this.DataContextRelativePerformanceCountryActivePosition = null;
            this.DataContext = null;
        }
        #endregion

        private void dgRelativePerformance_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
