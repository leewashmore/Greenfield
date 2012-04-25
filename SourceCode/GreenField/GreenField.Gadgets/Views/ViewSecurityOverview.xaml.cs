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
using System.ComponentModel.Composition;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewSecurityOverview : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSecurityOverview _dataContextSecurityOverview;
        public ViewModelSecurityOverview DataContextSecurityOverview
        {
            get { return _dataContextSecurityOverview; }
            set { _dataContextSecurityOverview = value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSecurityOverview(ViewModelSecurityOverview dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.SecurityOverviewDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceSecurityOverviewLoadEvent);
            this.DataContextSecurityOverview = dataContextSource;
        }
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceSecurityOverviewLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            this.DataContextSecurityOverview.Dispose();
            this.DataContextSecurityOverview.SecurityOverviewDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceSecurityOverviewLoadEvent);
            this.DataContextSecurityOverview = null;
            this.DataContext = null;
        }
        #endregion
    }
}
