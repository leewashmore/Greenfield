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
    /// <summary>
    /// Code-Behind for AssetAllocation
    /// </summary>
    public partial class ViewAssetAllocation : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Instance of View-Model
        /// </summary>
        private ViewModelAssetAllocation dataContextAssetAllocation;
        public ViewModelAssetAllocation DataContextAssetAllocation
        {
            get
            {
                return dataContextAssetAllocation;
            }
            set
            {
                dataContextAssetAllocation = value;
            }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextAssetAllocation != null)
                    DataContextAssetAllocation.IsActive = isActive;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">View-Model</param>
        public ViewAssetAllocation(ViewModelAssetAllocation dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextAssetAllocation = dataContextSource;
        }
        
        #endregion

        #region EventUnsubscribe
        public override void Dispose()
        {
            this.DataContextAssetAllocation.Dispose();
            this.DataContextAssetAllocation = null;
            this.DataContext = null;
        }
        #endregion
    }
}
