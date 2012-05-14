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
    public partial class ViewRelativePerformanceSecurityActivePosition : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRelativePerformanceSecurityActivePosition _dataContextRelativePerformanceSecurityActivePosition;
        public ViewModelRelativePerformanceSecurityActivePosition DataContextRelativePerformanceSecurityActivePosition
        {
            get { return _dataContextRelativePerformanceSecurityActivePosition; }
            set { _dataContextRelativePerformanceSecurityActivePosition = value; }
        }

        #endregion

        #region Constructor
        public ViewRelativePerformanceSecurityActivePosition(ViewModelRelativePerformanceSecurityActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.SecurityActivePositionDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceSecurityActivePositionLoadEvent);
            this.DataContextRelativePerformanceSecurityActivePosition = dataContextSource;
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceRelativePerformanceSecurityActivePositionLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            this.DataContextRelativePerformanceSecurityActivePosition.Dispose();
            this.DataContextRelativePerformanceSecurityActivePosition.SecurityActivePositionDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRelativePerformanceSecurityActivePositionLoadEvent);
            this.DataContextRelativePerformanceSecurityActivePosition = null;
            this.DataContext = null;
        }
        #endregion
    }
}