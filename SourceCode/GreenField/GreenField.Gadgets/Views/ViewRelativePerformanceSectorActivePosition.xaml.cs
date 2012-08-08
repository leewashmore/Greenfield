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
                if (DataContextRelativePerformanceSectorActivePosition != null) //DataContext instance
                    DataContextRelativePerformanceSectorActivePosition.IsActive = _isActive;
            }
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
            this.DataContextRelativePerformanceSectorActivePosition = dataContextSource;
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceSectorActivePosition.Dispose();
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
