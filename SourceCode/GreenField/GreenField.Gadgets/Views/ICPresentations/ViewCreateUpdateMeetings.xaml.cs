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
    public partial class ViewCreateUpdateMeetings : ViewBaseUserControl
    {
       
           #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCreateUpdateMeetings _dataContextViewModelCreateUpdateMeetings;
        public ViewModelCreateUpdateMeetings DataContextViewModelCreateUpdateMeetings
        {
            get { return _dataContextViewModelCreateUpdateMeetings; }
            set { _dataContextViewModelCreateUpdateMeetings = value; }
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
                if (DataContextViewModelCreateUpdateMeetings != null) //DataContext instance
                    DataContextViewModelCreateUpdateMeetings.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCreateUpdateMeetings(ViewModelCreateUpdateMeetings dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCreateUpdateMeetings = dataContextSource;
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCreateUpdateMeetings.Dispose();
            this.DataContextViewModelCreateUpdateMeetings = null;
            this.DataContext = null;
        }
        #endregion
    }
}
