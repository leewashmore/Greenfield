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
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;


namespace GreenField.Gadgets.Views
{
    public partial class ViewMarketCapitalization : ViewBaseUserControl
    {

        #region CONSTRUCTOR
        public ViewMarketCapitalization(ViewModelMarketCapitalization dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
            dataContextSource.MarketCapitalizationDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceMarketCapitalizationLoadEvent);
        }
        #endregion

        #region PROPERTIES

        private ViewModelMarketCapitalization dataContextSource = null;
        public ViewModelMarketCapitalization DataContextSource
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
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSource != null) //DataContext instance
                    DataContextSource.IsActive = isActive;
            }
        }

        #endregion
        #region Event
        /// <summary>
        /// event to handle RadBusyIndicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSourceMarketCapitalizationLoadEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.gridBusyIndicator.IsBusy = true;
            else
                this.gridBusyIndicator.IsBusy = false;
        }
        #endregion
    }
}
