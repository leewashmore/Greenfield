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
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;


namespace GreenField.Gadgets.Views
{
    public partial class ViewBasicData : ViewBaseUserControl
    {
        #region PROPERTIES

        /// <summary>
        /// Private variable to hold data
        /// </summary>
        private ViewModelBasicData dataContextSource = null;
        public ViewModelBasicData DataContextSourceModel
        {
            get
            {
                return dataContextSource;
            }
            set
            {
                if (value != null)
                    dataContextSource = value;
            }
        }
        /// <summary>
        /// Private variable to hold IsActive property of parent user control
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSourceModel != null) //DataContext instance
                    DataContextSourceModel.IsActive = isActive;
            }
        }
        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DataContextSource"></param>
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
