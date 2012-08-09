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

//check this
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;

using System.ComponentModel.Composition;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{   
    public partial class ViewICPresenterOverview : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelICPresenterOverview _dataContextICPresentationOverview;
        public ViewModelICPresenterOverview DataContextICPresentationOverview
        {
            get { return _dataContextICPresentationOverview; }
            set { _dataContextICPresentationOverview = value; }
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
                if (DataContextICPresentationOverview != null) //DataContext instance
                    DataContextICPresentationOverview.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewICPresenterOverview(ViewModelICPresenterOverview dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextICPresentationOverview = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextICPresentationOverview.Dispose();
            this.DataContextICPresentationOverview = null;
            this.DataContext = null;
        }
        #endregion

        
    }
}

