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

namespace GreenField.Gadgets.Views
{
    public partial class ViewMarketCapitalization : UserControl
    {
        public ViewMarketCapitalization(ViewModelMarketCapitalization DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;            
            DataContextSource.MarketCapitalizationDataLoadEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceMarketCapitalizationLoadEvent);
        }

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
