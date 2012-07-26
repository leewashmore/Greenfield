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
using GreenField.Common;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    public partial class ViewBasicData : ViewBaseUserControl
    {

        private ViewModelBasicData _dataContextSource = null;
        public ViewModelBasicData DataContextSourceModel
        {
            get
            {
                return _dataContextSource;
            }
            set
            {
                if (value != null)
                    _dataContextSource = value;
            }
        } 

        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextSourceModel != null) //DataContext instance
                    DataContextSourceModel.IsActive = _isActive;
            }
        }
        #region CONSTRUCTOR
        public ViewBasicData(ViewModelBasicData DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
            this.DataContextSourceModel = DataContextSource;
            DataContextSource.BasicDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceBasicDataLoadEvent);
            
        }
        #endregion

        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceBasicDataLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }
        #endregion
    }
}
