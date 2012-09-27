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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    public partial class ViewCompositeFund : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCompositeFund _dataContextCompositeFund;
        public ViewModelCompositeFund DataContextCompositeFund
        {
            get { return _dataContextCompositeFund; }
            set { _dataContextCompositeFund = value; }
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
                if (DataContextCompositeFund != null) //DataContext instance
                    DataContextCompositeFund.IsActive = _isActive;
            }
        }

        #endregion

        #region Constructor
        public ViewCompositeFund(ViewModelCompositeFund dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextCompositeFund = dataContextSource;
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextCompositeFund.Dispose();
            this.DataContextCompositeFund = null;
            this.DataContext = null;
        }
        #endregion
    }
}
