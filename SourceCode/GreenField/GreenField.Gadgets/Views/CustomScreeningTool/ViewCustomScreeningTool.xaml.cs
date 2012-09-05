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
    public partial class ViewCustomScreeningTool : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCustomScreeningTool _dataContextViewModelCustomScreeningTool;
        public ViewModelCustomScreeningTool DataContextViewModelCustomScreeningTool
        {
            get { return _dataContextViewModelCustomScreeningTool; }
            set { _dataContextViewModelCustomScreeningTool = value; }
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
                if (DataContextViewModelCustomScreeningTool != null) //DataContext instance
                    DataContextViewModelCustomScreeningTool.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCustomScreeningTool(ViewModelCustomScreeningTool dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCustomScreeningTool = dataContextSource;
        }

     
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCustomScreeningTool.Dispose();
            this.DataContextViewModelCustomScreeningTool = null;
            this.DataContext = null;
        }
        #endregion

    }
}
