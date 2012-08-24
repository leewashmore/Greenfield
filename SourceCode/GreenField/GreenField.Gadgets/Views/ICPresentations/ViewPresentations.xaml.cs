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
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;
using GreenField.Gadgets.ViewModels;
using System.ComponentModel.Composition;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    public partial class ViewPresentations : ViewBaseUserControl
    {    
        
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentations _dataContextViewModelPresentations;
        public ViewModelPresentations DataContextViewModelPresentations
        {
            get { return _dataContextViewModelPresentations; }
            set { _dataContextViewModelPresentations = value; }
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
                if (DataContextViewModelPresentations != null) //DataContext instance
                    DataContextViewModelPresentations.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPresentations(ViewModelPresentations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelPresentations = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelPresentations.Dispose();
            this.DataContextViewModelPresentations = null;
            this.DataContext = null;
        }
        #endregion

        private void dgICPPresentationsList_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {

        }

        
    }
}
