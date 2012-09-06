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
    public partial class ViewCSTDataFieldSelector : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCSTDataFieldSelector _dataContextViewModelCSTDataFieldSelector;
        public ViewModelCSTDataFieldSelector DataContextViewModelCSTDataFieldSelector
        {
            get { return _dataContextViewModelCSTDataFieldSelector; }
            set { _dataContextViewModelCSTDataFieldSelector = value; }
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
                if (DataContextViewModelCSTDataFieldSelector != null) //DataContext instance
                    DataContextViewModelCSTDataFieldSelector.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCSTDataFieldSelector(ViewModelCSTDataFieldSelector dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCSTDataFieldSelector = dataContextSource;
        }

     
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCSTDataFieldSelector.Dispose();
            this.DataContextViewModelCSTDataFieldSelector = null;
            this.DataContext = null;
        }
        #endregion

    }
}
