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
                if (DataContextRelativePerformanceSecurityActivePosition != null) //DataContext instance
                    DataContextRelativePerformanceSecurityActivePosition.IsActive = _isActive;
            }
        }

        #endregion

        #region Constructor
        public ViewRelativePerformanceSecurityActivePosition(ViewModelRelativePerformanceSecurityActivePosition dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceSecurityActivePosition = dataContextSource;
        } 
        #endregion
       
        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceSecurityActivePosition.Dispose();
            this.DataContextRelativePerformanceSecurityActivePosition = null;
            this.DataContext = null;
        }
        #endregion

        private void dgRelativePerformance_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}