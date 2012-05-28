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
    public partial class ViewContributorDetractor : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelContributorDetractor _dataContextContributorDetractor;
        public ViewModelContributorDetractor DataContextContributorDetractor
        {
            get { return _dataContextContributorDetractor; }
            set { _dataContextContributorDetractor = value; }
        }
        #endregion

        #region Constructor
        public ViewContributorDetractor(ViewModelContributorDetractor dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.ContributorDetractorDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceContributorDetractorLoadEvent);
            this.DataContextContributorDetractor = dataContextSource;
        } 
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceContributorDetractorLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            this.DataContextContributorDetractor.Dispose();
            this.DataContextContributorDetractor.ContributorDetractorDataLoadEvent -= new DataRetrievalProgressIndicatorEventHandler(DataContextSourceContributorDetractorLoadEvent);
            this.DataContextContributorDetractor = null;
            this.DataContext = null;
        }
        #endregion
    }
}
