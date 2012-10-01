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
    /// <summary>
    /// XAML.cs class for CSTDataFieldSelector
    /// </summary>
    public partial class ViewCSTDataFieldSelector : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// Property to set data context
        /// </summary>
        private ViewModelCSTDataFieldSelector dataContextViewModelCSTDataFieldSelector;
        public ViewModelCSTDataFieldSelector DataContextViewModelCSTDataFieldSelector
        {
            get { return dataContextViewModelCSTDataFieldSelector; }
            set { dataContextViewModelCSTDataFieldSelector = value; }
        }

        /// <summary>
        /// Property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                // dataContext instance
                if (DataContextViewModelCSTDataFieldSelector != null) 
                    DataContextViewModelCSTDataFieldSelector.IsActive = isActive;
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
        /// Method to dispose all running events
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
