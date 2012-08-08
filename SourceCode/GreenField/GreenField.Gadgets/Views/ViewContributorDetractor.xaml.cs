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

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextContributorDetractor != null) //DataContext instance
                    DataContextContributorDetractor.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        public ViewContributorDetractor(ViewModelContributorDetractor dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextContributorDetractor = dataContextSource;
        } 
        #endregion       

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextContributorDetractor.Dispose();
            this.DataContextContributorDetractor = null;
            this.DataContext = null;
        }
        #endregion

        private void dgContributorDetractor_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
