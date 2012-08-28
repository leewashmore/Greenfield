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

namespace GreenField.Gadgets.Views
{
    public partial class ViewCreateUpdatePresentations : ViewBaseUserControl
    {        
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCreateUpdatePresentations _dataContextViewModelCreateUpdatePresentations;
        public ViewModelCreateUpdatePresentations DataContextViewModelCreateUpdatePresentations
        {
            get { return _dataContextViewModelCreateUpdatePresentations; }
            set { _dataContextViewModelCreateUpdatePresentations = value; }
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
                if (DataContextViewModelCreateUpdatePresentations != null) //DataContext instance
                    DataContextViewModelCreateUpdatePresentations.IsActive = _isActive;
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCreateUpdatePresentations(ViewModelCreateUpdatePresentations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCreateUpdatePresentations = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCreateUpdatePresentations.Dispose();
            this.DataContextViewModelCreateUpdatePresentations = null;
            this.DataContext = null;
        }
        #endregion

        private void btnBrowsePowerPoint_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
